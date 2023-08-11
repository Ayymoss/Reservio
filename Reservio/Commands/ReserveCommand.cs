using Data.Models.Client;
using SharedLibraryCore;
using SharedLibraryCore.Commands;
using SharedLibraryCore.Configuration;
using SharedLibraryCore.Interfaces;

namespace Reservio.Commands;

public class ReserveCommand : Command
{
    private readonly ReservedClientsConfiguration _reservedConfig;
    private readonly IConfigurationHandlerV2<ReservedClientsConfiguration> _configurationHandler;

    public ReserveCommand(CommandConfiguration config, ITranslationLookup translationLookup,
        ReservedClientsConfiguration reservedConfig, IConfigurationHandlerV2<ReservedClientsConfiguration> configurationHandler) : base(config,
        translationLookup)
    {
        _reservedConfig = reservedConfig;
        _configurationHandler = configurationHandler;

        Name = "reserve";
        Description = "reserve the target players name";
        Alias = "rsrv";
        Permission = EFClient.Permission.SeniorAdmin;
        RequiresTarget = true;
        Arguments = new[]
        {
            new CommandArgument
            {
                Name = "player",
                Required = true
            }
        };
    }

    public override Task ExecuteAsync(GameEvent gameEvent)
    {
        var clientGuid = _reservedConfig.ReservedClients
            .FirstOrDefault(x => x.Guid.Contains(gameEvent.Target.GuidString));

        if (clientGuid == null)
        {
            clientGuid = new ReservedClientsModel
            {
                Guid = gameEvent.Target.GuidString,
                Game = gameEvent.Target.GameName,
                Names = new List<string>()
            };
            _reservedConfig.ReservedClients.Add(clientGuid);
        }

        var cleanedName = gameEvent.Target.CleanedName.ToLower();

        if (!clientGuid.Names.Contains(cleanedName))
        {
            clientGuid.Names.Add(cleanedName);
            gameEvent.Origin.Tell($"Created/updated reservation for: {gameEvent.Target.CleanedName}");
        }
        else
        {
            gameEvent.Origin.Tell($"{gameEvent.Target.CleanedName} is already reserved");
        }

        _configurationHandler.Set();
        return Task.CompletedTask;
    }
}
