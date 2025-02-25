using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkEventManager : SaiSingleton<NetworkEventManager>
{
    // Custom events
    public UnityEvent OnHostStarted;       // Triggered when Host starts successfully
    public UnityEvent OnServerStarted;     // Triggered when Server starts successfully
    public UnityEvent OnClientConnected;   // Triggered when Client connects successfully
    public UnityEvent OnClientJoined;      // Triggered when a new Client joins the Server
    public UnityEvent OnServerDisconnect;

    protected override void Start()
    {
        base.Start();
        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when the object is destroyed
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    // Handle Server started event
    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host started successfully!", gameObject);
            OnHostStarted?.Invoke(); // Trigger Host success event
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server started successfully!");
            OnServerStarted?.Invoke(); // Trigger Server success event
        }
    }

    // Handle Client connected event
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Successfully connected to the server!", gameObject);
            OnClientConnected?.Invoke(); // Trigger Client connection success event
        }
        else
        {
            Debug.Log($"New client joined with ID: {clientId}", gameObject);
            OnClientJoined?.Invoke(); // Trigger new Client joined event
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Disconnected from the server!");
            OnServerDisconnect?.Invoke();
        }
        else
        {
            Debug.Log($"Client with ID {clientId} disconnected from the server!");
        }
    }
}