//
//  PluginDependencyTreeNode.cs
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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remora.Plugins.Abstractions;

namespace Remora.Plugins;

/// <summary>
/// Represents a node in a dependency tree.
/// </summary>
[PublicAPI]
public sealed class PluginDependencyTreeNode
{
    private readonly List<PluginDependencyTreeNode> _dependents;

    /// <summary>
    /// Gets the plugin.
    /// </summary>
    public IPluginDescriptor Plugin { get; }

    /// <summary>
    /// Gets the nodes that depend on this plugin.
    /// </summary>
    public IReadOnlyCollection<PluginDependencyTreeNode> Dependents => _dependents;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginDependencyTreeNode"/> class.
    /// </summary>
    /// <param name="plugin">The plugin.</param>
    /// <param name="dependants">The dependants.</param>
    public PluginDependencyTreeNode
    (
        IPluginDescriptor plugin,
        List<PluginDependencyTreeNode>? dependants = null
    )
    {
        this.Plugin = plugin;
        _dependents = dependants ?? new List<PluginDependencyTreeNode>();
    }

    /// <summary>
    /// Adds a dependant to this node.
    /// </summary>
    /// <param name="node">The node.</param>
    internal void AddDependent(PluginDependencyTreeNode node)
    {
        if (_dependents.Contains(node))
        {
            return;
        }

        _dependents.Add(node);
    }

    /// <summary>
    /// Gets all the dependant plugins in this branch.
    /// </summary>
    /// <returns>The dependant plugins.</returns>
    public IEnumerable<PluginDependencyTreeNode> GetAllDependents()
    {
        foreach (var dependant in this.Dependents)
        {
            yield return dependant;

            foreach (var sub in dependant.GetAllDependents())
            {
                yield return sub;
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Plugin} => ({string.Join(", ", _dependents.Select(d => d.Plugin))})";
    }
}
