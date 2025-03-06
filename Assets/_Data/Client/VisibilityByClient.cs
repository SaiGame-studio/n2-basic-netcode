using Unity.Netcode;
using UnityEngine;

public class VisibilityByClient : VisibilityByTarget
{
    protected override Transform GetTarget(ulong clientId)
    {
        if (NetworkManager.ConnectedClients[clientId].PlayerObject == null) return null;
        return NetworkManager.ConnectedClients[clientId].PlayerObject.transform;
    }
}