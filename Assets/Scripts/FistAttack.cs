using UnityEngine;
using UnityEngine.Networking;

public class FistAttack : Attack
{
    public ActionControl actionControl;
    public int damage = 5;

    [Client]
    public override void DealDamage(string id)
    {
        actionControl.CmdDamage(id, damage);
    }
}
