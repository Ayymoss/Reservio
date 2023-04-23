using Data.Models;

namespace Reservio;

public class ReservedClientsModel
{
    public string Guid { get; set; }
    public Reference.Game Game { get; set; }
    public List<string> Names { get; set; }
}

public class ReservedClientsConfiguration
{
    public string KickMessage { get; set; } = "Name reserved. Change your name";

    public List<ReservedClientsModel> ReservedClients { get; set; } = new()
    {
        new ReservedClientsModel
        {
            Guid = "1995102c6e610ff7",
            Game = Reference.Game.IW4,
            Names = new List<string> {":emp:ayymoss", "ayymoss", "amos"}
        },
        new ReservedClientsModel
        {
            Guid = "0001110001110001",
            Game = Reference.Game.UKN,
            Names = new List<string> {"exampleothername1", "exampleothername2"}
        },
        new ReservedClientsModel
        {
            Guid = "0001110001110002",
            Game = Reference.Game.UKN,
            Names = new List<string> {"anotherexamplename"}
        }
    };
}
