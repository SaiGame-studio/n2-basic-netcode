using System.Collections.Generic;

[System.Serializable]
public class Room
{
    public int RoomID;
    public string RoomName;
    public int MaxPlayers;
    public RoomAnchorCtrl roomAnchor;
    public List<ulong> Players;

    public Room(string id, int maxPlayers)
    {
        RoomName = id;
        MaxPlayers = maxPlayers;
        Players = new List<ulong>();
    }
}