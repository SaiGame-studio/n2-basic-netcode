using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private Dictionary<ulong, GameObject> spawnedPlayers = new();

    private void OnEnable()
    {
        RoomManager.OnClientJoinedRoom += HandleClientJoinedRoom;
        RoomManager.OnClientLeftRoom += HandleClientLeftRoom;
    }

    private void OnDisable()
    {
        RoomManager.OnClientJoinedRoom -= HandleClientJoinedRoom;
        RoomManager.OnClientLeftRoom -= HandleClientLeftRoom;
    }

    private void HandleClientJoinedRoom(ulong clientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is not assigned in PlayerSpawner!");
            return;
        }

        if (spawnedPlayers.ContainsKey(clientId))
        {
            Debug.LogWarning($"Player for client {clientId} already exists.");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        spawnedPlayers[clientId] = playerInstance;
        Debug.Log($"Spawned player for client {clientId} in room {roomName}");

        SendExistingPlayersToClient(clientId, roomName);
    }

    private void HandleClientLeftRoom(ulong clientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (spawnedPlayers.TryGetValue(clientId, out GameObject player))
        {
            Destroy(player);
            spawnedPlayers.Remove(clientId);
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

        if (!spawnedPlayers.ContainsKey(existingClientId))
        {
            Debug.LogWarning($"Trying to spawn existing player {existingClientId} but it doesn't exist on the client.");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab);
        playerInstance.GetComponent<NetworkObject>().Spawn();
        spawnedPlayers[existingClientId] = playerInstance;
        Debug.Log($"Spawned existing player {existingClientId} for new client {targetClientId}");
    }
}
