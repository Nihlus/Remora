Remora.Plugins
==============

Remora.Plugins is the default implementation of the Remora.Plugins.Abstractions package, providing a dynamic plugin
system for your applications. In short, class libraries can be written as standalone packages and loaded at runtime as
integrated parts of your application, allowing loose coupling and easily swappable components. The plugin system is
designed around Microsoft's dependency injection framework.

## Usage
Creating a plugin is as simple as creating a class library project, and annotating the assembly with an attribute
denoting the type used as a plugin descriptor. The descriptor acts as the entry point of the plugin, as well as an
encapsulation point for information about it.

```c#
[assembly: RemoraPlugin(typeof(MyPlugin))]

public sealed class MyPlugin : PluginDescriptor
{
    /// <inheritdoc />
    public override string Name => "My Plugin";

    /// <inheritdoc />
    public override string Description => "My plugin that does a thing.";

    /// <inheritdoc/>
    public override Task<bool> RegisterServicesAsync(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<MyService>()

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public override async Task<bool> InitializeAsync(IServiceProvider serviceProvider)
    {
        var myService = serviceProvider.GetRequiredService<MyService>();
        await myService.DoTheThingAsync();

        return true;
    }
}
```

Loading plugins in your application is equally simple. The example below is perhaps a little convoluted, but shows the
flexibility of the system.

```c#
var pluginService = new PluginService();

var serviceCollection = new ServiceCollection()
    .AddSingleton(pluginService);

var successfullyRegisteredPlugins = new List<IPluginDescriptor>();

var availablePlugins = pluginService.LoadAvailablePlugins();
foreach (var availablePlugin in availablePlugins)
{
    if (!await availablePlugin.RegisterServicesAsync(serviceCollection))
    {
        this.Log.LogWarning
        (
            $"The plugin \"{availablePlugin.Name}\" (v{availablePlugin.Version}) failed to " +
            "register its services. It will not be loaded."
        );

        continue;
    }

    successfullyRegisteredPlugins.Add(availablePlugin);
}

_services = serviceCollection.BuildServiceProvider();

foreach (var plugin in successfullyRegisteredPlugins)
{
    if (!await plugin.InitializeAsync(_services))
    {
        this.Log.LogWarning
        (
            $"The plugin \"{plugin.Name}\"" +
            $" (v{plugin.Version}) failed to initialize. It may not be functional."
        );
    }
}
```

Plugins should be designed in such a way that a registration or initialization failure does not corrupt the application.

## Building
The library does not require anything out of the ordinary to compile.

```bash
cd $SOLUTION_DIR
dotnet build
dotnet pack -c Release
```

## Downloading
Get it on [NuGet][1].


[1]: https://www.nuget.org/packages/Remora.Plugins/
