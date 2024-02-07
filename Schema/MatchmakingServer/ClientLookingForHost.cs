using System;

public class ClientLookingForHost
{
    public const string MessageType = "ClientLookingForHost";
    public string Type = MessageType;
    public Guid Id;

    public ClientLookingForHost(Guid id)
    {
        Id = id;
    }
}