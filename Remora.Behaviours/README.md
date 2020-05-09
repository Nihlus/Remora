Remora.Behaviours
=================

Remora.Behaviours is a simple library for creating independently acting behaviours for your application, in the style
of behavioural programming. At its core, behavioural programming divides your application up into independently acting
threads of behaviour, which operate as independently of each other as possible. This approach allows you to easily
change or swap different components in your program without affecting the entire architecture, and integrates
parallelism more naturally in your workflow.

The library provides two base classes for behaviours - `BehaviourBase`, used for creating your own specially tailored
behaviours, and `ContinuousBehaviour`, which is a general-purpose behaviour for an action that should periodically
recur.

Typical usages for a behaviour might be hitting a remote API at intervals, running compaction jobs for a database,
sending a keep-alive heartbeat, or routing events in an asynchronous manner.

## Usage
Remora.Behaviours is built with systems using Microsoft's dependency injection framework in mind, but can be plugged
into any application with ease. The below examples are using the DI framework.

```c#
// Register the behaviour service, as well as any other required services
var services = new ServiceCollection()
    .AddScoped<MyService>()
    .AddScoped<BehaviourService>()
    .BuildServiceProvider();

var behaviourService = services.GetRequiredService<BehaviourService>();

// Either add your behaviours individually
await behaviourService.AddBehaviourAsync<MyBehaviour>(services);

// Or add all behaviours in an assembly
await behaviourService.AddBehavioursAsync(Assembly.GetEntryAssembly());

await behaviourService.StartBehavioursAsync();
```

Behaviours are implicitly singletons, and are instantiated using DI when using the behaviour service. The behaviours
themselves are simple to declare and use.

```c#
public sealed class MyBehaviour : BehaviourBase<MyBehaviour>
{
    public MyBehaviour([NotNull] IServiceScope serviceScope, [NotNull] ILogger<MyBehaviour> logger)
        : base(serviceScope, logger)
    {
    }

    public MyBehaviour([NotNull] IServiceScope serviceScope)
        : base(serviceScope)
    {
    }

    protected override Task OnStartingAsync()
    {
        // Implement startup procedures here. This method is optional, and a no-op by default.

        return base.OnStartingAsync();
    }

    protected override Task OnStoppingAsync()
    {
        // Implement shutdown procedures here. This method is optional, and a no-op by default.

        return base.OnStoppingAsync();
    }
}
```

```c#
public sealed class MyContinuousBehaviour : ContinuousBehaviour<MyContinuousBehaviour>
{
    public MyContinuousBehaviour([NotNull] IServiceScope serviceScope, [NotNull] ILogger<MyContinuousBehaviour> logger)
        : base(serviceScope, logger)
    {
    }

    public MyContinuousBehaviour([NotNull] IServiceScope serviceScope)
        : base(serviceScope)
    {
    }

    protected override Task OnStartingAsync()
    {
        // Implement startup procedures here. This method is optional, and a no-op by default.

        return base.OnStartingAsync();
    }

    protected override Task OnTickAsync(CancellationToken ct)
    {
        // Implement recurring action here. It is typical to implement small delay between each invocation to reduce CPU
        // pressure, unless otherwise required.

        throw new NotImplementedException();
    }

    protected override Task OnStoppingAsync()
    {
        // Implement shutdown procedures here. This method is optional, and a no-op by default.

        return base.OnStoppingAsync();
    }
}
```

## Building
The library does not require anything out of the ordinary to compile.

```bash
cd $SOLUTION_DIR
dotnet build
dotnet pack -c Release
```

## Downloading
Get it on [NuGet][1].


[1]: https://www.nuget.org/packages/Remora.Behaviours/
