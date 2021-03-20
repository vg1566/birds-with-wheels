using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThief : Enemy
{
	GameObject tower;
	float stealChance = .5f;

	protected override void FireProjectile()
	{
		if(Random.value < stealChance)
		{
			tower = target.gameObject;
			//target;
		}
		//delete gameobject or disable gameobject?
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//tower must follow the 
		if (tower != null)
			tower.transform.position = this.transform.position;

    }
	protected override void Die()
	{

		//base.Die();
	}
}
