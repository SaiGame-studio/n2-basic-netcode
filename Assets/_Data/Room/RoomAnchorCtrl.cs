using System;
using Unity.Netcode;
using UnityEngine;

public class RoomAnchorCtrl : SaiBehaviour
{
    [SerializeField] protected RoomContainerCtrl roomContainer;
    [SerializeField] protected NetworkObject networkObject;
    public NetworkObject NetworkObject => networkObject;

    protected virtual void OnEnable()
    {
        Invoke(nameof(MoveRoomContainer), 1f);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNetworkObject();
    }

    protected virtual void LoadNetworkObject()
    {
        if (this.networkObject != null) return;
        this.networkObject = GetComponent<NetworkObject>();
        Debug.LogWarning(transform.name + ": LoadNetworkObject", gameObject);
    }

    protected virtual void MoveRoomContainer()
    {
        this.roomContainer = FindAnyObjectByType<RoomContainerCtrl>();
        roomContainer.transform.position = transform.position;
        Debug.Log("MoveRoomContainer: " + transform.position, gameObject);
    }
}
