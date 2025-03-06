using UnityEngine;
using Unity.Netcode;

public abstract class VisibilityByTarget : NetworkVisibility
{
    [SerializeField] protected float visibilityDistance = 50f;
    [SerializeField] protected float targetDistance = Mathf.Infinity;

    protected abstract Transform GetTarget(ulong clientId);

    protected override bool CheckVisibility(ulong clientId)
    {
        if (!IsSpawned) return false;

        Transform target = this.GetTarget(clientId);
        if (target == null) return false;

        this.targetDistance = Vector3.Distance(target.position, transform.position);
        return this.targetDistance <= visibilityDistance;
    }

}