using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorActivation : MonoBehaviour
{
    public List<GameObject> Boundaires;
    public Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that triggered this collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            foreach (GameObject boundary in Boundaires)
            {
                if (boundary != null)
                {
                    boundary.SetActive(true);
                }
            }

            animator.enabled = true;
        }
    }



}
