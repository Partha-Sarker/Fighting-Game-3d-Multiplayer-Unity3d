using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    private AudioManager audioManager;
    [SerializeField]
    private Shake camShake;
    public Manager manager;
    private PlayerMovement playerMovement;
    public Collider shieldCollider;
    public int maxHealth = 100;
    public RectTransform myHealth;
    public RectTransform myHealthHolder;
    private Animator animator;
    private Animator shootAnimator;
    private NetworkAnimator networkAnimator;
    public Transform hitHolder, blockHolder;
    public GameObject fistHitParticle, swordHitParticle;
    public float shakeDelay = .1f;
    public int currentHealth;
    private Vector3 initialPos;
    private Vector3 initialRot;
    [SerializeField]
    private MeshRenderer shield;

    private TextMeshProUGUI myRatio, oponentRatio;

    private int winCount, loseCount;

    [SerializeField]
    private Transform fireballPos;
    [SerializeField]
    private GameObject fireBall;
    private float fireBallShakePosMag, fireBallShakeRotMag;
    [SerializeField]
    private int shootForce = 5;
    [SerializeField]
    public float shootingDelay = .1f;

    [SyncVar]
    public Vector3 scale;
    [SyncVar]
    public string ratio;
    [SyncVar]
    public bool isDead, isWinner;

    private void Start()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
        audioManager = GetComponent<AudioManager>();
        //camShake = FindObjectOfType<Shake>();
        manager = FindObjectOfType<Manager>();
        playerMovement = GetComponent<PlayerMovement>();
        shootAnimator = manager.shootAnimator;

        fireBallShakePosMag = fireBall.GetComponent<FireBall>().explosionShakeMag;
        fireBallShakeRotMag = fireBall.GetComponent<FireBall>().explosionRotMag;

        initialPos = transform.position;
        initialRot = transform.eulerAngles;

        SetupHealthBar();

        ResetAll();

    }

    private void SetupHealthBar()
    {
        if (isLocalPlayer)
        {
            myHealth = GameObject.Find("local player health").GetComponent<RectTransform>();
            myHealthHolder = GameObject.Find("local player health holder").GetComponent<RectTransform>();
            myRatio = GameObject.Find("local player ratio").GetComponent<TextMeshProUGUI>();
            //oponentRatio = GameObject.Find("oponent ratio").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            myHealth = GameObject.Find("oponent health").GetComponent<RectTransform>();
            myHealthHolder = GameObject.Find("oponent health holder").GetComponent<RectTransform>();
            myRatio = GameObject.Find("oponent ratio").GetComponent<TextMeshProUGUI>();
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.K) && isLocalPlayer)
    //        TakeDamage(30, "Magic");
    //    if (Input.GetKeyDown(KeyCode.V) && isLocalPlayer)
    //        Win();
    //    if (Input.GetKeyDown(KeyCode.C) && isLocalPlayer)
    //        ratio = "Shit :(";
    //}

    public void TakeDamage(int damage, string type)
    {
        if (isDead)
            return;
        if(type == "Magic")
            camShake.ShakeCam(fireBallShakePosMag, fireBallShakeRotMag);
        else
            camShake.ShakeCam(shakeDelay);


        if (type == "Sword")
        {
            //blood particle
            audioManager.PlaySFX("Sword Hit");
            GameObject particle = Instantiate(swordHitParticle, hitHolder);
            Destroy(particle, 1f);
        }
        else if(type == "Fist")
        {
            //hit effect
            audioManager.PlaySFX("Fist Hit");
            GameObject particle = Instantiate(fistHitParticle, hitHolder);
            Destroy(particle, 1f);
        }

        audioManager.PlaySFX("Pain");

        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        SetHealthBar(currentHealth);

        if (isLocalPlayer)
        {
            if (currentHealth == 0)
                Die();
            animator.SetInteger("HitNO", Random.Range(1, 4));
            animator.SetTrigger("GotHit");
        }
        if (currentHealth == 0 && !isLocalPlayer)
        {
            manager.localPlayer.GetComponent<Player>().Win();            
        }
    }

    public void BlockAttack(string type)
    {
        animator.SetTrigger("Blocked");
    }

    private void SetHealthBar(int amount)
    {
        scale.x = (float)amount / (float)maxHealth;
        myHealth.localScale = scale;
    }

    public void DisableShield()
    {
        shieldCollider.enabled = false;
    }

    public void EnableShield()
    {
        shieldCollider.enabled = true;
    }

    public void ResetAll()
    {
        manager.disableControl = false;
        isDead = false;
        isWinner = false;
        Attack.isWinner = false;
        animator.SetBool("Dead", false);
        animator.SetBool("Win", false);
        animator.SetBool("Armed", false);
        scale = Vector3.one;
        myHealthHolder.localScale = scale;
        currentHealth = maxHealth;
        SetHealthBar(maxHealth);
        transform.position = initialPos;
        transform.eulerAngles = initialRot;
        shield.enabled = true;
        GetComponent<PlayerMovement>().isGrounded = true;
        if (isLocalPlayer)
        {
            shootAnimator.Play("Shoot Recovering");
            RefreshRatio();
        }
    }

    public void RefreshRatio()
    {
        winCount = PlayerPrefs.GetInt("Win Count");
        loseCount = PlayerPrefs.GetInt("Lose Count");
        if (winCount == 0 && loseCount == 0)
            ratio = "First Match!";
        else if (loseCount == 0)
            ratio = "Undefeated!";
        else
        {
            float winLoseRatio = (float)winCount / (float)loseCount;
            ratio = winLoseRatio.ToString();
        }
        //myRatio.SetText(ratio);
        GetComponent<ActionControl>().SetOwnRatio(netId.ToString(), ratio);
    }

    public void SetMyRatio(string ratio)
    {
        SetupHealthBar();
        myRatio.SetText(ratio);
    }

    public void Die()
    {
        isDead = true;
        PlayerPrefs.SetInt("Lose Count", PlayerPrefs.GetInt("Lose Count")+1);
        RefreshRatio();
        animator.SetBool("Dead", true);
        //manager = FindObjectOfType<Manager>();
        manager.reMatchButton.gameObject.SetActive(true);
        manager.disableControl = true;
        audioManager.PlaySFX("Defeat");
        print("I am dead :(");
    }

    public void Win()
    {
        isWinner = true;
        PlayerPrefs.SetInt("Win Count", PlayerPrefs.GetInt("Win Count")+1);
        RefreshRatio();
        animator.SetBool("Win", true);
        audioManager.PlaySFX("Victory");
        //manager = FindObjectOfType<Manager>();
        manager.disableControl = true;
        print("I win :D");
    }

    private void OnDisable()
    {
        scale = Vector3.zero;
        if(myHealthHolder != null)
            myHealthHolder.localScale = scale;
    }

    public void HideShield()
    {
        shield.enabled = false;
    }

    public void ShootFireball()
    {
        //StartCoroutine(Shooting());
        Invoke("Shooting", shootingDelay);
    }

    public void Shooting()
    {
        //yield return new WaitForSeconds(shootingDelay);
        GameObject tempFireBall = Instantiate(fireBall, fireballPos);
        tempFireBall.GetComponent<Rigidbody>().AddForce(fireballPos.forward * shootForce, ForceMode.Impulse);
        audioManager.PlaySFX("Magic Shoot");
    }

}
