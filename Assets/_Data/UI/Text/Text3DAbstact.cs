using TMPro;
using UnityEngine;

public abstract class Text3DAbstact : SaiBehaviour
{
    [SerializeField] protected TextMeshPro textPro;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTextPro();
    }

    protected virtual void LoadTextPro()
    {
        if (this.textPro != null) return;
        this.textPro = GetComponent<TextMeshPro>();
        Debug.Log(transform.name + ": LoadTextPro", gameObject);
    }
}
