using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AttackingEntity
{	
	public float birdDropRate;
	public float wheelDropRate;

	public GameObject projectilePrefab;

	public GameObject dropPrefab;

	int[,] map;
	public int[,] traversed;
	int mapWidth;
	int mapHeight;
	int pathInt = 1;
	public Vector2 basePosition;
	public Vector2 targetPoint;
	//DELETE
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
	protected float elapsedTime;

	float lifetime = 0;

	/// <summary>
	/// Kills the enemy. Drops necessary drops and destroys the gameobject.
	/// </summary>
	protected override void Die()
	{
        var drop = Instantiate(dropPrefab);
		drop.transform.position = transform.position;
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
	/// Sets the map of the enemy to the given map so the enemy can find the path.
	/// </summary>
	/// <param name="map">the map the enemy will use to navigate</param>
	public void SetMap(int[,] map)
	{
		this.map = map;
		//gets starting position so targetpoint isn't empty
		targetPoint = new Vector2(transform.position.x, transform.position.y);

		//nothing makes sense anymore
		mapHeight = map.GetLength(0);
		mapWidth = map.Length / mapHeight;

		//creates and populates traversed
		traversed = new int[mapHeight, mapWidth];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				traversed[y, x] = 0;
				//also gets the base's position
				if (map[y, x] == 2)
				{
					basePosition.x = x; 
					basePosition.y = y; 
				}
			}
		}
	}

	/// <summary>
	/// Moves the enemy left across the screen by its speed
	/// needs to have pathing implemented.
	/// </summary>
	public void Move()
	{
		//exits if there is no map.
		if (map == null) return;

		List<Vector2> possibleTiles = new List<Vector2>();
		//TODO
		//goes left across the screen
		//transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
		//{ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		//{ 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		//{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
		//{ 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		//{ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
		//columns = x, rows = y
		//[y,x]; rc race car
		//why is everything the wrong way. :(

		//gets current position in map
		int wholeX = (int)Mathf.Round(X);
		int wholeY = (int)Mathf.Round(Y);
		//float xDecimal = X - wholeX;
		//float yDecimal = Y - wholeY;

		float distanceFromBase = Vector2.Distance(basePosition, transform.position);
		float distanceToNextPoint = Vector2.Distance(targetPoint, transform.position);

		//if enemey is at the target tile or will pass the target tile, 
		//count that tile as traversed
		if (distanceToNextPoint < .05)//speed * Time.deltaTime)
		{
			traversed[wholeY, wholeX] = 1;

			//find possible tiles:
			//left
			if (wholeX > 0 && traversed[wholeY, wholeX - 1] == 0 && map[wholeY, wholeX - 1] == 1)
				possibleTiles.Add(new Vector2(wholeX - 1, wholeY));
			//right
			if (wholeX < mapWidth && traversed[wholeY, wholeX + 1] == 0 && map[wholeY, wholeX + 1] == 1)
				possibleTiles.Add(new Vector2(wholeX + 1, wholeY));
			//up, +/- are reversed because map is upside down
			if (wholeY > 0 && traversed[wholeY - 1, wholeX] == 0 && map[wholeY - 1, wholeX] == 1)
				possibleTiles.Add(new Vector2(wholeX, wholeY - 1));
			//down, +/- are reversed because map is upside down
			if (wholeY < mapHeight - 1 && traversed[wholeY + 1, wholeX] == 0 && map[wholeY + 1, wholeX] == 1)
				possibleTiles.Add(new Vector2(wholeX, wholeY + 1));


			//finds nearest tile that brings enemy closer to the base  
			float shortestPtBaseDistance = distanceFromBase * 2;
			foreach (Vector2 pt in possibleTiles)
			{
				float ptBaseDistance = Vector2.Distance(pt, basePosition);
				if (ptBaseDistance < shortestPtBaseDistance)
				{
					shortestPtBaseDistance = ptBaseDistance;
					targetPoint = pt;

				}
			}
		}
		Vector2 dir = targetPoint - new Vector2(transform.position.x, transform.position.y);
		dir.Normalize();
		transform.Translate(dir * speed * Time.deltaTime);

	}

	// Start is called before the first frame update
	void Start()
    {
		lifetime = Time.time;
		targetPoint = new Vector2(transform.position.x, transform.position.y);

	}

	// Update is called once per frame
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
