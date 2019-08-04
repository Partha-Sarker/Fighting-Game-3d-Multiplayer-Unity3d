using System.Collections;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private GameObject localPlayer;
    private GameObject oponent;
    public GameObject rightPanel;
    public GameObject controlPanel;
    private Animator animator;
    //public float showTimer = .3f;
    //public float hideTimer = .45f;

    void Update()
    {
        if (oponent == null)
            oponent = GameObject.Find("oponent");
        if (localPlayer == null)
            localPlayer = GameObject.Find("local player");
        if (oponent == null && rightPanel.activeSelf)
            rightPanel.SetActive(false);
        else if (oponent != null && !rightPanel.activeSelf)
            rightPanel.SetActive(true);
        if (localPlayer == null && controlPanel.activeSelf)
            controlPanel.SetActive(false);
        else if (localPlayer != null && !controlPanel.activeSelf)
            controlPanel.SetActive(true);
    }

    public void LockOponent()
    {
        localPlayer.GetComponent<PlayerMovement>().lockOponent = true;
    }

    public void UnlockOponent()
    {
        localPlayer.GetComponent<PlayerMovement>().lockOponent = false;
    }

    public void Unsheath()
    {
        animator = localPlayer.GetComponent<Animator>();
        animator.SetBool("Armed", true);
        //StartCoroutine("ShowSword");
        //localPlayer.GetComponent<LocalPlayerManager>().Unsheath();
    }

    public void Sheath()
    {
        animator = localPlayer.GetComponent<Animator>();
        animator.SetBool("Armed", false);
        //StartCoroutine("HideSword");
        //localPlayer.GetComponent<LocalPlayerManager>().Sheath();
    }

    //public void ShowSword()
    //{
    //    //yield return new WaitForSeconds(showTimer);
    //    GameObject sword = GameObject.Find
    //        ("/local player/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
    //    sword.GetComponent<MeshRenderer>().enabled = true;
    //}


    //public void HideSword()
    //{
    //    //yield return new WaitForSeconds(hideTimer);
    //    GameObject sword = GameObject.Find
    //        ("/local player/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
    //    sword.GetComponent<MeshRenderer>().enabled = false;
    //}

}