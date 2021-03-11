using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AttackingEntity : Entity
{
    int AttackPower { get; }
    /// <summary>
    /// Range in tiles
    /// </summary>
    int Range { get; }
    /// <summary>
    /// Projectiles per second
    /// </summary>
    float RateOfFire { get; }
    /// <summary>
    /// What this entity is shooting at
    /// </summary>
    GameObject Target { get; }

    /// <summary>
    /// Chooses a target based on proximity (for attacking towers/enemies) 
    /// or priority (enemies prioritize the avatar)
    /// </summary>
    /// <param name="possibleTargets">List of possible targets</param>
    void FindTarget(List<Entity> possibleTargets);
    /// <summary>
    /// Fires a projectile from this entity's position towards Target
    /// </summary>
    void FireProjectile();
}
