using UnityEngine;
using UnityEngine.Networking;

public class FistAttack : Attack
{
    public ActionControl actionControl;
    public int damage = 5;
    public Animator animator;
    public NetworkAnimator networkAnimator;

    [Client]
    public override void DealDamage(string id, bool isBlocked)
    {
        if (!isBlocked)
        {
            if (Manager.isServer)
                actionControl.RpcDamage(id, damage);
            else
                actionControl.CmdDamage(id, damage);
        }
        else
        {
            animator.SetTrigger("Deflected");
            networkAnimator.SetTrigger("Deflected");
            if (Manager.isServer)
                actionControl.RpcDamage(id, 0);
            else
                actionControl.CmdDamage(id, 0);
        }
    }

    //public override void Deflect(string id)
    //{
    //    base.Deflect(id);
    //    animator.SetTrigger("Deflected");
    //    networkAnimator.SetTrigger("Deflected");
    //}
}
