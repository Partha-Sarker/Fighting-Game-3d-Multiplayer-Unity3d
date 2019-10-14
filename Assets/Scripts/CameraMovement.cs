using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Vector3 offset;
    private Transform player;
    public int rotationSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent;
        transform.parent = null;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
        transform.RotateAround(player.transform.up, transform.position, 100 * Time.deltaTime);
    }
}
