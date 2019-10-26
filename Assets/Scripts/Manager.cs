using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public bool pcInput = true;
    public GameObject joystick;
    private GameObject localPlayer;
    private GameObject oponent;
    public GameObject rightPanel;
    public GameObject controlPanel;
    public Button jumpButton;
    public Button sheathButton;
    public Button unsheathButton;
    public Button attackButton;
    public float jumpButtonDelay = 1.5f;
    public float sheathUnsheathDelay = 1f;
    public float unarmedAttackButtonDelay = .5f;
    private float defaultUnarmedAttackButtonDelay;
    public float attackOtherButtonDelay = .5f;
    public byte currentAttackCount = 0;
    private bool armed = false;
    private Animator animator;
    private NetworkAnimator networkAnimator;

    private void Start()
    {
        defaultUnarmedAttackButtonDelay = unarmedAttackButtonDelay;
    }

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
            
    }

    public void Unsheath()
    {
        currentAttackCount = 0;
        animator.ResetTrigger("Attacking");
        animator.SetBool("Armed", true);
        unarmedAttackButtonDelay += .2f;
        DisableButtons();
        StartCoroutine(EnableButtons(sheathUnsheathDelay));
    }

    public void Sheath()
    {
        currentAttackCount = 0;
        animator.SetBool("Armed", false);
        unarmedAttackButtonDelay = defaultUnarmedAttackButtonDelay;
        DisableButtons();
        StartCoroutine(EnableButtons(sheathUnsheathDelay));
    }

    public void Attack()
    {
        currentAttackCount++;
        animator.SetTrigger("Attacking");
        networkAnimator.SetTrigger("Attacking");
        animator.SetInteger("AttackNO", Random.Range(1, 6));
        DisableButtons();
        StartCoroutine(EnableButtons(unarmedAttackButtonDelay));
    }

    public void Jump()
    {
        currentAttackCount = 0;
        localPlayer.GetComponent<PlayerMovement>().Jump();
        DisableButtons();
        StartCoroutine(EnableButtons(jumpButtonDelay));
    }

    private void DisableButtons()
    {
        jumpButton.interactable = false;
        sheathButton.interactable = false;
        unsheathButton.interactable = false;
        attackButton.interactable = false;
    }

    IEnumerator EnableButtons(float timer)
    {
        yield return new WaitForSeconds(timer);
        attackButton.interactable = true;
        if(currentAttackCount > 0)
        {
            yield return new WaitForSeconds(attackOtherButtonDelay);
        }
        if(currentAttackCount <= 1)
        {
            jumpButton.interactable = true;
            sheathButton.interactable = true;
            unsheathButton.interactable = true;
        }
        if (currentAttackCount > 0)
            currentAttackCount--;
    }

}