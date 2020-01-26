using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public bool pcInput = true;
    public static bool isServer = false;
    public GameObject joystick;
    [HideInInspector]
    public GameObject localPlayer;
    [HideInInspector]
    public GameObject oponent;
    public GameObject rightPanel;
    public GameObject controlPanel;
    public GameObject lifePanel;
    public Button jumpButton;
    public Button sheathButton;
    public Button unsheathButton;
    public Button attackButton;
    public Button guardButton;
    public Button unguardButton;
    public Button reMatchButton;
    public float jumpButtonDelay = 1.5f;
    public float sheathUnsheathDelay = 1f;
    public float guardUnguardDelay = .5f;
    public float unarmedAttackButtonDelay = .5f;
    public float armedAttackExtraDelay = .2f;
    private float defaultUnarmedAttackButtonDelay;
    public float attackOtherButtonDelay = .5f;
    public byte currentAttackCount = 0;

    private Animator animator;
    private NetworkAnimator networkAnimator;
    private ActionControl actionControl;

    private bool isGuarding = false;
    private bool isArmed = false;

    [HideInInspector]
    public bool disableControl = false;

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
            if(animator == null)
                animator = localPlayer.GetComponent<Animator>();
            if(networkAnimator == null)
                networkAnimator = localPlayer.GetComponent<NetworkAnimator>();
            if (actionControl == null)
                actionControl = localPlayer.GetComponent<ActionControl>();
            if (!controlPanel.activeSelf && !disableControl)
                controlPanel.SetActive(true);
            if (disableControl && controlPanel.activeSelf)
                controlPanel.SetActive(false);
        }

        GetKeyInput();
    }

    private void GetKeyInput()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (isGuarding && unguardButton.IsActive() && unguardButton.IsInteractable())
                unguardButton.onClick.Invoke();
            else if (!isGuarding && guardButton.IsActive() && guardButton.IsInteractable())
                guardButton.onClick.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) && attackButton.IsActive() && attackButton.IsInteractable())
            attackButton.onClick.Invoke();

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (isArmed && sheathButton.IsActive() && sheathButton.IsInteractable())
                sheathButton.onClick.Invoke();
            else if (!isArmed && unsheathButton.IsActive() && unsheathButton.IsInteractable())
                unsheathButton.onClick.Invoke();
        }

        if ((Input.GetKeyUp(KeyCode.DownArrow) | Input.GetKeyUp(KeyCode.Space)) && jumpButton.IsActive() && jumpButton.IsInteractable())
            jumpButton.onClick.Invoke();
    }

    public void Unsheath()
    {
        currentAttackCount = 0;
        //animator.ResetTrigger("Attacking");
        UnguardIfGuarded();
        animator.SetBool("Armed", true);
        isArmed = true;
        unarmedAttackButtonDelay += armedAttackExtraDelay;
        DisableButtons();
        StartCoroutine(EnableButtons(sheathUnsheathDelay));
    }

    public void Sheath()
    {
        currentAttackCount = 0;
        UnguardIfGuarded();
        animator.SetBool("Armed", false);
        isArmed = false;
        unarmedAttackButtonDelay = defaultUnarmedAttackButtonDelay;
        DisableButtons();
        StartCoroutine(EnableButtons(sheathUnsheathDelay));
    }

    public void Attack()
    {
        currentAttackCount++;
        UnguardIfGuarded();
        animator.SetInteger("AttackNO", Random.Range(1, 6));
        animator.SetTrigger("Attacking");
        //networkAnimator.SetTrigger("Attacking");
        DisableButtons();
        StartCoroutine(EnableButtons(unarmedAttackButtonDelay));
    }

    public void Jump()
    {
        currentAttackCount = 0;
        localPlayer.GetComponent<PlayerMovement>().Jump();
        UnguardIfGuarded();
        DisableButtons();
        StartCoroutine(EnableButtons(jumpButtonDelay));
    }

    public void Guard()
    {
        currentAttackCount = 0;
        animator.SetBool("IsBlocking", true);
        isGuarding = true;
        DisableButtons();
        StartCoroutine(EnableButtons(guardUnguardDelay));
    }

    public void Unguard()
    {
        currentAttackCount = 0;
        animator.SetBool("IsBlocking", false);
        isGuarding = false;
        DisableButtons();
        StartCoroutine(EnableButtons(guardUnguardDelay));
    }

    private void UnguardIfGuarded()
    {

        if (animator.GetBool("IsBlocking"))
        {
            animator.SetBool("IsBlocking", false);
            isGuarding = false;
            unguardButton.gameObject.SetActive(false);
            guardButton.gameObject.SetActive(true);
        }
    }

    public void ReMatch()
    {
        actionControl.ResetALlPlayers();
    }

    private void DisableButtons()
    {
        jumpButton.interactable = false;
        sheathButton.interactable = false;
        unsheathButton.interactable = false;
        attackButton.interactable = false;
        guardButton.interactable = false;
        unguardButton.interactable = false;
    }

    IEnumerator EnableButtons(float timer)
    {
        yield return new WaitForSeconds(timer);
        attackButton.interactable = true;
        if (currentAttackCount > 0)
        {
            yield return new WaitForSeconds(attackOtherButtonDelay);
        }
        if(currentAttackCount <= 1)
        {
            jumpButton.interactable = true;
            sheathButton.interactable = true;
            unsheathButton.interactable = true;
            guardButton.interactable = true;
            unguardButton.interactable = true;
        }
        if (currentAttackCount > 0)
            currentAttackCount--;
    }

    public static void RegisterPlayer(string id, Player player)
    {
        players.Add(id, player);
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public static List<string> GetAllPlayer()
    {
        List<string> allPlayerKey = new List<string>();
        foreach (KeyValuePair<string, Player> player in players)
        {
            allPlayerKey.Add(player.Key);
        }

        return allPlayerKey;

    }

    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(0, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
}