using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;
    public Collider[] collidersToDisable;

    public string player_id;
    public string net_id;

    Camera sceneCamera;

    void Start()
    {
        player_id = GetComponent<NetworkIdentity>().netId.ToString();

        if (!isLocalPlayer)
        {
            this.gameObject.name = "oponent";
            GameObject localPlayer = GameObject.Find("local player");
            if(localPlayer != null)
            {
                localPlayer.GetComponent<PlayerMovement>().oponent = this.transform;
                print("oponent set for local player");
            }

            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            for (int i = 0; i < collidersToDisable.Length; i++)
            {
                collidersToDisable[i].enabled = false;
            }
        }
        else
        {
            this.gameObject.name = "local player";
            if (isServer)
                Manager.isServer = true;
            GameObject oponent = GameObject.Find("oponent");
            if(this.GetComponent<PlayerMovement>().oponent == null && oponent != null)
            {
                this.GetComponent<PlayerMovement>().oponent = oponent.transform;
                print("oponent set for local player");
            }

            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        net_id = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        Manager.RegisterPlayer(net_id, player);
    }


    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        Manager.UnRegisterPlayer(net_id);
    }

}
