using System.Collections;
using UnityEngine;
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
    //public bool transitioning = false;
    //public float showTimer = .3f;
    //public float hideTimer = .45f;

    void Update()
    {
        if (oponent == null)
            oponent = GameObject.Find("oponent");
        if (localPlayer == null)
            localPlayer = GameObject.Find("local player");
        //if (oponent == null && rightPanel.activeSelf)
        //    rightPanel.SetActive(false);
        //else if (oponent != null && !rightPanel.activeSelf)
        //    rightPanel.SetActive(true);
        if (localPlayer == null && controlPanel.activeSelf)
            controlPanel.SetActive(false);
        else if (localPlayer != null && !controlPanel.activeSelf)
            controlPanel.SetActive(true);
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
        animator = localPlayer.GetComponent<Animator>();
        animator.SetBool("Armed", true);
    }

    public void Sheath()
    {
        //transitioning = true;
        //unsheathButton.GetComponent<Button>().interactable = false;
        animator = localPlayer.GetComponent<Animator>();
        animator.SetBool("Armed", false);
    }

}