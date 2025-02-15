using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField maxPlayersInputField;
    [SerializeField] private TMP_Text roomListText;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private Button showRoomListButton;

    private RoomManager roomManager;
    [SerializeField] protected bool autoRefresh = true;

    private void Start()
    {
        roomManager = FindAnyObjectByType<RoomManager>();

        if (roomManager == null)
        {
            Debug.LogError("RoomManager not found in the scene.");
            return;
        }

        createRoomButton.onClick.AddListener(CreateRoomFromUI);
        joinRoomButton.onClick.AddListener(JoinRoomFromUI);
        leaveRoomButton.onClick.AddListener(LeaveRoomFromUI);
        showRoomListButton.onClick.AddListener(ShowRoomListFromUI);

        InvokeRepeating(nameof(AutoRefreshRoomList), 1f, 2f); 
    }

    public void CreateRoomFromUI()
    {
        if (roomManager == null) return;

        string roomName = roomNameInputField.text;
        if (!int.TryParse(maxPlayersInputField.text, out int maxPlayers) || maxPlayers <= 0)
        {
            Debug.LogWarning("Invalid max players value.");
            return;
        }
        roomManager.CreateRoom(roomName, maxPlayers);
    }

    public void JoinRoomFromUI()
    {
        if (roomManager == null) return;

        string roomName = roomNameInputField.text;
        roomManager.JoinRoom(roomName);
    }

    public void LeaveRoomFromUI()
    {
        if (roomManager == null) return;

        roomManager.LeaveRoom();
    }

    public void UpdateRoomListUI()
    {
        if (roomManager == null) return;

        roomListText.text = "Room List:\n";
        foreach (var room in roomManager.GetRooms()) // Requires GetRooms() method in RoomManager
        {
            roomListText.text += $"{room.RoomID} ({room.Players.Count}/{room.MaxPlayers})\n";
        }
    }

    private void SetAutoRefresh(bool isEnabled)
    {
        autoRefresh = isEnabled;
    }

    private void AutoRefreshRoomList()
    {
        if (autoRefresh)
        {
            UpdateRoomListUI();
        }
    }

    public void ShowRoomListFromUI()
    {
        if (roomManager == null) return;

        roomManager.ShowRoomList();
    }
}
