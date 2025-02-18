using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class RoomManager : NetworkBehaviour
{

    [SerializeField] protected ulong clientId;
    [SerializeField] protected Room currentRoom;
    [SerializeField] protected List<Room> rooms = new();
    protected Dictionary<ulong, Room> playerRoomMap = new();

    public string roomNameInput = "Room_1234";
    public int maxPlayersInput = 2;

    [SerializeField] protected bool autoUpdateRooms = true;

    public static event Action<ulong, string> OnClientJoinedRoom;
    public static event Action<ulong, string> OnClientLeftRoom;

    [System.Serializable]
    public class Room
    {
        public string RoomID;
        public int MaxPlayers;
        public List<ulong> Players;

        public Room(string id, int maxPlayers)
        {
            RoomID = id;
            MaxPlayers = maxPlayers;
            Players = new List<ulong>();
        }
    }

    [System.Serializable]
    private class RoomListWrapper
    {
        public List<Room> Rooms;

        public RoomListWrapper(List<Room> rooms)
        {
            Rooms = rooms;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RequestRoomDataServerRpc();
        }

        this.clientId = NetworkManager.LocalClientId;
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

        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
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
        if (rooms.Exists(r => r.RoomID == roomName))
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' already exists.");
            return;
        }

        Room newRoom = new(roomName, maxPlayers);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        playerRoomMap[clientId] = newRoom;
        if(clientId == this.clientId) this.currentRoom = newRoom;
        if (autoUpdateRooms) UpdateClientsRoomList();

        OnClientJoinedRoom?.Invoke(clientId, roomName);
        Debug.Log($"[{clientId}] Created room: {roomName} (Max Players: {maxPlayers})");
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

        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            Room room = rooms.Find(r => r.RoomID == roomName);
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
        Room room = rooms.Find(r => r.RoomID == roomName);
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
        playerRoomMap[clientId] = room;
        if (autoUpdateRooms) UpdateClientsRoomList();
        if (room.RoomID == this.currentRoom.RoomID) this.currentRoom = room;

        OnClientJoinedRoom?.Invoke(clientId, roomName);
        Debug.Log($"[{clientId}] Joined room: {roomName} (Players: {room.Players.Count}/{room.MaxPlayers})");
    }

    public void LeaveRoom()
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot leave room. You are not connected to the server.");
            return;
        }

        if (!playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
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
        if (!playerRoomMap.TryGetValue(clientId, out Room room))
        {
            Debug.LogWarning($"[{clientId}] Not in any room.");
            return;
        }

        room.Players.Remove(clientId);
        playerRoomMap.Remove(clientId);
        Debug.Log($"[{clientId}] Left room: {room.RoomID}");

        if (room.Players.Count == 0)
        {
            Debug.Log($"Room {room.RoomID} is now empty and will be removed.");
            rooms.Remove(room);
        }

        if (autoUpdateRooms) UpdateClientsRoomList();

        OnClientLeftRoom?.Invoke(clientId, room.RoomID);
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
        Debug.Log("Updated room list from server.");
    }

    protected virtual void ClientUpdateCurrentRoom()
    {
        Room room = this.GetRoomClientBelongTo(this.clientId);
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

    protected virtual Room GetRoomClientBelongTo(ulong clientId)
    {
        foreach (Room room in rooms)
        {
            if (room.Players.Contains(clientId)) return room;
        }
        return null;
    }
}
