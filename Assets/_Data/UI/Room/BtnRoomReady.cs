using UnityEngine;

public class BtnRoomReady : ButttonAbstract
{
    public override void OnClick()
    {
        RoomAnchorCtrl roomAnchorCtrl = FindAnyObjectByType<RoomAnchorCtrl>();
        ClientCtrl myClientCtrl = ClientManager.Instance.GetMyClient();
        myClientCtrl.ClientMoving.SetTarget(roomAnchorCtrl.transform);
        Debug.Log("Player Ready");
    }
}
