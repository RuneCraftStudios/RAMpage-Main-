using UnityEngine;

public class ElectricityEffect : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Transform chargingStationMesh; // Reference to the charging station mesh
    public ParticleSystem electricityParticleSystem; // Reference to the electricity particle system

    private void Update()
    {
        if (player != null && chargingStationMesh != null && electricityParticleSystem != null)
        {
            // Get the direction from the charging station mesh to the player's hand collider
            Vector3 direction = (player.transform.position - chargingStationMesh.position).normalized;

            // Set the velocity of the particles in the direction towards the player
            var particleSystemVelocity = electricityParticleSystem.velocityOverLifetime;
            particleSystemVelocity.enabled = true;
            particleSystemVelocity.xMultiplier = direction.x;
            particleSystemVelocity.yMultiplier = direction.y;
            particleSystemVelocity.zMultiplier = direction.z;
        }
    }
}
