using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMoving : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject localPlayer = GameObject.Find("local player");
        PlayerMovement movment = localPlayer.GetComponent<PlayerMovement>();
        movment.canMove = false;
        //string player = "/" + animator.gameObject.name;
        //if (stateInfo.IsName("2Hand-Sword-Unsheath-Back-Unarmed"))
        //{
        //    //sword drawing
        //    GameObject sword = GameObject.Find
        //        (player+"/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        //    sword.GetComponent<MeshRenderer>().enabled = true;
        //}
        //else
        //{
        //    GameObject sword = GameObject.Find
        //        (player+"/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        //    sword.GetComponent<MeshRenderer>().enabled = false;
        //}
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
