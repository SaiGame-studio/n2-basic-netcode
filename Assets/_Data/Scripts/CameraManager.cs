using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{

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


    }

    private void HandleClientLeftRoom(ulong clientId, string roomName)
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }
}
