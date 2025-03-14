using UnityEngine;

public class TxtRoomIndex : Text3DUpdate
{
    [SerializeField] protected ClientCtrl clientCtrl;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadClientCtrl();
    }

    protected virtual void LoadClientCtrl()
    {
        if (this.clientCtrl != null) return;
        this.clientCtrl = GetComponentInParent<ClientCtrl>();
        Debug.LogWarning(transform.name + ": LoadClientCtrl", gameObject);
    }

    protected override void ShowingText()
    {
        this.textPro.text = RoomManager.Instance.GetMyIndex(this.clientCtrl.OwnerId()).ToString();
    }
}
