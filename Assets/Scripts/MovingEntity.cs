using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MovingEntity : Entity
{
    int Speed { get; }

    /// <summary>
    /// Moves this entity's position
    /// </summary>
    void Move();
}
