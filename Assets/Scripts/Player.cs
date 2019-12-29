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

    [SyncVar]
    public int currentHealth;

    private void Start()
    {
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
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        //Debug.Log(transform.name + "now has " + currentHealth + " health");
        //myScore.SetText(currentHealth.ToString());
        myHealth.fillAmount = (float)currentHealth / (float)maxHealth;
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
