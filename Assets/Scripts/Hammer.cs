using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hammer : MonoBehaviour
{
    [Header("Weapon Parameters")]
    public Collider WeaponCollider;
    public int WeaponDamage;
    public float DamageCooldown;
    public int WeaponEnergyCost;

    [Header("Skill Parameters")]
    public float SkillCooldown;
    public int SkillImpactRange;
    public int SkillEnergyCost;

    [Header("Effects")]
    public ParticleSystem ImpactEffect;
    public ParticleSystem SkillEffect;
    
    public LayerMask TargetLayer;

    private bool canUseSkill = true;
    private bool canDealDamage = true;
    private bool SkillisActive = false;

    private WeaponSoundManager SoundManager;
    private EnemyHealth enemyHealth;
    private Health health;
    

    private void Awake()
    {
        WeaponSoundManager soundManager = GetComponent<WeaponSoundManager>();
        if (soundManager == null)
        {
            Debug.Log("WeaponSoundManager can not be located");
        }

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(ActivateSkill);
        grabbable.deactivated.AddListener(DeactivateSkill);

        GameObject playerGameObject = GameObject.Find("Player");
        if (playerGameObject != null)
        {
            // Get the Health component from the player GameObject
            health = playerGameObject.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
    }

    private void Update()
    {
        if (!canDealDamage)
        {
            StartCoroutine(ReactivateAttackCollider());
        }

        if (!canUseSkill)
        {
            StartCoroutine(ReactivateSkill());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage)
        {
            return;
        }
        if (other.tag == "Enemy")
        {
            // Get the EnemyHealth component from the collider that the hammer collided with
            EnemyHealth enemyhealth = other.GetComponent<EnemyHealth>();

            if (enemyhealth != null)
            {
                enemyhealth.TakeDamage(WeaponDamage);
                IntantiateImpactEffect(other.transform);
                health.DepleteEnergy(WeaponEnergyCost);
                DeactivateAttackCollider();
            }
        }

        if (other.tag == "Ground" && SkillisActive)
        {
            if (canUseSkill)
            {
                Collider[] colliders = Physics.OverlapSphere(other.transform.position, SkillImpactRange, TargetLayer);
                foreach (Collider collider in colliders)
                {
                    EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(WeaponDamage);
                        InstantiateSkillEffect(collider.transform);
                        health.DepleteEnergy(SkillEnergyCost);
                        DeactivateAttackCollider();

                    }
                }
            }
            else if (!canUseSkill)
            {
                WeaponSoundManager soundManager = GetComponent<WeaponSoundManager>();
                if (soundManager != null)
                {
                    soundManager.PlaySkillCooldown();
                }
            }

        }
    }

    private void ActivateSkill(ActivateEventArgs Arg)
    {
        SkillisActive = true;
        WeaponSoundManager soundManager = GetComponent<WeaponSoundManager>();
        if (soundManager != null)
        {
            soundManager.PlaySkillActivate();
        }

    }
    
    private void DeactivateSkill(DeactivateEventArgs Arg)
    {
        SkillisActive = false;
        WeaponSoundManager soundManager = GetComponent<WeaponSoundManager>();
        if (soundManager != null)
        {
            soundManager.PlaySkillDeactivate();
        }
        
    }

    private void IntantiateImpactEffect(Transform collisionTransform)
    {
        if (ImpactEffect != null)
        {
            Instantiate(ImpactEffect, collisionTransform.position, Quaternion.identity);
        }
    }

    private void InstantiateSkillEffect(Transform collisionTransform)
    {
        if (SkillEffect != null)
        {
            Instantiate(SkillEffect, collisionTransform.position, Quaternion.identity);
        }
        WeaponSoundManager soundManager = GetComponent<WeaponSoundManager>();
        if (soundManager != null)
        {
            soundManager.PlaySkillImpact();
        }
        
    }

    private void DeactivateAttackCollider()
    {
        WeaponCollider.enabled = false;
        canDealDamage = false;
    }

    private IEnumerator ReactivateAttackCollider()
    {
        yield return new WaitForSeconds(DamageCooldown);
        canDealDamage = true;
        WeaponCollider.enabled = true;
    }

    private IEnumerator ReactivateSkill()
    {
        yield return new WaitForSeconds(SkillCooldown);
        canUseSkill = true;
    }
}
