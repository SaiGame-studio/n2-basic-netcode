using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class UICanvasCtrl : SaiSingleton<UICanvasCtrl>
{
    [SerializeField] protected UIServerCreateMoving serverCreateMoving;
    public UIServerCreateMoving ServerCreateMoving { get { return serverCreateMoving; } }   

    [SerializeField] protected UIServerSelectionMoving serverSelectionMoving;
    public UIServerSelectionMoving ServerSelectionMoving { get { return serverSelectionMoving; } }

    [SerializeField] protected UIRoomMoving roomMoving;
    public UIRoomMoving RoomMoving { get { return roomMoving; } }   

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadUIServerCreateMoving();
        this.LoadUIServerSelectionMoving();
        this.LoadUIRoomMoving();
    }

    protected virtual void LoadUIServerCreateMoving()
    {
        if (this.serverCreateMoving != null) return;
        this.serverCreateMoving = FindAnyObjectByType<UIServerCreateMoving>();
        Debug.LogWarning(transform.name + ": LoadUIServerCreateMoving", gameObject);
    }

    protected virtual void LoadUIServerSelectionMoving()
    {
        if (this.serverSelectionMoving != null) return;
        this.serverSelectionMoving = FindAnyObjectByType<UIServerSelectionMoving>();
        Debug.LogWarning(transform.name + ": LoadUIServerSelectionMoving", gameObject);
    }

    protected virtual void LoadUIRoomMoving()
    {
        if (this.roomMoving != null) return;
        this.roomMoving = FindAnyObjectByType<UIRoomMoving>();
        Debug.LogWarning(transform.name + ": LoadUIRoomMoving", gameObject);
    }

}
