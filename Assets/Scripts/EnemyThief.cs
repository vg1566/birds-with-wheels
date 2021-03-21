using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThief : Enemy
{
	List<GameObject> towers;
	[SerializeField]
	public float stealChance = .5f;
	[SerializeField]
	public int carryLimit = 3;

	//private float elapsedTime;

	// Start is called before the first frame update
	void Start()
	{
		towers = new List<GameObject>();
	}

	protected override void FireProjectile()
	{
		if(Random.value < stealChance && towers.Count <= carryLimit)
		{
			towers.Add(target.gameObject);
			target.gameObject.SetActive(false);
		}
	}

    // Update is called once per frame
    void Update()
    {
		Move();
		elapsedTime += Time.deltaTime;
		if (target != null && elapsedTime > rateOfFire)
		{
			FireProjectile();
			elapsedTime = 0f;
		}
	}
	protected override void Die()
	{
		foreach (GameObject tower in towers)
		{
			tower.gameObject.SetActive(true);
			tower.GetComponent<Tower>().canFire = false;
			tower.transform.position = this.transform.position;
		}
		Destroy(gameObject);

		//base.Die();
	}
}
