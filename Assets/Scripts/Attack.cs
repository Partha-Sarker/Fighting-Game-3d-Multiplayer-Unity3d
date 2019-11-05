using UnityEngine;
using UnityEngine.Networking;


public class Attack : NetworkBehaviour
{
    private static bool canDamage = false;

    private void Start()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage)
            return;
        if (other.name == "local player")
            return;
        if (other.tag != "Player" && other.tag != "PlayerParts")
            return;
        string id = other.transform.root.GetComponent<NetworkIdentity>().netId.ToString();
        DealDamage(id);
        canDamage = false;
    }
    
    public virtual void DealDamage(string id)
    {
        //Debug.Log("Dealing damage");
    }

    public static void CanDamage()
    {
        canDamage = true;
        //Debug.Log(canDamage);
    }

    public static void CantDamage()
    {
        canDamage = false;
        //Debug.Log(canDamage);
    }
}
