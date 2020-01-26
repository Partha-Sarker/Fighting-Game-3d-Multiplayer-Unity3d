using UnityEngine;
using UnityEngine.Networking;


public class Attack : NetworkBehaviour
{
    private static bool canDamage = false;
    public static bool isWinner;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage || isWinner)
            return;

        if (other.name == "local player")
            return;
        if (other.tag != "Player" && other.tag != "Shield")
            return;

        string id = other.transform.root.GetComponent<NetworkIdentity>().netId.ToString();
        if (other.tag == "Shield")
        {
            DealDamage(id, true);
        }
        else
        {
            DealDamage(id, false);
        }

        canDamage = false;

    }

    public virtual void DealDamage(string id, bool isBlocked)
    {
        Debug.Log("You have hit the oponent. Dealing damage");
    }

    //public virtual void Deflect(string id)
    //{
    //    Debug.Log("Oponent has blocked the attack!");
    //}

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
