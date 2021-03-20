using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Merge Notes:
//      MapManager set player on creation
public class SpecialTower : Tower
{
    [HideInInspector]
    public TowerType mode = TowerType.SpecialOffense;
    public Avatar player;

    private float resourceTimer = 0f;
    private float secondsPerResourceGain = 4f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Avatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == TowerType.SpecialDefense)
        {
            resourceTimer += Time.deltaTime;
            if (resourceTimer > secondsPerResourceGain)
            {
                resourceTimer = 0;
                Debug.Log("Add Resources");
                player.numBirds++;
                player.numWheels++;
            }
        }    
    }

    /// <summary>
    /// Sends message to Avatar before dying
    /// </summary>
    protected override void Die()
    {
        player.GetComponent<Avatar>().TowerDie();
        base.Die();
    }

    /// <summary>
    /// Only fires when in Offense mode
    /// </summary>
    protected override void FireProjectile()
    {
        if(mode == TowerType.SpecialOffense)
        {
            base.FireProjectile();
        }
    }
}
