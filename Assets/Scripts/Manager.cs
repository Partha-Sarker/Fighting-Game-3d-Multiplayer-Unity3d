using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public bool pcInput = true;
    public GameObject joystick;
    private GameObject localPlayer;
    private GameObject oponent;
    public GameObject rightPanel;
    public GameObject controlPanel;
    public GameObject sheathButton;
    public GameObject unsheathButton;
    private Animator animator;
    private NetworkAnimator networkAnimator;
    //public bool transitioning = false;
    //public float showTimer = .3f;
    //public float hideTimer = .45f;

    void Update()
    {
        if (oponent == null)
            oponent = GameObject.Find("oponent");
        if (localPlayer == null)
        {
            localPlayer = GameObject.Find("local player");
            if(controlPanel.activeSelf)
                controlPanel.SetActive(false);
        }
        else
        {
            animator = localPlayer.GetComponent<Animator>();
            networkAnimator = localPlayer.GetComponent<NetworkAnimator>();
            if (!controlPanel.activeSelf)
                controlPanel.SetActive(true);
        }
            
        //if (oponent == null && rightPanel.activeSelf)
        //    rightPanel.SetActive(false);
        //else if (oponent != null && !rightPanel.activeSelf)
        //    rightPanel.SetActive(true);
            
    }

    //public void LockOponent()
    //{
    //    localPlayer.GetComponent<PlayerMovement>().lockOponent = true;
    //}

    //public void UnlockOponent()
    //{
    //    localPlayer.GetComponent<PlayerMovement>().lockOponent = false;
    //}

    public void Unsheath()
    {
        //transitioning = true;
        //sheathButton.GetComponent<Button>().interactable = false;
        animator.ResetTrigger("Attacking");
        animator.SetBool("Armed", true);
    }

    public void Sheath()
    {
        //transitioning = true;
        //unsheathButton.GetComponent<Button>().interactable = false;
        animator.SetBool("Armed", false);
    }

    public void Attack()
    {
        animator.SetTrigger("Attacking");
        networkAnimator.SetTrigger("Attacking");
        animator.SetInteger("AttackNO", Random.Range(1, 6));
    }

}