using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField]
    private float selfDestructionDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, selfDestructionDelay);
    }

}
