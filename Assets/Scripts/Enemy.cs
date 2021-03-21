using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AttackingEntity
{	
	public float birdDropRate;
	public float wheelDropRate;

	public GameObject projectilePrefab;

	public GameObject dropPrefab;

	public MapManager mapManager;

	public float X
	{
		get{ return this.transform.position.x; }
		set{this.transform.position = new Vector3(value, this.transform.position.y);}
	}
	public float Y
	{
		get { return this.transform.position.y; }
		set { this.transform.position = new Vector3(this.transform.position.x, value); }
	}

	float lifetime = 0;

	/// <summary>
	/// Kills the enemy. Drops necessary drops and destroys the gameobject.
	/// </summary>
	protected override void Die()
	{
        GameObject drop = Instantiate(dropPrefab);
		drop.transform.position = transform.position;
		drop = drop.transform.GetChild(0).gameObject; // where the drop script is
		drop.GetComponent<Drops>().SetDropValues(Random.value < birdDropRate ? 1 : 0, Random.value < wheelDropRate ? 1 : 0);

		Destroy(gameObject);
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
		// TODO: once mapmanager is sorted out
		//sortedTargets.Insert(0, mapManager.avatar);
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
	protected override void FireProjectile()
	{
		GameObject projectileObject = Instantiate(projectilePrefab, new Vector3(this.X, this.Y), Quaternion.identity);
		Projectile projectileEntity = projectileObject.GetComponent<Projectile>();
		projectileEntity.damage = attackPower;
		projectileEntity.target = this.target.gameObject;
	}
	
	/// <summary>
	/// Moves the enemy left across the screen by its speed
	/// needs to have pathing implemented.
	/// </summary>
	public void Move()
	{
		//TODO
		//goes left across the screen
		transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
	}

	// Start is called before the first frame update
	void Start()
    {
		lifetime = Time.time;
    }

	// Update is called once per frame
	private float elapsedTime;
    void Update()
    {
		//moves
		Move();

		elapsedTime += Time.deltaTime;
		if (target != null && elapsedTime > rateOfFire)
		{
			FireProjectile();
			elapsedTime = 0f;
		}
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
		Base baseScript = collision.GetComponent<Base>();
		if (baseScript != null)
        {
			baseScript.LoseHealth(1);
			Destroy(gameObject);
        }
    }
}
