using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using System.Linq;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] protected ulong localClientID;
    [SerializeField] protected GameObject anchorPrefab;

    [SerializeField] protected Room currentRoom;
    public Room CurrentRoom => currentRoom;

    [SerializeField] protected List<Room> rooms = new();
    [SerializeField] protected bool autoUpdateRooms = true;

    public string roomNameInput = "Room_1234";
    public int maxPlayersInput = 2;

    public static event Action<ulong, string> OnClientJoinedRoomOnServer;
    public static event Action<ulong, string> OnClientLeftRoomOnServer;

    public static event Action<ulong, string> OnJoinedRoomAtClient;
    public static event Action<ulong, string> OnLeftRoomAtClient;

    [System.Serializable]
    private class RoomListWrapper
    {
        public List<Room> Rooms;

        public RoomListWrapper(List<Room> rooms)
        {
            Rooms = rooms;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of RoomManager detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RequestRoomDataServerRpc();
        }

        this.localClientID = NetworkManager.LocalClientId;
    }

    public void CreateRoom()
    {
        CreateRoom(this.roomNameInput, this.maxPlayersInput);
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot create room. You are not connected to the server.");
            return;
        }

        Room room = this.FindRoomByClientId(this.localClientID);
        if (room != null)
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            CreateRoomOnServer(NetworkManager.Singleton.LocalClientId, roomName, maxPlayers);
        }
        else
        {
            CreateRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomName, maxPlayers);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(ulong clientId, string roomName, int maxPlayers)
    {
        CreateRoomOnServer(clientId, roomName, maxPlayers);
    }

    private void CreateRoomOnServer(ulong clientId, string roomName, int maxPlayers)
    {
        if (rooms.Exists(r => r.RoomName == roomName))
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' already exists.");
            return;
        }

        Room newRoom = new(roomName, maxPlayers);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        newRoom.RoomID = this.CreateRoomId(); ;
        RoomAnchorCtrl roomAnchor = this.CreateRoomAnchor(newRoom);
        this.MoveClientToAnchor(clientId, roomAnchor);

        if (clientId == this.localClientID) this.currentRoom = newRoom;
        if (autoUpdateRooms) UpdateClientsRoomList();

        OnClientJoinedRoomOnServer?.Invoke(clientId, roomName);
        SendClientJoinRoomClientRpc(roomName, clientId);
        Debug.Log($"[{clientId}] Created room: {roomName} (Max Players: {maxPlayers})", gameObject);
    }

    private int CreateRoomId()
    {
        List<int> roomIds = rooms.Select(r => r.RoomID).ToList();
        roomIds.Sort();
        for (int i = 1; i <= roomIds.Count; i++)
        {
            if (!roomIds.Contains(i))
            {
                return i;
            }
        }
        return roomIds.Count + 1;
    }


    protected virtual void MoveClientToAnchor(ulong clientId, Room room)
    {
        this.MoveClientToAnchor(clientId, room.roomAnchor);
    }

    protected virtual void MoveClientToAnchor(ulong clientId, RoomAnchorCtrl roomAnchor)
    {
        ClientCtrl playerCtrl = ClientManager.Instance.GetClientObject(clientId);
        playerCtrl.transform.position = roomAnchor.transform.position;
    }

    protected virtual RoomAnchorCtrl CreateRoomAnchor(Room newRoom)
    {
        GameObject anchorObj = Instantiate(this.anchorPrefab);
        RoomAnchorCtrl roomAnchor = anchorObj.GetComponent<RoomAnchorCtrl>();
        newRoom.roomAnchor = roomAnchor;
        Vector3 pos = Vector3.zero;
        pos.x = 100 * newRoom.RoomID;
        anchorObj.transform.position = pos;
        anchorObj.name = this.anchorPrefab.name + "_" + newRoom.RoomID;

        roomAnchor.NetworkObject.Spawn();
        return roomAnchor;
    }

    public void JoinRoom()
    {
        JoinRoom(this.roomNameInput);
    }

    public void JoinRoom(string roomName)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot join room. You are not connected to the server.");
            return;
        }

        Room room = this.FindRoomByClientId(localClientID);
        if (room != null)
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            room = this.GetRoomByName(roomName);
            if (room == null)
            {
                Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Room '{roomName}' does not exist.");
                return;
            }

            JoinSpecificRoom(NetworkManager.Singleton.LocalClientId, roomName);
        }
        else
        {
            JoinRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomName);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void JoinRoomServerRpc(ulong clientId, string roomName)
    {
        JoinSpecificRoom(clientId, roomName);
    }

    private void JoinSpecificRoom(ulong clientId, string roomName)
    {
        Room room = rooms.Find(r => r.RoomName == roomName);
        if (room == null)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' not found.");
            return;
        }

        if (room.Players.Count >= room.MaxPlayers)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' is full.");
            return;
        }

        room.Players.Add(clientId);
        if (autoUpdateRooms) UpdateClientsRoomList();
        if (this.currentRoom != null && room.RoomName == this.currentRoom.RoomName) this.currentRoom = room;

        this.MoveClientToAnchor(clientId, room);

        OnClientJoinedRoomOnServer?.Invoke(clientId, roomName);
        SendClientJoinRoomClientRpc(roomName, clientId);
        Debug.Log($"[{clientId}] Joined room: {roomName} (Players: {room.Players.Count}/{room.MaxPlayers})");
    }

    public void LeaveRoom()
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot leave room. You are not connected to the server.");
            return;
        }

        Room room = this.currentRoom;
        if (!room.Players.Contains(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] You are not in any room.");
            return;
        }

        if (IsServer)
        {
            RemovePlayerFromRoom(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            LeaveRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void LeaveRoomServerRpc(ulong clientId)
    {
        RemovePlayerFromRoom(clientId);
    }

    private void RemovePlayerFromRoom(ulong clientId)
    {
        Room room = this.FindRoomByClientId(clientId);
        if (room == null)
        {
            Debug.LogWarning($"[{clientId}] Not in any room.");
            return;
        }

        room.Players.Remove(clientId);
        Debug.Log($"[{clientId}] Left room: {room.RoomName}", gameObject);

        if (room.Players.Count == 0)
        {
            Debug.Log($"Room {room.RoomName} is now empty and will be removed.", gameObject);
            rooms.Remove(room);
        }

        if (autoUpdateRooms) UpdateClientsRoomList();

        OnClientLeftRoomOnServer?.Invoke(clientId, room.RoomName);
        SendLeftRoomClientRpc(room.RoomName, clientId);
        this.MoveObjectToZero(clientId);
    }

    protected virtual void MoveObjectToZero(ulong clientId)
    {
        if (!IsServer) return;

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) return;

        var networkObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObj.transform.position = Vector3.zero;
    }

    public void ShowRoomList()
    {
        if (IsServer)
        {
            SendRoomDataClientRpc(JsonUtility.ToJson(new RoomListWrapper(rooms)));
        }
        else
        {
            RequestRoomDataServerRpc();
        }
    }

    private void UpdateClientsRoomList()
    {
        string json = JsonUtility.ToJson(new RoomListWrapper(rooms));
        SendRoomDataClientRpc(json);
    }

    [ClientRpc]
    private void SendRoomDataClientRpc(string json, ClientRpcParams clientRpcParams = default)
    {
        RoomListWrapper wrapper = JsonUtility.FromJson<RoomListWrapper>(json);
        rooms = wrapper.Rooms;
        this.ClientUpdateCurrentRoom();
        Debug.Log("Updated room list from server.", gameObject);
    }

    protected virtual void ClientUpdateCurrentRoom()
    {
        Room room = this.FindRoomByClientId(this.localClientID);
        this.currentRoom = room;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRoomDataServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        string json = JsonUtility.ToJson(new RoomListWrapper(rooms));

        SendRoomDataClientRpc(json, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { senderClientId } }
        });
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public virtual Room FindRoomByClientId(ulong clientId)
    {
        foreach (Room room in rooms)
        {
            if (room.Players.Contains(clientId)) return room;
        }
        return null;
    }

    public virtual List<ulong> GetPlayersInRoom(string roomName)
    {
        Room room = this.GetRoomByName(roomName);
        return room.Players;
    }

    public Room GetRoomByName(string roomName)
    {
        return rooms.Find(room => room.RoomName == roomName);
    }

    [ClientRpc]
    private void SendClientJoinRoomClientRpc(string roomName, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"Client {clientId} join room {roomName}", gameObject);
        OnJoinedRoomAtClient?.Invoke(clientId, roomName);
    }

    [ClientRpc]
    private void SendLeftRoomClientRpc(string roomName, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"Client {clientId} left room {roomName}", gameObject);
        OnLeftRoomAtClient?.Invoke(clientId, roomName);
    }

    public virtual int GetMyIndex(ulong ownerId)
    {
        Room myRoom = this.FindRoomByClientId(ownerId);
        if (myRoom == null) return 0;
        return myRoom.Players.FindIndex(player => player == ownerId);
    }
}
