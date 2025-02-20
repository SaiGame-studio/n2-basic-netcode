using UnityEngine;
using Unity.Netcode;

public class ClientManager : SaiSingleton<ClientManager>
{
    public virtual ClientCtrl GetClientObject(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            GameObject playerObject = client.PlayerObject.gameObject;
            ClientCtrl playerCtrl = playerObject.GetComponent<ClientCtrl>();
            Debug.Log($"Found PlayerObject for Client {clientId}: {playerObject.name}");
            return playerCtrl;
        }
        Debug.LogWarning($"PlayerObject for Client {clientId} not found.");

        return null;
    }
}