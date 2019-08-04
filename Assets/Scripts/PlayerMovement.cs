using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private float HInput = 0, VInput = 0;
    public float speed = 5;
    private Rigidbody rb;
    private Vector3 velocity = Vector3.zero;
    public Joystick joystick;
    public Transform oponent;
    public bool lockOponent = true;

    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPcInput();
        //GetAndroidInput();
        if(oponent != null && lockOponent)
            RotatePlayer();

        animator.SetFloat("HInput", HInput);
        animator.SetFloat("VInput", VInput);

        //MovePlayer();

    }

    private void RotatePlayer()
    {
        float AngularDistance = Quaternion.Angle(transform.rotation, oponent.rotation);
        Vector3 direction = oponent.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        if (180 - AngularDistance < 25)
            transform.rotation = rotation;
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime);
    }

    private void GetAndroidInput()
    {
        HInput = joystick.Horizontal;
        if (HInput > .33) HInput = 1;
        else if (HInput < -.33) HInput = -1;
        else HInput = 0;
        VInput = joystick.Vertical;
        if (VInput > .33) VInput = 1;
        else if (VInput < -.33) VInput = -1;
        else VInput = 0;
    }

    private void GetPcInput()
    {
        HInput = Input.GetAxis("Horizontal");
        VInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer()
    {
        Vector3 moveHorizontal = transform.right * HInput;
        Vector3 moveVertical = transform.forward * VInput;

        velocity = (moveHorizontal + moveVertical).normalized * speed;
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }
    }

}
