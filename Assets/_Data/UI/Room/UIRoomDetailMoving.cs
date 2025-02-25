using Unity.Netcode;
using UnityEngine;

public class UIRoomDetailMoving : UIMoving
{
    protected override void Start()
    {
        base.Start();
        RoomManager.OnJoinedRoomAtClient += HandleClientJoinedRoomAtClient;
    }

    protected override void LoadPointA()
    {
        if (this.start != Vector3.zero) return;
        this.start = new Vector3(-597, 222, 0);
        Debug.LogWarning(transform.name + ": LoadPointA", gameObject);
    }

    protected override void LoadPointB()
    {
        if (this.end != Vector3.zero) return;
        this.end = new Vector3(-333, 222, 0);
        Debug.Log(transform.name + ": LoadPointB", gameObject);
    }

    protected virtual void HandleClientJoinedRoomAtClient(ulong clientId, string roomName)
    {
        string message = $"Client {clientId} join room {roomName}";
        Debug.Log(transform.name + ": HandleClientJoinedRoomAtClient - " + message, gameObject);
        ulong localClientID = NetworkManager.Singleton.LocalClientId;
        if (localClientID != clientId) return;
        this.Move();
    }
}
