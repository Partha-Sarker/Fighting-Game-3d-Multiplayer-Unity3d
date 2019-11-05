using UnityEngine;
using UnityEngine.Networking;

public class SwordAttack : Attack
{
    public ActionControl actionControl;
    public int damage = 7;

    [Client]
    public override void DealDamage(string id)
    {
        actionControl.CmdDamage(id, damage);
    }
}
