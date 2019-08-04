using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerManager : MonoBehaviour
{
    private Animator animator;
    public float showTimer = .3f;
    public float hideTimer = .45f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Unsheath()
    {
        animator.SetBool("Armed", true);
        StartCoroutine("ShowSword");
    }

    public void Sheath()
    {
        animator.SetBool("Armed", false);
        StartCoroutine("HideSword");
    }

    IEnumerator ShowSword()
    {
        yield return new WaitForSeconds(showTimer);
        GameObject sword = GameObject.Find
            ("/local player/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        sword.GetComponent<MeshRenderer>().enabled = true;
    }


    IEnumerator HideSword()
    {
        yield return new WaitForSeconds(hideTimer);
        GameObject sword = GameObject.Find
            ("/local player/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        sword.GetComponent<MeshRenderer>().enabled = false;
    }
}
