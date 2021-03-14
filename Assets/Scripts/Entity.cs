using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Entity
{
	GameObject EntityObject { get; }

	int HP { get; }
    /// <summary>
    /// Returns the transform.position of the Entity
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Subtracts amount from this Entity's HP
    /// </summary>
    /// <param name="amount">The amount of HP to lose</param>
    void LoseHealth(int amount);
    /// <summary>
    /// Adds amount to this Entity's HP
    /// </summary>
    /// <param name="amount">The amount of HP to gain</param>
    void GainHealth(int amount);
    /// <summary>
    /// Does any actions needed when this Entity dies
    /// (ex. Enemies drop)
    /// </summary>
    void Die();
}
