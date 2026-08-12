using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Loaders;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Hosted;

namespace LiveLikeBossSpawnChances.Server.Internal;

// Copied from https://github.com/sp-tarkov/server-csharp/blob/main/Testing/UnitTests/DI.cs

[TestFixture]
public class DI
{
    private static IServiceProvider _serviceProvider = default!;

    private static DI? _instance;

    private DI(IReadOnlyDictionary<Type, BaseConfig> configuration, LocaleTable locales, ILogger logger)
    {
        ConfigureServices(configuration, locales, logger);
    }

    public static DI GetInstance(IReadOnlyDictionary<Type, BaseConfig> configuration, LocaleTable locales, ILogger logger)
    {
        return _instance ??= new DI(configuration, locales, logger);
    }

    private DatabaseTables SetupDB(IReadOnlyDictionary<Type, BaseConfig> configuration, LocaleTable locales, ILogger logger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(locales);
        foreach (var configEntry in configuration)
        {
            services.AddSingleton(configEntry.Key, configEntry.Value);
        }
        services.AddSingleton(logger);
        services.AddSingleton(typeof(ILogger<>), typeof(MockLogger<>));
        services.AddSingleton(typeof(ISptLogger<>), typeof(MockLogger<>));

        var diHandler = new DependencyInjectionHandler(services);
        diHandler.AddInjectableTypesFromAssembly(typeof(Program).Assembly);
        diHandler.AddInjectableTypesFromAssembly(typeof(SPTStartupHostedService).Assembly);
        diHandler.InjectAll();
        services.AddSingleton<DatabaseImporter>();

        var serviceProvider = services.BuildServiceProvider();
        var dbImporter = serviceProvider.GetRequiredService<DatabaseImporter>();
        var tables = dbImporter.LoadDatabaseAsync(false).GetAwaiter().GetResult();

        return tables is null ? throw new InvalidOperationException("Tables aren't loaded lol") : tables;
    }

    private void ConfigureServices(IReadOnlyDictionary<Type, BaseConfig> configuration, LocaleTable locales, ILogger logger)
    {
        if (_serviceProvider != null)
        {
            return;
        }

        var services = new ServiceCollection();
        services.AddSingleton(logger);
        services.AddSingleton(typeof(ILogger<>), typeof(MockLogger<>));
        services.AddSingleton(typeof(ISptLogger<>), typeof(MockLogger<>));
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        var db = SetupDB(configuration, locales, logger);
        services.AddSingleton(db.Bots);
        services.AddSingleton(db.Hideout);
        services.AddSingleton(db.Locales);
        services.AddSingleton(db.Locations);
        services.AddSingleton(db.Match);
        services.AddSingleton(db.Templates);
        services.AddSingleton(db.Traders);
        services.AddSingleton(db.Globals);
        services.AddSingleton(db.Server);
        services.AddSingleton(db.Settings);

        foreach (var configEntry in configuration)
        {
            services.AddSingleton(configEntry.Key, configEntry.Value);
        }

        var diHandler = new DependencyInjectionHandler(services);

        diHandler.AddInjectableTypesFromTypeAssembly(typeof(SPTStartupHostedService));

        diHandler.InjectAll();

        services.AddSingleton<IReadOnlyList<SptMod>>(_ => []);

        _serviceProvider = services.BuildServiceProvider();

        var cancellationTokenSource = new CancellationTokenSource();

        foreach (var onLoad in _serviceProvider.GetServices<IOnLoad>())
        {
            onLoad.OnLoadAsync(cancellationTokenSource.Token).Wait();
        }
    }

    public T GetService<T>()
        where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}