using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireBall : MonoBehaviour
{
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private float destroyDelay = .2f;
    private ActionControl actionControl;
    public int damage = 30;

    private void Start()
    {
        actionControl = transform.root.GetComponent<ActionControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.name == transform.root.name)
            return;

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameObject tempExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, destroyDelay);

        if (other.tag != "Player")
            return;

        if(transform.root.name == "local player")
            actionControl.Damage(other.GetComponent<NetworkIdentity>().netId.ToString(), damage, "Magic");

    }
}
