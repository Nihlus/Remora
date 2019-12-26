//
//  PluginService.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;
using Remora.Plugins.Extensions;
using Remora.Plugins.Results;

namespace Remora.Plugins.Services
{
    /// <summary>
    /// Serves functionality related to plugins.
    /// </summary>
    [PublicAPI]
    public sealed class PluginService
    {
        /// <summary>
        /// Loads the available plugins into a dependency tree.
        /// </summary>
        /// <returns>The dependency tree.</returns>
        [PublicAPI, NotNull, Pure]
        public PluginDependencyTree LoadPluginDescriptors()
        {
            var pluginAssemblies = LoadAvailablePluginAssemblies().ToList();
            var pluginsWithDependencies = pluginAssemblies.ToDictionary
            (
                a => a.PluginAssembly,
                a => a.PluginAssembly.GetReferencedAssemblies()
                    .Where(ra => pluginAssemblies.Any(pa => pa.PluginAssembly.FullName == ra.FullName))
                    .Select(ra => pluginAssemblies.First(pa => pa.PluginAssembly.FullName == ra.FullName))
                    .Select(ra => ra.PluginAssembly)
            );

            bool IsDependency(Assembly assembly, Assembly other)
            {
                var dependencies = pluginsWithDependencies[assembly];
                foreach (var dependency in dependencies)
                {
                    if (dependency == other)
                    {
                        return true;
                    }

                    if (IsDependency(dependency, other))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsDirectDependency(Assembly assembly, Assembly dependency)
            {
                var dependencies = pluginsWithDependencies[assembly];
                if (IsDependency(assembly, dependency) && dependencies.All(d => !IsDependency(d, dependency)))
                {
                    return true;
                }

                return false;
            }

            var tree = new PluginDependencyTree();
            var nodes = new Dictionary<Assembly, PluginDependencyTreeNode>();

            var sorted = pluginsWithDependencies.Keys.TopologicalSort(k => pluginsWithDependencies[k]).ToList();
            while (sorted.Count > 0)
            {
                var current = sorted[0];
                var loadDescriptorResult = LoadPluginDescriptor(current);
                if (!loadDescriptorResult.IsSuccess)
                {
                    continue;
                }

                var node = new PluginDependencyTreeNode(loadDescriptorResult.Plugin);

                var dependencies = pluginsWithDependencies[current].ToList();
                if (!dependencies.Any())
                {
                    // This is a root of a chain
                    tree.AddBranch(node);
                }

                foreach (var dependency in dependencies)
                {
                    if (!IsDirectDependency(current, dependency))
                    {
                        continue;
                    }

                    var dependencyNode = nodes[dependency];
                    dependencyNode.AddDependant(node);
                }

                nodes.Add(current, node);
                sorted.Remove(current);
            }

            return tree;
        }

        /// <summary>
        /// Loads the available plugins.
        /// </summary>
        /// <returns>The descriptors of the available plugins.</returns>
        [PublicAPI, Pure, NotNull, ItemNotNull]
        public IEnumerable<IPluginDescriptor> LoadAvailablePlugins()
        {
            var pluginAssemblies = LoadAvailablePluginAssemblies().ToList();
            var sorted = pluginAssemblies.TopologicalSort
            (
                a => a.PluginAssembly.GetReferencedAssemblies()
                    .Where
                    (
                        n => pluginAssemblies.Any(pa => pa.PluginAssembly.GetName().FullName == n.FullName)
                    )
                    .Select
                    (
                        n => pluginAssemblies.First(pa => pa.PluginAssembly.GetName().FullName == n.FullName)
                    )
            );

            foreach (var pluginAssembly in sorted)
            {
                var descriptor = (IPluginDescriptor?)Activator.CreateInstance
                (
                    pluginAssembly.PluginAttribute.PluginDescriptor
                );

                if (descriptor is null)
                {
                    continue;
                }

                yield return descriptor;
            }
        }

        /// <summary>
        /// Loads the plugin descriptor from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The plugin descriptor.</returns>
        [Pure, NotNull]
        private LoadPluginResult LoadPluginDescriptor([NotNull] Assembly assembly)
        {
            var pluginAttribute = assembly.GetCustomAttribute<RemoraPlugin>();
            if (pluginAttribute is null)
            {
                return LoadPluginResult.FromError("No plugin attribute found on the assembly.");
            }

            var descriptor = (IPluginDescriptor?)Activator.CreateInstance(pluginAttribute.PluginDescriptor);
            if (descriptor is null)
            {
                return LoadPluginResult.FromError("Failed to create an instance of the plugin descriptor.");
            }

            return LoadPluginResult.FromSuccess(descriptor);
        }

        /// <summary>
        /// Loads the available plugin assemblies.
        /// </summary>
        /// <returns>The available assemblies.</returns>
        [Pure, NotNull]
        private IEnumerable<(RemoraPlugin PluginAttribute, Assembly PluginAssembly)> LoadAvailablePluginAssemblies()
        {
            var entryAssemblyPath = Assembly.GetEntryAssembly()?.Location;

            if (entryAssemblyPath is null)
            {
                yield break;
            }

            var installationDirectory = Directory.GetParent(entryAssemblyPath);
            var assemblies = Directory.EnumerateFiles
            (
                installationDirectory.FullName,
                "*.dll",
                SearchOption.AllDirectories
            );

            foreach (var assemblyPath in assemblies)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                    continue;
                }

                var pluginAttribute = assembly.GetCustomAttribute<RemoraPlugin>();
                if (pluginAttribute is null)
                {
                    continue;
                }

                yield return (pluginAttribute, assembly);
            }
        }
    }
}
