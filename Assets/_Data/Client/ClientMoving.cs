using Unity.Netcode;
using UnityEngine;

public class ClientMoving : NetworkBehaviour
{
    [SerializeField] protected float moveSpeed = 25f;
    [SerializeField] protected Transform target;
    [SerializeField] protected float maxDistance = 43f;

    void Update()
    {
        if (!IsOwner || target == null) return;

        Vector3 targetPosition = InputManager.Instance.GetMouseWorldPosition();
        targetPosition.z = 0;

        if (Vector3.Distance(targetPosition, target.position) > maxDistance)
        {
            targetPosition = target.position + (targetPosition - target.position).normalized * maxDistance;
        }

        SendMovementToServerServerRpc(targetPosition);
    }

    [ServerRpc]
    private void SendMovementToServerServerRpc(Vector3 targetPosition, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out var client))
        {
            if (client.PlayerObject.TryGetComponent(out ClientMoving player))
            {
                player.MoveTowardsTarget(targetPosition);
            }
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
