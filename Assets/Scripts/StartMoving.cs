using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMoving : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!FindObjectOfType<Manager>().transitioning)
            return;
        GameObject sheathButton = GameObject.Find("Sheath");
        GameObject unsheathButton = GameObject.Find("Unsheath");
        if(sheathButton != null)
        {
            sheathButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            unsheathButton.GetComponent<Button>().interactable = true;
        }
        GameObject localPlayer = GameObject.Find("local player");
        PlayerMovement movment = localPlayer.GetComponent<PlayerMovement>();
        movment.canMove = true;
        FindObjectOfType<Manager>().transitioning = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
