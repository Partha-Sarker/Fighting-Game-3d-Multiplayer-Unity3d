using UnityEngine;

public class SlowRotation : StateMachineBehaviour
{
    private GameObject player;
    private PlayerMovement playerMovement;
    private Player playerScript;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.gameObject;
            playerScript = player.GetComponent<Player>();
        }
        
        playerScript.EnableShield();

        if (player.name == "local player")
        {
            if (playerMovement == null) { }
                playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.SlowSelfRotation();

        }
        else
        {
            if (playerMovement == null)
                playerMovement = GameObject.Find("local player").GetComponent<PlayerMovement>();
            playerMovement.SlowOponentRotation();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.name == "local player")
        {
            if (playerMovement == null) { }
            playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.ResetSelfRotation();

        }
        else
        {
            if (playerMovement == null)
                playerMovement = GameObject.Find("local player").GetComponent<PlayerMovement>();
            playerMovement.ResetOponentRotation();
        }
    }

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
