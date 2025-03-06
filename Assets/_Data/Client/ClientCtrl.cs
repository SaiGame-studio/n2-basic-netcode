using UnityEngine;

public class ClientCtrl : SaiBehaviour
{
    [SerializeField] protected VisibilityByRoom visibilityByTarget;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadVisibilityByTarget();
    }

    protected virtual void LoadVisibilityByTarget()
    {
        if (this.visibilityByTarget != null) return;
        this.visibilityByTarget = GetComponent<VisibilityByRoom>();
        Debug.LogWarning(transform.name + ": LoadVisibilityByTarget", gameObject);
    }
}
