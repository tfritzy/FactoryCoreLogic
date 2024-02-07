using System;
using Newtonsoft.Json;

public class HostCreatingGame
{
    public const string MessageType = "HostCreatingGame";
    public string Type = MessageType;
    public Guid Id;

    public HostCreatingGame(Guid id)
    {
        Id = id;
    }
}