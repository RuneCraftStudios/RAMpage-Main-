using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyAiTutorial
{
    public float timeBetweenAttacks;
    public float raycastCooldownDuration;
    private float lastRaycastTime;
    private int totalRaycasts;
    private int failedRaycasts;
    public GameObject grenade;
    private float grenadeSpeed;
    bool isGrenadeThrown = false;

    public EnemyState RangedEnemyState { get; private set; }

    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            ChangeState(EnemyState.Decision);
            return;
        }

        if (Time.time - lastRaycastTime < raycastCooldownDuration)
            return;

        foreach (Transform muzzleTransform in muzzleTransforms)
        {
            if (currentState == EnemyState.Relocating || currentState == EnemyState.ThrowingGrenade)
                return;

            Vector3 directionToPlayer = playerCollider.bounds.center - muzzleTransform.position;
            Aim(directionToPlayer, muzzleTransform);
            Debug.DrawRay(muzzleTransform.position, directionToPlayer, Color.green, 2.0f);

            RaycastHit hit;
            totalRaycasts++;
            if (RaycastCheck(muzzleTransform.position, directionToPlayer, Mathf.Infinity, out hit, PlayerLayer))
            {
                RotateTowardsPlayer();
                InstantiateProjectile(muzzleTransform);
                lastRaycastTime = Time.time;
                failedRaycasts = 0;
            }
            else
            {
                failedRaycasts++;
                StartCoroutine(HandleFailedRaycasts());
            }
        }
    }

    private IEnumerator HandleFailedRaycasts()
    {
        if (failedRaycasts >= 1 && failedRaycasts <= 4)
        {
            ChangeState(EnemyState.Relocating);
        }
        else if (failedRaycasts >= 5)
        {
            ChangeState(EnemyState.ThrowingGrenade);
            failedRaycasts = 0; // Reset the count after deciding to throw a grenade
        }

        yield return null; // This coroutine might not need to yield a wait, but it's here to fit coroutine structure
    }

    private void InstantiateProjectile(Transform muzzleTransform)
    {
        Instantiate(Projectile, muzzleTransform.position, muzzleTransform.rotation);
    }

    private void Aim(Vector3 directionToPlayer, Transform muzzleTransform)
    {
        muzzleTransform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    private bool RaycastCheck(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hitInfo, LayerMask targetLayer)
    {
        if (Physics.Raycast(origin, direction, out hitInfo, maxDistance, targetLayer))
        {
            if (hitInfo.collider.CompareTag("Player") && !Physics.Linecast(origin, hitInfo.point, ObstacleLayers))
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator Relocate()
    {
        yield return new WaitForSeconds(2.0f);
        Vector3 randomDirection = Random.insideUnitSphere * walkpointRange + transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkpointRange, 1);
        walkpoint = hit.position;
        agent.SetDestination(walkpoint);
        Debug.Log("Relocating...");
        ChangeState(EnemyState.Decision);
    }

    public IEnumerator ThrowGrenade()
    {
        if (isGrenadeThrown)
            yield break;

        isGrenadeThrown = true;
        GameObject grenadeInstance = Instantiate(grenade, muzzleTransforms[0].position, Quaternion.identity);
        Debug.Log("Throwing Grenade...");

        yield return new WaitForSeconds(3.0f);

        isGrenadeThrown = false;
        ChangeState(EnemyState.Decision);
    }
}
