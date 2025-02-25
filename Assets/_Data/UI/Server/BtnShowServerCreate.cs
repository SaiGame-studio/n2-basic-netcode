using UnityEngine;

public class BtnShowServerCreate : ButttonAbstract
{
    public override void OnClick()
    {
        UICanvasCtrl.Instance.ServerCreateMoving.Move();
    }
}
