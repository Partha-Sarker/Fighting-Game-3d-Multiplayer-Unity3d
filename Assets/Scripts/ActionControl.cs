using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActionControl : NetworkBehaviour
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Command]
    public void CmdDamage(string id, int damage)
    {
        //player = Manager.GetPlayer(id);
        //Debug.Log("Inside Command method from: "+ transform.name + " and " + id + "(" + player.transform.name + ") got hit");
        RpcDamage(id, damage);
    }

    [ClientRpc]
    public void RpcDamage(string id, int damage)
    {
        player = Manager.GetPlayer(id);
        player.TakeDamage(damage);
        Debug.Log("Inside ClientRPC method from: " + transform.name + " and " + id + "(" +player.transform.name+") got hit");
    }

}
