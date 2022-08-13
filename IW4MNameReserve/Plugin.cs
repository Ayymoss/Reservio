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
        _configurationHandler =
            configurationHandlerFactory.GetConfigurationHandler<ReservedClientsConfiguration>(
                $"ReservedClientsSettings");
    }

    private readonly IConfigurationHandler<ReservedClientsConfiguration> _configurationHandler;
    public const int ConfigurationVersion = 1;

    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        switch (gameEvent.Type)
        {
            case GameEvent.EventType.Join:
                var access = true;

                var clientGuid = _configurationHandler.Configuration().ReservedClients
                    .Find(x => x.Names.Contains(gameEvent.Origin.CleanedName));

                Console.WriteLine($"\nE-N: {gameEvent.Origin.CleanedName} - E-G: {gameEvent.Origin.GuidString}" +
                                  $"\nR-N: {clientGuid?.Names} - R-G: {clientGuid?.Guid}\nAccess: {access}\n");

                if (clientGuid != null)
                {
                    access = clientGuid.Guid == gameEvent.Origin.GuidString;
                }

                if (!access)
                {
                    gameEvent.Origin.Kick(_configurationHandler.Configuration().KickMessage,
                        Utilities.IW4MAdminClient(gameEvent.Owner));
                }

                break;
        }

        return Task.CompletedTask;
    }

    public async Task OnLoadAsync(IManager manager)
    {
        // Read/Write configuration
        await _configurationHandler.BuildAsync();
        if (_configurationHandler.Configuration() == null)
        {
            Console.WriteLine($"[{Name}] Configuration not found, creating.");
            _configurationHandler.Set(new ReservedClientsConfiguration());
            await _configurationHandler.Save();
        }

        // Duplicate checking
        var duplicateNames = _configurationHandler.Configuration().ReservedClients
            .GroupBy(x => x.Names)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();
        var duplicateGuids = _configurationHandler.Configuration().ReservedClients
            .GroupBy(x => x.Guid)
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
        if (_configurationHandler.Configuration().Version < ConfigurationVersion)
        {
            Console.WriteLine($"[{Name}] Configuration version is outdated, updating.");
            _configurationHandler.Configuration().Version = ConfigurationVersion;
            await _configurationHandler.Save();
        }

        // Debug
        Console.WriteLine("\nListing clients...");
        foreach (var client in _configurationHandler.Configuration().ReservedClients)
        {
            Console.WriteLine($"{client.Guid}: {string.Join(", ", client.Names)}");
        }

        Console.WriteLine("Listing clients end\n");

        // Load confirmation
        Console.WriteLine($"[{Name}] v{Version} by {Author} loaded");
    }

    public Task OnUnloadAsync()
    {
        Console.WriteLine($"[{Name}] unloaded");
        return Task.CompletedTask;
    }

    public Task OnTickAsync(Server server)
    {
        return Task.CompletedTask;
    }
}
