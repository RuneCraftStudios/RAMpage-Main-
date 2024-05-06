using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class IdleBoss : StateMachineBehaviour
{
    private Transform playerTransform; // Cache the player's transform
    private bool playerInCloseRange;
    private bool playerInMidRange;
    private bool playerInLongRange;
    private Animator animator;
    private NavMeshAgent agent;
    public LayerMask PlayerLayer;
    public int CloseRange;
    public int MidRange;
    public int LongRange;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }

        //ResetAnimatorBools

        animator.SetBool("playerInCloseRange", false);
        animator.SetBool("playerInMidRange", false);
        animator.SetBool("playerInLongRange", false);
        animator.SetBool("isNavigating", false);

        agent.isStopped = true;
        Vector3 bossPosition = animator.gameObject.transform.position;

        // Check if the player is within the specified ranges
        playerInCloseRange = Physics.CheckSphere(animator.gameObject.transform.position, CloseRange, PlayerLayer);
        playerInMidRange = Physics.CheckSphere(animator.gameObject.transform.position, MidRange, PlayerLayer);
        playerInLongRange = Physics.CheckSphere(animator.gameObject.transform.position, LongRange, PlayerLayer);

        if (playerInCloseRange)
        {
            animator.SetBool("playerInCloseRange", true);
        }

        if (playerInMidRange)
        {
            animator.SetBool("playerInMidRange", true);
        }

        if (playerInLongRange)
        {
            animator.SetBool("playerInLongRange", true);
        }

        if (!playerInLongRange)
        {
            agent.isStopped = false;
            agent.speed = 2f;
            agent.SetDestination(playerTransform.position);
            animator.SetBool("isNavigating", true);
        }


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isNavigating", false);
    }

}
