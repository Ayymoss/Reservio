using Data.Models.Client;
using SharedLibraryCore;
using SharedLibraryCore.Commands;
using SharedLibraryCore.Configuration;
using SharedLibraryCore.Interfaces;

namespace IW4MNameReserve.Commands;

public class ReserveCommand : Command
{
    public ReserveCommand(CommandConfiguration config, ITranslationLookup translationLookup) : base(config,
        translationLookup)
    {
        Name = "reserve";
        Description = "reserve the target players name";
        Alias = "res";
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
        var clientGuid = Plugin.ReservedClientsList
            .Find(x => x.Guid.Contains(gameEvent.Target.GuidString));

        if (clientGuid == null)
        {
            Plugin.ReservedClientsList.Add(new ReservedClientsModel
            {
                Guid = gameEvent.Target.GuidString,
                Names = new List<string> {gameEvent.Target.CleanedName.ToLower()}
            });
            gameEvent.Origin.Tell($"Reserved: {gameEvent.Target.CleanedName}");
        }
        else
        {
            if (!Plugin.ReservedClientsList
                    .First(x => x.Guid == gameEvent.Target.GuidString).Names
                    .Contains(gameEvent.Target.CleanedName.ToLower()))
            {
                Plugin.ReservedClientsList
                    .First(x => x.Guid == gameEvent.Target.GuidString).Names
                    .Add(gameEvent.Target.CleanedName.ToLower());
                gameEvent.Origin.Tell($"Updated reservation for: {gameEvent.Target.CleanedName}");
            }
            else
            {
                gameEvent.Origin.Tell($"{gameEvent.Target.CleanedName} is already reserved");
            }
        }

        return Task.CompletedTask;
    }
}
