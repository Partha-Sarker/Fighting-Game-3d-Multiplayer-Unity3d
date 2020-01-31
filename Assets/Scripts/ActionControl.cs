using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

public class ActionControl : NetworkBehaviour
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //        ResetALlPlayers();
    //}

    public void Damage(string id, int damage, string type)
    {
        if (isServer)
            RpcDamage(id, damage, type);
        else
            CmdDamage(id, damage, type);
    }

    public void Block(string id, string type)
    {
        if (Manager.isServer)
            RpcBlock(id, type);
        else
            CmdBlock(id, type);
    }

    public void Shoot(string id)
    {
        if (Manager.isServer)
            RpcShoot(id);
        else
            CmdShoot(id);
    }

    public void ResetALlPlayers()
    {
        List<string> allPlayerId = Manager.GetAllPlayer();

        if (Manager.isServer)
        {
            foreach (string id in allPlayerId)
                RpcResetPlayer(id);
        }
        else
        {
            foreach (string id in allPlayerId)
                CmdResetPlayer(id);
        }
    }

    public void SetOwnRatio(string id, string ratio)
    {
        if (Manager.isServer)
            RpcSetOwnRatio(id, ratio);
        else
            CmdSetOwnRatio(id, ratio);
    }


    [Command]
    public void CmdDamage(string id, int damage, string type)
    {
        RpcDamage(id, damage, type);
    }

    [ClientRpc]
    public void RpcDamage(string id, int damage, string type)
    {
        player = Manager.GetPlayer(id);
        player.TakeDamage(damage, type);
    }
    
    [Command]
    public void CmdBlock(string id, string type)
    {
        RpcBlock(id, type);
    }

    [ClientRpc]
    public void RpcBlock(string id, string type)
    {
        player = Manager.GetPlayer(id);
        player.BlockAttack(type);
    }
    
    [Command]
    public void CmdSetOwnRatio(string id, string ratio)
    {
        RpcSetOwnRatio(id, ratio);
    }

    [ClientRpc]
    public void RpcSetOwnRatio(string id, string ratio)
    {
        player = Manager.GetPlayer(id);
        player.SetMyRatio(ratio);
    }

    [Command]
    public void CmdShoot(string id)
    {
        RpcShoot(id);
    }

    [ClientRpc]
    public void RpcShoot(string id)
    {
        player = Manager.GetPlayer(id);
        player.ShootFireball();
    }

    [Command]
    public void CmdResetPlayer(string id)
    {
        RpcResetPlayer(id);
    }

    [ClientRpc]
    public void RpcResetPlayer(string id)
    {
        player = Manager.GetPlayer(id);
        player.ResetAll();
    }

}
