using UnityEngine;

public class UIServerManager : SaiSingleton<UIServerManager>
{
    [SerializeField] protected BtnChooseServer btnPrefab;

    protected override void Start()
    {
        base.Start();
        this.ShowServerList();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadButtonPrefab();
    }

    protected virtual void LoadButtonPrefab()
    {
        if(this.btnPrefab != null) return;
        this.btnPrefab = GetComponentInChildren<BtnChooseServer>();
        Debug.LogWarning(transform.name + ": LoadButtonPrefab", gameObject);
    }

    protected virtual void ShowServerList()
    {
        foreach(ServerInfo serverInfo in ServerManager.Instance.Servers)
        {
            BtnChooseServer newButton = Instantiate(this.btnPrefab);
            newButton.transform.SetParent(this.btnPrefab.transform.parent);
            newButton.serverInfo = serverInfo;
        }

        this.btnPrefab.SetActive(false);
    }
}
