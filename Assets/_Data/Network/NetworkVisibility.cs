using UnityEngine;
using Unity.Netcode;

public abstract class NetworkVisibility : NetworkBehaviour
{
    [SerializeField] protected bool continuallyCheckVisibility = true;

    protected abstract bool CheckVisibility(ulong clientId);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // The server handles visibility checks and should subscribe when spawned locally on the server-side.
            NetworkObject.CheckObjectVisibility += CheckVisibility;
            // If we want to continually update, we don't need to check every frame but should check at least once per tick
            if (continuallyCheckVisibility)
            {
                NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
            }
        }
        base.OnNetworkSpawn();
    }

    protected void OnNetworkTick()
    {
        // If CheckObjectVisibility is enabled, check the distance to clients
        // once per network tick.
        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            var shouldBeVisibile = CheckVisibility(clientId);
            var isVisibile = NetworkObject.IsNetworkVisibleTo(clientId);
            if (shouldBeVisibile && !isVisibile)
            {
                // Note: This will invoke the CheckVisibility check again
                NetworkObject.NetworkShow(clientId);
            }
            else if (!shouldBeVisibile && isVisibile)
            {
                NetworkObject.NetworkHide(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility -= CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
        base.OnNetworkDespawn();
    }
}