# AdUnification
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
AdUnification serves as a lightweight system for implementing any advertisement into a project and the ability to combine them,
without having to add new event dispatch points to the code for each of them.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
    - [Initialization](#Initialization)
    - [Custom Providers, Adapters & Configs](#Custom-Providers,-Adapters-&-Configs)
    - [AdService Methods](#AdService-Methods)
    - [Requests And Responses](#Requests-And-Responses)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/AdUnification/releases/) page.
2. Open AdUnification.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.adunification": "https://github.com/DanilChizhikov/AdUnification.git?path=Assets/AdUnification",
      ```
UPM should now install the package.

## Basic Usage

### Initialization
First, you need to initialize the AdService, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public class AdServiceBootstrap : MonoBehaviour
{
    [SerializeField] private ExampleConfig _config = default;
    
    private static IAdService _service;

    public static IAdService Service => _service;

    private void Awake()
    {
        if (_service != null)
        {
            Destroy(gameObject);
            return;
        }

        var adapters = new List<IAdAdapter>()
                {
                        new ExampleRewardedAdapter(_config),
                };

        var provider = new ExampleProvider(_config, adapters);
        _service = new AdService(provider);
        _service.Initialize();
    }
}
```

### Custom Providers, Adapters & Configs
In order to create your own custom config for the advertisement provider and adapter,
it is enough to inherit from the abstract ScriptableAdConfig class or IAdConfig interface.

Example Config:
```csharp
public class ExampleConfig : ScriptableAdConfig
{
    // Yours config data
}
```

In order to create your own custom advertisement adapter, you can use two options,
inherit from the ready-made abstract class AdAdapter<TConfig>, which accepts the IAdConfig as a generic parameter,
or you can inherit from the IAdAdapter interface and implement all the methods yourself.

Example Adapter:
```csharp
public sealed class ExampleRewardedAdapter : ExampleAdapter
{
    public override event Action<AdType> OnAdLoaded;
    
    public override AdType ServicedAdType { get; }
    public override bool IsReady { get; }
    public override bool IsShowing { get; }
    
    public ExampleRewardedAdapter(ExampleConfig config) : base(config) { }

    protected override void InitializeProcessing()
    {
        base.InitializeProcessing();
    }

    protected override bool ShowAdProcessing(IAdRequest request)
    {
        // some code ...
    }

    protected override void HideAdProcessing()
    {
        // some code ...
    }

    protected override void DeInitializeProcessing()
    {
        base.DeInitializeProcessing();
    }
}
```

Use protected method ```SendAdShown(IAdResponse response)``` when you show ad.

Next, you need to create a provider that will collect all the adapters and manage advertising through them.

To create a provider, it is enough to inherit a new class from AdProvider<Config, TAdapter> which takes as generic parameters config and adapter, which are IAdConfig and IAdAdapter.

Example Provider:
```csharp
public sealed class ExampleProvider : AdProvider<ExampleConfig, ExampleAdapter>
{
    public ExampleProvider(ExampleConfig config, IEnumerable<ExampleAdapter> adapters) : base(config, adapters) { }

    protected override void InitializeProcessing()
    {
        base.InitializeProcessing();
    }

    protected override void DeInitializeProcessing()
    {
        base.DeInitializeProcessing();
    }
}
```

### AdService Methods

```csharp
public interface IAdService
{
    //Called when ad was load
    event Action<AdType> OnAdLoaded;
    //Called before shown ad
    event Action<AdType> OnAdBeganShow;
    //Called after shown ad
    event Action<IAdResponse> OnAdShown;
    
    bool IsInitialized { get; }
    bool AnyAdIsShowing { get; }

    //Need for initialize
    void Initialize();
    //Check ad is ready by type
    bool IsReady(AdType type);
    //Check ad is showing by type
    bool IsShowing(AdType type);
    //Try show ad by request
    bool TryShowAd(IAdRequest request);
    //Hide ad by type
    void HideAd(AdType type);
}
```

### Requests And Responses
```csharp
public interface IAdRequest
{
    AdType Type { get; }
    string Placement { get; }
}
```
```csharp
public interface IAdResponse
{
    AdType Type { get; }
    bool IsSuccessful { get; }
}
```

## License

MIT