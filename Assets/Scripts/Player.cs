using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    public Collider shieldCollider;
    public int maxHealth = 100;
    //public TextMeshProUGUI myScore;
    public Image myHealth;
    private Animator animator;
    private NetworkAnimator networkAnimator;
    public Transform hitHolder, blockHolder;
    public GameObject fistHitParticle, swordHitParticle;

    [SyncVar]
    public int currentHealth;

    private void Start()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();

        currentHealth = maxHealth;

        if (isLocalPlayer)
            myHealth = GameObject.Find("local player health").GetComponent<Image>();
        else
            myHealth = GameObject.Find("oponent health").GetComponent<Image>();

        myHealth.fillAmount = 1;

        //if (isLocalPlayer)
        //    myScore = GameObject.Find("my score").GetComponent<TextMeshProUGUI>();
        //else
        //    myScore = GameObject.Find("oponent score").GetComponent<TextMeshProUGUI>();

    }

    public void TakeDamage(int damage)
    {
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
        myHealth.fillAmount = (float)currentHealth / (float)maxHealth;

        if (isLocalPlayer)
        {
            animator.SetInteger("HitNO", Random.Range(1, 4));
            animator.SetTrigger("GotHit");
        }
    }

    public void DisableShield()
    {
        shieldCollider.enabled = false;
    }

    public void EnableShield()
    {
        shieldCollider.enabled = true;
    }
}
