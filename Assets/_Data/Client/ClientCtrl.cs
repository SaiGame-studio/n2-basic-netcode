using Unity.Netcode;
using UnityEngine;

public class ClientCtrl : SaiBehaviour
{
    [SerializeField] protected NetworkObject networkObject;
    public NetworkObject NetworkObject => networkObject;

    [SerializeField] protected VisibilityByRoom visibilityByTarget;
    [SerializeField] protected ClientMoving clientMoving;
    public ClientMoving ClientMoving => clientMoving;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadVisibilityByTarget();
        this.LoadClientMoving();
        this.LoadNetworkObject();
    }

    protected virtual void LoadNetworkObject()
    {
        if (this.networkObject != null) return;
        this.networkObject = GetComponent<NetworkObject>();
        Debug.LogWarning(transform.name + ": LoadNetworkObject", gameObject);
    }

    protected virtual void LoadVisibilityByTarget()
    {
        if (this.visibilityByTarget != null) return;
        this.visibilityByTarget = GetComponent<VisibilityByRoom>();
        Debug.LogWarning(transform.name + ": LoadVisibilityByTarget", gameObject);
    }

    protected virtual void LoadClientMoving()
    {
        if (this.clientMoving != null) return;
        this.clientMoving = GetComponent<ClientMoving>();
        Debug.LogWarning(transform.name + ": LoadClientMoving", gameObject);
    }

    public virtual ulong OwnerId()
    {
        return this.networkObject.OwnerClientId;
    }
}
