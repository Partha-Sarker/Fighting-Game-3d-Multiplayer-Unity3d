using System.Collections;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public bool pcInput = true;
    public GameObject joystick;
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
    }

    public void Sheath()
    {
        animator = localPlayer.GetComponent<Animator>();
        animator.SetBool("Armed", false);
    }

}