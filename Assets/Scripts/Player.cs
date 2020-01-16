using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    public Collider shieldCollider;
    public int maxHealth = 100;
    //public TextMeshProUGUI myScore;
    public RectTransform myHealth;
    public RectTransform myHealthHolder;
    private Animator animator;
    private NetworkAnimator networkAnimator;
    public Transform hitHolder, blockHolder;
    public GameObject fistHitParticle, swordHitParticle;
    private Vector3 scale;
    private bool isDead;

    [SyncVar]
    public int currentHealth;

    private void Start()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();

        if (isLocalPlayer)
        {
            myHealth = GameObject.Find("local player health").GetComponent<RectTransform>();
            myHealthHolder = GameObject.Find("local player health holder").GetComponent<RectTransform>();
        }
        else
        {
            myHealth = GameObject.Find("oponent health").GetComponent<RectTransform>();
            myHealthHolder = GameObject.Find("oponent health holder").GetComponent<RectTransform>();
        }

        ResetPlayer();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(40);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        if(damage == 0)
        {
            animator.SetTrigger("Blocked");
            return;
        }

        if(damage == 7)
        {
            //blood particle
            GameObject particle = Instantiate(swordHitParticle, hitHolder);
            Destroy(particle, 1f);
        }
        else
        {
            //hit effect
            GameObject particle = Instantiate(fistHitParticle, hitHolder);
            Destroy(particle, 1f);
        }

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

    private void ResetPlayer()
    {
        isDead = false;
        animator.SetBool("Dead", false);
        scale = Vector3.one;
        myHealthHolder.localScale = scale;
        currentHealth = maxHealth;
        SetHealthBar(maxHealth);
    }

    private void OnDestroy()
    {
        scale = Vector3.zero;
        myHealthHolder.localScale = scale;
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
    }

}
