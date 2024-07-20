using Data.Models;

namespace Reservio;

public class ReservedClientsModel
{
    public required string Guid { get; set; }
    public required Reference.Game Game { get; set; }
    public required List<string> Names { get; set; }
}

public class ReservedClientsConfiguration
{
    public string KickMessage { get; set; } = "Name reserved. Change your name";

    public List<ReservedClientsModel> ReservedClients { get; set; } =
    [
        new ReservedClientsModel
        {
            Guid = "1995102c6e610ff7",
            Game = Reference.Game.IW4,
            Names = [":emp:ayymoss", "ayymoss", "amos"]
        },
        new ReservedClientsModel
        {
            Guid = "0001110001110001",
            Game = Reference.Game.UKN,
            Names = ["exampleothername1", "exampleothername2"]
        },
        new ReservedClientsModel
        {
            Guid = "0001110001110002",
            Game = Reference.Game.UKN,
            Names = ["anotherexamplename"]
        }
    ];
}
