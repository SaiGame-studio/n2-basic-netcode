using Unity.Netcode;
using UnityEngine;

public class UIRoomMoving : UIMoving
{
    protected override void Start()
    {
        base.Start();
        NetworkEventManager.Instance.OnClientConnected.AddListener(this.OnClientConnected);
        RoomManager.OnLeftRoomAtClient += HandleOnLeftRoomAtClient;
    }

    protected override void LoadPointA()
    {
        if (this.start != Vector3.zero) return;
        this.start = new Vector3(-749, 262, 0);
        Debug.LogWarning(transform.name + ": LoadPointA", gameObject);
    }

    protected override void LoadPointB()
    {
        if (this.end != Vector3.zero) return;
        this.end = new Vector3(-450, 262, 0);
        Debug.LogWarning(transform.name + ": LoadPointB", gameObject);
    }

    protected virtual void OnClientConnected()
    {
        Debug.Log(transform.name + ": OnClientConnected", gameObject);
        this.Move();
    }

    protected virtual void HandleOnLeftRoomAtClient(ulong clientId, string roomName)
    {
        string message = $"Client {clientId} left roomName {roomName}";
        Debug.Log(transform.name + ": HandleOnClientLeftRoom - " + message, gameObject);
        ulong localClientID = NetworkManager.Singleton.LocalClientId;
        if (localClientID != clientId) return;
        this.Move();
    }
}
