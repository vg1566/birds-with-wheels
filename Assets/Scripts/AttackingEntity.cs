using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackingEntity : Entity
{
    [SerializeField]
    protected int attackPower;
    /// <summary>
    /// Range in tiles
    /// </summary>
    [SerializeField]
    protected int range;
    /// <summary>
    /// Projectiles per second
    /// </summary>
    [SerializeField]
    protected float rateOfFire;
    /// <summary>
    /// What this entity is shooting at
    /// </summary>
    protected Entity target;

    /// <summary>
    /// Chooses a target based on proximity (for attacking towers/enemies) 
    /// or priority (enemies prioritize the avatar)
    /// </summary>
    /// <param name="possibleTargets">List of possible targets</param>
    public virtual void FindTarget(List<Entity> possibleTargets)
    {
        Entity possibleTarget = null;
        target = null;

        for (int i = 0; i < possibleTargets.Count; i++)
        {
            possibleTarget = possibleTargets[i];
            if (possibleTarget == null) continue;
            float distToTarget = Vector3.Distance(possibleTarget.transform.position, transform.position);

            // If the target is in range, set and break
            if (distToTarget <= range)
            {
                target = possibleTarget;
                break;
            }
        }
    }
    /// <summary>
    /// Fires a projectile from this entity's position towards Target
    /// </summary>
    protected abstract void FireProjectile();
}
