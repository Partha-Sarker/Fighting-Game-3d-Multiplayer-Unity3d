using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour
{
    private bool pcInput;
    private Animator animator;
    private float HInput = 0, VInput = 0;
    public float speed = 1.2f;
    private Rigidbody rb;
    private Vector3 velocity = Vector3.zero;
    public Joystick joystick;
    public Transform oponent;
    //public bool lockOponent = true;
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        pcInput = FindObjectOfType<Manager>().pcInput;
        if (!pcInput)
        {
            joystick = FindObjectOfType<Manager>().joystick.GetComponent<Joystick>();
            joystick.gameObject.SetActive(true);
        }
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        if(!pcInput)
        joystick.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pcInput)
            GetPcInput();
        else
            GetAndroidInput();
        if(oponent != null)
        {
            RotatePlayer();
        }
            

        animator.SetFloat("HInput", HInput, .05f, Time.deltaTime);
        animator.SetFloat("VInput", VInput, .05f, Time.deltaTime);
        animator.SetFloat("Speed", speed);

        //if(canMove) MovePlayer();

    }

    private void RotatePlayer()
    {
        float AngularDistance = Quaternion.Angle(transform.rotation, oponent.rotation);
        Vector3 direction = oponent.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
        //print("local: "+AngularDistance + "|" + direction + "|" + rotation);

        AngularDistance = Quaternion.Angle(oponent.rotation, transform.rotation);
        direction = transform.position - oponent.position;
        rotation = Quaternion.LookRotation(direction);
        oponent.rotation = rotation;
        //print("oponent: " + AngularDistance + "|" + direction + "|" + rotation);
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
        HInput = Input.GetAxisRaw("Horizontal");
        VInput = Input.GetAxisRaw("Vertical");
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
