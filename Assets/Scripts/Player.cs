using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    public int maxHealth = 100;
    public TextMeshProUGUI myScore;

    [SyncVar]
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        if (isLocalPlayer)
            myScore = GameObject.Find("my score").GetComponent<TextMeshProUGUI>();
        else
            myScore = GameObject.Find("oponent score").GetComponent<TextMeshProUGUI>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        Debug.Log(transform.name + "now has " + currentHealth + " health");
        myScore.SetText(currentHealth.ToString());
    }
}
