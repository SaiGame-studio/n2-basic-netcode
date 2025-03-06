using Unity.Netcode;
using UnityEngine;

public class VisibilityByRoom : VisibilityByTarget
{
    protected override Transform GetTarget(ulong clientId)
    {
        Room room = RoomManager.Instance.FindRoomByClientId(clientId);
        if (room == null) return null;
        //Debug.Log("GetTarget: " + clientId + " - room: " + room.RoomName);
        if (room.roomAnchor == null) return null;
        return room.roomAnchor.transform;
    }
}