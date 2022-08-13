using SharedLibraryCore;
using SharedLibraryCore.Interfaces;

namespace IW4MNameReserve;

public class Plugin : IPlugin
{
    public string Name => "Name Reserve";
    public float Version => 20220813f;
    public string Author => "Amos";

    public Plugin(IConfigurationHandlerFactory configurationHandlerFactory)
    {
        ConfigurationHandler =
            configurationHandlerFactory.GetConfigurationHandler<ReservedClientsConfiguration>(
                "ReservedClientsSettings");
    }

    private readonly IConfigurationHandler<ReservedClientsConfiguration> ConfigurationHandler;
    public static List<ReservedClientsModel> ReservedClientsList;
    public const int ConfigurationVersion = 1;

    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        switch (gameEvent.Type)
        {
            case GameEvent.EventType.Join:
                var access = true;

                var clientGuid = ConfigurationHandler.Configuration().ReservedClients
                    .Find(x => x.Names.Contains(gameEvent.Origin.CleanedName));

                Console.WriteLine($"\nE-N: {gameEvent.Origin.CleanedName} - E-G: {gameEvent.Origin.GuidString}" +
                                  $"\nR-N: {clientGuid?.Names} - R-G: {clientGuid?.Guid}\nAccess: {access}\n");

                if (clientGuid != null)
                {
                    access = clientGuid.Guid.ToLower() == gameEvent.Origin.GuidString;
                }

                if (!access)
                {
                    gameEvent.Origin.Kick(ConfigurationHandler.Configuration().KickMessage,
                        Utilities.IW4MAdminClient(gameEvent.Owner));
                }

                break;
        }

        return Task.CompletedTask;
    }

    public async Task OnLoadAsync(IManager manager)
    {
        // Read/Write configuration
        await ConfigurationHandler.BuildAsync();
        if (ConfigurationHandler.Configuration() == null)
        {
            Console.WriteLine($"[{Name}] Configuration not found, creating.");
            ConfigurationHandler.Set(new ReservedClientsConfiguration());
            await ConfigurationHandler.Save();
        }

        // Duplicate checking
        var duplicateNames = ConfigurationHandler.Configuration().ReservedClients
            .SelectMany(x => x.Names)
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();

        var duplicateGuids = ConfigurationHandler.Configuration().ReservedClients
            .GroupBy(x => x.Guid.ToLower())
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();

        if (duplicateGuids.Any() || duplicateNames.Any())
        {
            if (duplicateNames.Any())
            {
                Console.WriteLine($"Duplicate Names: {string.Join(", ", duplicateNames.Select(x => x))}");
            }

            if (duplicateGuids.Any())
            {
                Console.WriteLine($"Duplicate GUIDs: {string.Join(", ", duplicateGuids)}");
            }

            Console.WriteLine("Duplicates found!\nRemove duplicates before starting!\nExiting...");
            Environment.Exit(1);
        }

        // Save new config if version is newer
        if (ConfigurationHandler.Configuration().Version < ConfigurationVersion)
        {
            Console.WriteLine($"[{Name}] Configuration version is outdated, updating.");
            ConfigurationHandler.Configuration().Version = ConfigurationVersion;
            await ConfigurationHandler.Save();
        }

        ReservedClientsList = ConfigurationHandler.Configuration().ReservedClients;
        
        // Load confirmation
        Console.WriteLine($"[{Name}] v{Version} by {Author} loaded");
    }

    public async Task OnUnloadAsync()
    {
        // Write back new entries
        ConfigurationHandler.Configuration().ReservedClients = ReservedClientsList;
        await ConfigurationHandler.Save();
        
        Console.WriteLine($"[{Name}] unloaded");
    }

    public Task OnTickAsync(Server server)
    {
        return Task.CompletedTask;
    }
}
