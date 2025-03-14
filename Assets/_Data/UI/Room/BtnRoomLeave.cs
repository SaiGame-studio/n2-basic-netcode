
public class BtnRoomLeave : ButttonAbstract
{
    public override void OnClick()
    {
        RoomManager.Instance.LeaveRoom();
    }
}
