using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;

    Camera sceneCamera;

    void Start()
    {

        // Disable components that should only be
        // active on the player that we control
        if (!isLocalPlayer)
        {
            this.gameObject.name = "oponent";
            GameObject localPlayer = GameObject.Find("local player");
            if(localPlayer != null)
            {
                localPlayer.GetComponent<PlayerMovement>().oponent = this.transform;
                print("oponent set for local player");
            }
            else
            {
                Debug.Log("local player not found");
            }

            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            this.gameObject.name = "local player";
            GameObject oponent = GameObject.Find("oponent");
            // We are the local player: Disable the scene camera
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

    // When we are destroyed
    void OnDisable()
    {
        // Re-enable the scene camera
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

}
