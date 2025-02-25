using UnityEngine;

public class BtnShowServerSelection : ButttonAbstract
{
    public override void OnClick()
    {
        UICanvasCtrl.Instance.ServerSelectionMoving.Move();
    }
}
