using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected float speed = 0;

    /// <summary>
    /// Subtracts amount from this Entity's HP
    /// </summary>
    /// <param name="amount">The amount of HP to lose</param>
    public void LoseHealth(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            //for visual purposes, makes sure hp never goes below 0
            hp = 0;
            //kills the entity
            Die();
        }
    }
    /// <summary>
    /// Adds amount to this Entity's HP
    /// </summary>
    /// <param name="amount">The amount of HP to gain</param>
    public void GainHealth(int amount)
    {
        hp += amount;
    }
    /// <summary>
    /// Does any actions needed when this Entity dies
    /// (ex. Enemies drop)
    /// </summary>
    protected abstract void Die();
}
