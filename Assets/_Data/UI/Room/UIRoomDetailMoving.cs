using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomDetailMoving : UIMoving
{
    [SerializeField] protected TextMeshProUGUI LeaveButtonText;

    protected override void Start()
    {
        base.Start();
        RoomManager.OnJoinedRoomAtClient += HandleClientJoinedRoomAtClient;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLeaveButton();
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
        Room currentRoom = RoomManager.Instance.CurrentRoom;
        this.LeaveButtonText.text = "Leave " + currentRoom.RoomName;
    }

    protected virtual void LoadLeaveButton()
    {
        if (this.LeaveButtonText != null) return;
        this.LeaveButtonText = transform.Find("BtnRoomLeave").Find("Text").GetComponent<TextMeshProUGUI>();
        Debug.LogWarning(transform.name + ": LoadLeaveButton", gameObject);
    }
}
