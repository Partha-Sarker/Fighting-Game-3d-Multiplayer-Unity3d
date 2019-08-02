using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private GameObject localPlayer;
    private GameObject oponent;

    public void LockOponent()
    {
        localPlayer = GameObject.Find("local player");
        //oponent = GameObject.Find("oponent");
        //Vector3 direction = oponent.transform.position - localPlayer.transform.position;
        //Quaternion rotation = Quaternion.LookRotation(direction);
        //localPlayer.transform.rotation = rotation;
        localPlayer.GetComponent<PlayerMovement>().lockOponent = true;
        //StartCoroutine("SmoothCamera");
    }

    IEnumerator SmoothCamera()
    {
        yield return new WaitForSeconds(1f);
        localPlayer.GetComponent<PlayerMovement>().lockOponent = true;
    }

    public void UnlockOponent()
    {
        localPlayer = GameObject.Find("local player");
        localPlayer.GetComponent<PlayerMovement>().lockOponent = false;
    }

}
