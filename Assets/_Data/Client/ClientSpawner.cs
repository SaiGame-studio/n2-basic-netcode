using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ClientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject clientPrefab;
    private Dictionary<ulong, GameObject> spawnedClients = new();

    private void OnEnable()
    {
        RoomManager.OnClientJoinedRoomOnServer += HandleClientJoinedRoom;
        RoomManager.OnClientLeftRoomOnServer += HandleClientLeftRoom;
    }

    private void OnDisable()
    {
        RoomManager.OnClientJoinedRoomOnServer -= HandleClientJoinedRoom;
        RoomManager.OnClientLeftRoomOnServer -= HandleClientLeftRoom;
    }

    private void HandleClientJoinedRoom(ulong clientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (clientPrefab == null)
        {
            Debug.LogError("PlayerPrefab is not assigned in PlayerSpawner!");
            return;
        }

        if (spawnedClients.ContainsKey(clientId))
        {
            Debug.LogWarning($"Player for client {clientId} already exists.");
            return;
        }

        GameObject playerInstance = Instantiate(clientPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        spawnedClients[clientId] = playerInstance;
        Debug.Log($"Spawned player for client {clientId} in room {roomName}");

        SendExistingPlayersToClient(clientId, roomName);
    }

    private void HandleClientLeftRoom(ulong clientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (spawnedClients.TryGetValue(clientId, out GameObject player))
        {
            Destroy(player);
            spawnedClients.Remove(clientId);
            Debug.Log($"Destroyed player for client {clientId} after leaving room {roomName}");
        }
    }

    private void SendExistingPlayersToClient(ulong newClientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        List<ulong> playersInRoom = RoomManager.Instance.GetPlayersInRoom(roomName);
        foreach (ulong existingClientId in playersInRoom)
        {
            if (existingClientId == newClientId) continue; 

            RequestSpawnExistingPlayerClientRpc(existingClientId, newClientId);
        }
    }

    [ClientRpc]
    private void RequestSpawnExistingPlayerClientRpc(ulong existingClientId, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        if (!spawnedClients.ContainsKey(existingClientId))
        {
            Debug.LogWarning($"Trying to spawn existing player {existingClientId} but it doesn't exist on the client.");
            return;
        }

        GameObject playerInstance = Instantiate(clientPrefab);
        playerInstance.GetComponent<NetworkObject>().Spawn();
        spawnedClients[existingClientId] = playerInstance;
        Debug.Log($"Spawned existing player {existingClientId} for new client {targetClientId}");
    }
}
