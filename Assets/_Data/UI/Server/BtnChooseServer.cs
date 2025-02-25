using UnityEngine;

public class BtnChooseServer : ButttonAbstract
{
    public ServerInfo serverInfo;
    [SerializeField] protected int timeout = 3;
    [SerializeField] protected float checkInterval = 5f;
    [SerializeField] protected string statusText = "-";

    protected override void Start()
    {
        base.Start();
        this.serverInfo = ServerManager.Instance.GetServer(this.serverInfo.ServerName);
        this.UpdateUI();
    }

    public override void OnClick()
    {
        Debug.Log("Choose server: " + this.serverInfo.ServerName);
        ServerManager.Instance.ConnectToServer(this.serverInfo.ServerName);
    }

    private void UpdateUI()
    {
        if (statusText != null && this.serverInfo != null)
        {
            this.buttonText.text = this.serverInfo.ServerName + ": " + this.statusText;
        }
    }
}
