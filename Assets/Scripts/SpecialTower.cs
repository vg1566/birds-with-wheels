﻿using System.Collections;
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

    [SerializeField]
    private Sprite defenseSprite;
    [SerializeField]
    private Sprite offenseSprite;

    private bool spriteSwitched;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Avatar>();
    }

    public void ResolveSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (mode == TowerType.SpecialDefense)
            sr.sprite = defenseSprite;
        else
            sr.sprite = offenseSprite;
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
                //Debug.Log("Add Resources");
                player.numBirds++;
                player.numWheels++;
            }
        }
		if (mode == TowerType.SpecialOffense)
		{
			base.Update();
		}
	}

    /// <summary>
    /// Sends message to Avatar before dying
    /// </summary>
    protected override void Die()
    {
        player.TowerDie();
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
