using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;

public class NetworkManagerCustom : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;
    public Button startHostButton;
    public Button startClientButton;
    public Button stopServerButton;
    public Button leaveServerButton;
    public TMP_Text statusText;

    private UnityTransport transport;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
        stopServerButton.onClick.AddListener(StopServer);
        leaveServerButton.onClick.AddListener(LeaveServer);
    }

    void StartHost()
    {
        string port = string.IsNullOrEmpty(portInputField.text) ? "7777" : portInputField.text;

        transport.SetConnectionData("0.0.0.0", ushort.Parse(port));
        NetworkManager.Singleton.StartHost();
        statusText.text = "Server started on port " + port;
    }

    void StartClient()
    {
        string ip = string.IsNullOrEmpty(ipInputField.text) ? "127.0.0.1" : ipInputField.text;
        string port = string.IsNullOrEmpty(portInputField.text) ? "7777" : portInputField.text;

        transport.SetConnectionData(ip, ushort.Parse(port));
        NetworkManager.Singleton.StartClient();
        statusText.text = "Connecting to " + ip + ":" + port;
    }

    void StopServer()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            statusText.text = "Server stopped.";
        }
    }

    void LeaveServer()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            statusText.text = "Disconnected from server.";
        }
    }
}
