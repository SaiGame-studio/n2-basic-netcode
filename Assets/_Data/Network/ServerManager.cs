using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerManager : SaiSingleton<ServerManager>
{
    
    [SerializeField] protected List<ServerInfo> servers = new();

    public IReadOnlyList<ServerInfo> Servers => servers.AsReadOnly();

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadServerList();
    }

    protected virtual void LoadServerList()
    {
        if (this.servers.Count > 0) return;
        this.AddServer("Server 1", "127.0.0.1", 1111);
        this.AddServer("Server 2", "127.0.0.1", 2222);
        this.AddServer("Server 3", "127.0.0.1", 3333);
        this.AddServer("Server 4", "127.0.0.1", 4444);
        this.AddServer("Server 5", "127.0.0.1", 5555);
        this.AddServer("Server 6", "127.0.0.1", 6666);
        this.AddServer("Server 7", "127.0.0.1", 7777);
        this.AddServer("Server 8", "127.0.0.1", 8888);
        this.AddServer("Server 9", "127.0.0.1", 9999);
        Debug.LogWarning(transform.name + ": LoadServerList", gameObject);
    }

    public void AddServer(string name, string ip, int port)
    {
        if (servers.Exists(s => s.IPAddress == ip && s.Port == port))
        {
            Debug.LogWarning($"Server {name} ({ip}:{port}) already exists.");
            return;
        }

        servers.Add(new ServerInfo(name, ip, port));
        Debug.Log($"Added server: {name} ({ip}:{port})");
    }

    public void RemoveServer(string name)
    {
        ServerInfo serverToRemove = servers.Find(s => s.ServerName == name);
        if (serverToRemove != null)
        {
            servers.Remove(serverToRemove);
            Debug.Log($"Removed server: {name}");
        }
        else
        {
            Debug.LogWarning($"Server {name} not found.");
        }
    }

    public void ConnectToServer(string name)
    {
        ServerInfo server = servers.Find(s => s.ServerName == name);
        if (server == null)
        {
            Debug.LogWarning($"Server {name} not found.");
            return;
        }

        Debug.Log($"Connecting to server: {server.ServerName} ({server.IPAddress}:{server.Port})");

        // Set the IP address and port of NetworkManager before connecting
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = server.IPAddress;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)server.Port;

        // Start client connection
        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogError("Failed to connect to the server.");
        }
    }

    public virtual ServerInfo GetServer(string name)
    {
        return servers.Find(server => server.ServerName == name);
    }

}
