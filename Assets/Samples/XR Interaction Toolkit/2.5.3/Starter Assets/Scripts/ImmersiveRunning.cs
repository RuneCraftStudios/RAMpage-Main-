using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ImmersiveRunning : MonoBehaviour
{
    // Reference to the DynamicMoveProvider component
    [SerializeField] private DynamicMoveProvider moveProvider;

    // Collider references
    [SerializeField] private Collider topCollider;
    [SerializeField] private Collider bottomCollider;
    [SerializeField] private Collider frontCollider;

    // Buffers for phase transitions
    [SerializeField] private float increasePhaseBufferTime = 1f;
    [SerializeField] private float keepPhaseBufferTime = 1f;

    // Move speed values for different phases
    [SerializeField] private float slowWalkSpeed = 2f;
    [SerializeField] private float fastWalkSpeed = 4f;
    [SerializeField] private float slowRunSpeed = 6f;
    [SerializeField] private float fastRunSpeed = 8f;

    // Current phase
    private string currentPhase = "Stop";

    // Time stamps for last collider triggers
    private float lastTopColliderEnterTime;
    private float lastBottomColliderEnterTime;
    private float lastFrontColliderEnterTime;

    // Layer mask for collider checks
    [SerializeField] private LayerMask colliderLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the DynamicMoveProvider component is assigned
        if (moveProvider == null)
        {
            Debug.LogError("DynamicMoveProvider is not assigned.");
            return;
        }

        // Ensure collider references are assigned
        if (topCollider == null || bottomCollider == null || frontCollider == null)
        {
            Debug.LogError("Collider references are not assigned.");
            return;
        }

        // Initialize last collider enter times
        lastTopColliderEnterTime = Time.time;
        lastBottomColliderEnterTime = Time.time;
        lastFrontColliderEnterTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
    }

    private void UpdatePhase()
    {
        // Calculate time differences
        float topDelta = Time.time - lastTopColliderEnterTime;
        float bottomDelta = Time.time - lastBottomColliderEnterTime;
        float frontDelta = Time.time - lastFrontColliderEnterTime;

        // Check if two colliders were triggered within the buffer times
        if ((topDelta <= increasePhaseBufferTime && bottomDelta <= increasePhaseBufferTime) ||
            (topDelta <= increasePhaseBufferTime && frontDelta <= increasePhaseBufferTime) ||
            (bottomDelta <= increasePhaseBufferTime && frontDelta <= increasePhaseBufferTime))
        {
            // First initiation
            if (currentPhase == "Stop")
            {
                currentPhase = "SlowWalk";
                moveProvider.moveSpeed = slowWalkSpeed;
            }
            else if (currentPhase == "SlowWalk")
            {
                currentPhase = "FastWalk";
                moveProvider.moveSpeed = fastWalkSpeed;
            }
            else if (currentPhase == "FastWalk")
            {
                currentPhase = "SlowRun";
                moveProvider.moveSpeed = slowRunSpeed;
            }
            else if (currentPhase == "SlowRun")
            {
                currentPhase = "FastRun";
                moveProvider.moveSpeed = fastRunSpeed;
            }
        }
        // Check if two colliders were triggered within the keep buffer time
        else if ((topDelta <= keepPhaseBufferTime && bottomDelta <= keepPhaseBufferTime) ||
                 (topDelta <= keepPhaseBufferTime && frontDelta <= keepPhaseBufferTime) ||
                 (bottomDelta <= keepPhaseBufferTime && frontDelta <= keepPhaseBufferTime))
        {
            // Keep current phase
        }
        else
        {
            // Reset to stop phase
            currentPhase = "Stop";
            moveProvider.moveSpeed = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the specified layer mask
        if (((1 << other.gameObject.layer) & colliderLayerMask) != 0)
        {
            // Update the time stamp when a collider is triggered
            if (other == topCollider)
            {
                lastTopColliderEnterTime = Time.time;
            }
            else if (other == bottomCollider)
            {
                lastBottomColliderEnterTime = Time.time;
            }
            else if (other == frontCollider)
            {
                lastFrontColliderEnterTime = Time.time;
            }
        }
    }
}
