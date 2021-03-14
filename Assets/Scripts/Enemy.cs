using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AttackingEntity
{	
	public int hp;
	public int attackPower;
	public int rateOfFire;
	public int range;
	public int speed;

	public float birdDropRate;
	public float wheelDropRate;

	public GameObject projectilePrefab;

	//TODO FIXME might be better to be match the type of the List sent in FindTarget.
	GameObject target;

	Vector3 position;

	//attackingEntity properites
	public  int AttackPower
	{
		get { return 1; }
	}
	public float RateOfFire
	{
		get { return 1; }
	}
	public int Range
	{
		get { return 1; }
	}
	public GameObject Target
	{
		get { return null; }
		set { target = value; }
	}
	public GameObject EntityObject
	{
		get
		{
			return this.gameObject;
		}
	}

	//movingEntity properties
	public int Speed
	{
		get { return 1; }
	}

	//entity properties
	public int HP
	{
		get {return 1;}
		set{hp = value;}
	}

	public Vector3 Position
	{
		get {return this.transform.position;}
		set { position = value; }
	}

	public float X
	{
		get{ return this.transform.position.x; }
		set{this.transform.position = new Vector3(value, this.transform.position.y);}
	}
	public float Y
	{
		get { return this.transform.position.x; }
		set { this.transform.position = new Vector3(this.transform.position.x, value); }
	}

	float lifetime = 0;

	/// <summary>
	/// Reduces the enemy's health. Kills the enemy if hp is lower than 0.
	/// </summary>
	/// <param name="amt">The amount of health to reduce by</param>
	public void LoseHealth(int amt)
	{
		hp -= amt;
		if(hp <= 0)
		{
			//for visual purposes, makes sure hp never goes below 0
			hp = 0;
			//kills the entity
			Die();
		}
	}

	/// <summary>
	/// Increases the enemy's hp
	/// </summary>
	/// <param name="amt">The amount to increase its health by</param>
	public void GainHealth(int amt)
	{
		hp += amt;
	}

	/// <summary>
	/// Kills the enemy. Drops necessary drops and destroys the gameobject.
	/// </summary>
	public void Die()
	{
		if(Random.value > birdDropRate)
		{
			//drop a birb
			//Instantiate(BirdDropPrefab, new Vector3(this.X, this.Y));
		}
		if (Random.value > wheelDropRate)
		{
			//drop a wheel
			//Instantiate(WheelDropPrefab, new Vector3(this.X, this.Y));
		}
		Destroy(this.gameObject);
	}

	/// TODO fixme list type has been changed to gameobject to match
	/// <summary>
	/// Finds a target tower. Must be called from MapManager.
	/// </summary>
	/// <param name="targets">An array of all towers on the map</param>
	public override void FindTarget(List<Entity> targets)
	{
		// sort
		List<Entity> sortedTargets = targets;
		sortedTargets.Sort(SortByDistanceToTarget);
		base.FindTarget(sortedTargets);

		int SortByDistanceToTarget(Entity e1, Entity e2)
		{
			return 
				(int)(Vector3.Distance(e1.transform.position, transform.position)
					- Vector3.Distance(e2.transform.position, transform.position));
		}
	}
	
	/// <summary>
	/// Fires a projectile at this enemy's target.
	/// </summary>
	public void FireProjectile()
	{
		GameObject projectileObject = Instantiate(projectilePrefab, new Vector3(this.X, this.Y), Quaternion.identity);
		Projectile projectileEntity = projectileObject.GetComponent<Projectile>();
		projectileEntity.target = this.target;
	}
	
	/// <summary>
	/// Moves the enemy left across the screen by its speed
	/// needs to have pathing implemented.
	/// </summary>
	public void Move()
	{
		//TODO
		//goes left across the screen
		transform.Translate(new Vector3(1, 0, 0) * speed);
	}

	// Start is called before the first frame update
	void Start()
    {
		lifetime = Time.time;
    }
	
    // Update is called once per frame
    void Update()
    {
		//gets current time
		lifetime = Time.time;
		//moves
		Move();
		//if it is time to shoot and a target exists
		if(Mathf.Floor(lifetime % rateOfFire) == 0 && target != null) //&& target.isdead)
		{
			//shoot
			FireProjectile();
		}
	}
}
