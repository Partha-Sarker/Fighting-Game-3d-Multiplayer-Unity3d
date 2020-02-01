using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    public bool pcInput = true;
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public static bool isServer = false;
    private string localId;
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
    public Button shootButton;
    public Button soundOnButton;
    public Button soundOffButton;
    public TextMeshProUGUI winStatus, loseStatus;
    private int winCount, loseCount;
    private Image attackButtonImage;
    public Sprite swordIcon, fistIcon;
    public float jumpButtonDelay = 1.5f;
    public float sheathUnsheathDelay = 1f;
    public float guardUnguardDelay = .5f;
    public float shootButtonDelay = 3f;
    public float unarmedAttackButtonDelay = .5f;
    public float armedAttackExtraDelay = .2f;
    private float defaultUnarmedAttackButtonDelay;
    public float attackOtherButtonDelay = .5f;
    public byte currentAttackCount = 0;

    private Animator animator;
    public Animator shootAnimator;
    private NetworkAnimator networkAnimator;
    private ActionControl actionControl;

    private bool isGuarding = false;
    private bool isArmed = false;
    private bool isSet = false;
    public static bool isRefresed = false;

    [HideInInspector]
    public bool disableControl = false;

    private void Start()
    {
        defaultUnarmedAttackButtonDelay = unarmedAttackButtonDelay;
        attackButtonImage = attackButton.GetComponent<Image>();
        if (PlayerPrefs.GetInt("Sound") == 1)
            soundOffButton.onClick.Invoke();
    }

    void Update()
    {
        if (localPlayer == null)
        {
            localPlayer = GameObject.Find("local player");
            if(controlPanel.activeSelf)
                controlPanel.SetActive(false);
        }
        else
        {
            if(animator == null || networkAnimator == null || actionControl == null)
            {
                animator = localPlayer.GetComponent<Animator>();
                networkAnimator = localPlayer.GetComponent<NetworkAnimator>();
                actionControl = localPlayer.GetComponent<ActionControl>();
                localId = actionControl.netId.ToString();
            }
            if (!controlPanel.activeSelf && !disableControl)
                ResetControl();
            if (disableControl && controlPanel.activeSelf)
                controlPanel.SetActive(false);
        }

        if (oponent == null)
        {
            if(isSet)
                isSet = false;
            oponent = GameObject.Find("oponent");
        }
        else
        {
            if (!isSet)
            {
                shootAnimator.SetTrigger("Shoot");
                RefreshEverything();
                actionControl.SetOwnRatio(localPlayer.GetComponent<NetworkIdentity>().netId.ToString(), localPlayer.GetComponent<Player>().ratio);
                isSet = true;
            }
        }

        if (localPlayer == null && oponent == null && !isRefresed)
            RefreshEverything();

        GetKeyInput();
    }

    private void GetKeyInput()
    {
        if (Input.GetKeyUp(KeyCode.F) && shootButton.IsActive() && shootButton.IsInteractable())
            shootButton.onClick.Invoke();

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

        if ((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.Space)) && jumpButton.IsActive() && jumpButton.IsInteractable())
            jumpButton.onClick.Invoke();
    }

    private void RefreshEverything()
    {
        winCount = PlayerPrefs.GetInt("Win Count");
        loseCount = PlayerPrefs.GetInt("Lose Count");
        string winText, loseText;
        if (winCount == 0)
            winText = "No\nWin";
        else if (winCount == 1)
            winText = "1\nWin";
        else
            winText = winCount + "\nWins";

        if (loseCount == 0)
            loseText = "No\nDefeat";
        else if (winCount == 1)
            loseText = "1\nDefeat";
        else
            loseText = loseCount + "\nDefeats";
        winStatus.SetText(winText);
        loseStatus.SetText(loseText);
        isRefresed = true;
    }

    private void ResetControl()
    {
        controlPanel.SetActive(true);
        unsheathButton.gameObject.SetActive(true);
        sheathButton.gameObject.SetActive(false);
        guardButton.gameObject.SetActive(true);
        unguardButton.gameObject.SetActive(false);
        attackButtonImage.sprite = fistIcon;
        isArmed = false;
    }

    public void Unsheath()
    {
        currentAttackCount = 0;
        //animator.ResetTrigger("Attacking");
        UnguardIfGuarded();
        animator.SetBool("Armed", true);
        isArmed = true;
        attackButtonImage.sprite = swordIcon;
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
        attackButtonImage.sprite = fistIcon;
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
        //shootButton.interactable = false;
    }

    public void ShootFireBall()
    {
        //actionControl.Shoot(localId);
        //animator.SetTrigger("Magic");
        networkAnimator.SetTrigger("Magic");
        shootAnimator.SetTrigger("Shoot");
        DisableButtons();
        StartCoroutine(EnableButtons(shootButtonDelay));
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
            //shootButton.interactable = true;
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

    public void OnSoundOnClicked()
    {
        AudioManager.isMasterVolumeOn = true;
        PlayerPrefs.SetInt("Sound", 0);
    }

    public void OnSoundOffClicked()
    {
        AudioManager.isMasterVolumeOn = false;
        PlayerPrefs.SetInt("Sound", 1);
    }

    public void OnExitClicked()
    {
        Application.Quit();
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