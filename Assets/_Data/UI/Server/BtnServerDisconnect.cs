using Unity.Netcode;
using UnityEngine;

public class BtnServerDisconnect : ButttonAbstract
{
    public override void OnClick()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
