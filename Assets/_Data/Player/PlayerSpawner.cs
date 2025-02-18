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
}
