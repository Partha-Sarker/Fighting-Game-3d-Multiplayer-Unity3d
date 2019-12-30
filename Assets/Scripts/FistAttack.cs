using UnityEngine;
using UnityEngine.Networking;

public class FistAttack : Attack
{
    public ActionControl actionControl;
    public int damage = 5;

    
    public override void DealDamage(string id)
    {
        if (Manager.isServer)
            actionControl.RpcDamage(id, damage);
        else
            actionControl.CmdDamage(id, damage);
    }
}
