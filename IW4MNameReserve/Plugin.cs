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
            configurationHandlerFactory.GetConfigurationHandler<ReservedClientsListModel>($"ReservedClients");
    }

    private readonly IConfigurationHandler<ReservedClientsListModel> _configurationHandler;

    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        switch (gameEvent.Type)
        {
            case GameEvent.EventType.Join:
                var access = true;

                var clientGuid = _configurationHandler.Configuration().ReservedClients
                    .Find(x => x.Name == gameEvent.Origin.CleanedName);

                Console.WriteLine("1");
                Console.WriteLine(
                    $"\nE-N: {gameEvent.Origin.CleanedName} - E-G: {gameEvent.Origin.GuidString}\nR-N: {clientGuid?.Name} R-G: {clientGuid?.Guid}\nAccess: {access}\n");


                if (clientGuid != null)
                {
                    access = clientGuid.Guid == gameEvent.Origin.GuidString;

                    Console.WriteLine("2");
                    Console.WriteLine(
                        $"\nE-N: {gameEvent.Origin.CleanedName} - E-G: {gameEvent.Origin.GuidString}\nR-N: {clientGuid?.Name} R-G: {clientGuid?.Guid}\nAccess: {access}\n");
                }

                if (!access)
                {
                    Console.WriteLine("3");
                    Console.WriteLine(
                        $"\nE-N: {gameEvent.Origin.CleanedName} - E-G: {gameEvent.Origin.GuidString}\nR-N: {clientGuid?.Name} - R-G: {clientGuid?.Guid}\nAccess: {access}\n");

                    gameEvent.Origin.Kick("Name is reserved. Please change your name.",
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
            _configurationHandler.Set(new ReservedClientsListModel());
            await _configurationHandler.Save();
        }

        // Duplicate checking
        var duplicateNames = _configurationHandler.Configuration().ReservedClients
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();
        var duplicateGuid = _configurationHandler.Configuration().ReservedClients
            .GroupBy(x => x.Guid)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key).ToList();
        
        if (duplicateGuid.Any() || duplicateNames.Any())
        {
            if (duplicateNames.Any())
            {
                foreach (var name in duplicateNames)
                {
                    Console.WriteLine($"Duplicate Name: {name}");
                }
            }

            if (duplicateGuid.Any())
            {
                foreach (var guid in duplicateGuid)
                {
                    Console.WriteLine($"Duplicate GUID: {guid}");
                }
            }

            Console.WriteLine("Duplicates found!\nRemove duplicates before starting!\nExiting...");
            Environment.Exit(1);
        }

        // Debug
        Console.WriteLine("\nListing clients...");
        foreach (var client in _configurationHandler.Configuration().ReservedClients)
        {
            Console.WriteLine($"{client.Name}: {client.Guid}");
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
