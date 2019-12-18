using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    PlayerMovement playerMovement;
    Animator animator;
    public GroundCheck otherGroundCHeck;
    public bool isGrounded = true;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground")
        {
            isGrounded = true;
            animator.applyRootMotion = true;
            //playerMovement.isGrounded = true;
            animator.SetBool("IsGrounded", true);
            playerMovement.isJumping = false;
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isGrounded = false;
            if (otherGroundCHeck.isGrounded != false)
                return;
            playerMovement.isGrounded = false;
            animator.SetBool("IsGrounded", false);
        }
    }


}
