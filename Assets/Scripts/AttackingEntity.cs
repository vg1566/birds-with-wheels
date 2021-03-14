using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackingEntity : Entity
{

    private int attackPower;
    /// <summary>
    /// Range in tiles
    /// </summary>
    private int range;
    /// <summary>
    /// Projectiles per second
    /// </summary>
    private float rateOfFire;
    /// <summary>
    /// What this entity is shooting at
    /// </summary>
    private Entity target;

    /// <summary>
    /// Chooses a target based on proximity (for attacking towers/enemies) 
    /// or priority (enemies prioritize the avatar)
    /// </summary>
    /// <param name="possibleTargets">List of possible targets</param>
    public virtual void FindTarget(List<Entity> possibleTargets)
    {
        Entity closest = null;

        for (int i = 0; i < possibleTargets.Count; i++)
        {
            float distToTarget = Vector3.Distance(target.transform.position, transform.position);

            // If the target is in range, set and break
            if (distToTarget <= range)
            {
                target = possibleTargets[i];
                break;
            }
        }

        target = closest;
    }
    /// <summary>
    /// Fires a projectile from this entity's position towards Target
    /// </summary>
    protected abstract void FireProjectile();
}
