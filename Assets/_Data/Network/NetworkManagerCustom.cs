using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;

public class NetworkManagerCustom : SaiBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;
    public Button startHostButton;
    public Button startClientButton;
    public Button stopServerButton;
    public Button leaveServerButton;
    public TMP_Text statusText;

    private UnityTransport transport;

    protected override void Start()
    {
        base.Start();
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
        stopServerButton.onClick.AddListener(StopServer);
        leaveServerButton.onClick.AddListener(LeaveServer);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadIPInputField();
        this.LoadPortInputField();
        this.LoadBtnStartHost();
        this.LoadBtnStartClient();
        this.LoadBtnStopServer();
        this.LoadBtnStopClient();
        this.LoadStatusText();
    }

    protected virtual void LoadStatusText()
    {
        if (this.statusText != null) return;
        this.statusText = GameObject.Find("TxtServerStatusText").GetComponent<TMP_Text>();
        Debug.LogWarning(transform.name + ": LoadStatusText", gameObject);
    }

    protected virtual void LoadIPInputField()
    {
        if (this.ipInputField != null) return;
        this.ipInputField = GameObject.Find("InputServerIP").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadIPInputField", gameObject);
    }

    protected virtual void LoadPortInputField()
    {
        if (this.portInputField != null) return;
        this.portInputField = GameObject.Find("InputServerPort").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadPortInputField", gameObject);
    }

    protected virtual void LoadBtnStartHost()
    {
        if (this.startHostButton != null) return;
        this.startHostButton = GameObject.Find("BtnStartHost").GetComponent<Button>();
        Debug.LogWarning(transform.name + ": LoadBtnStartHost", gameObject);
    }

    protected virtual void LoadBtnStartClient()
    {
        if (this.startClientButton != null) return;
        this.startClientButton = GameObject.Find("BtnStartClient").GetComponent<Button>();
        Debug.LogWarning(transform.name + ": LoadBtnStartClient", gameObject);
    }

    protected virtual void LoadBtnStopServer()
    {
        if (this.stopServerButton != null) return;
        this.stopServerButton = GameObject.Find("BtnStopHost").GetComponent<Button>();
        Debug.LogWarning(transform.name + ": LoadBtnStopServer", gameObject);
    }

    protected virtual void LoadBtnStopClient()
    {
        if (this.leaveServerButton != null) return;
        this.leaveServerButton = GameObject.Find("BtnStopClient").GetComponent<Button>();
        Debug.LogWarning(transform.name + ": LoadBtnStopClient", gameObject);
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
