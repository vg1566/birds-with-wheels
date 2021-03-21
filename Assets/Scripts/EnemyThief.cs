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
	[SerializeField]
	public GameObject towerDrop;

	//private float elapsedTime;

	// Start is called before the first frame update
	void Start()
	{
		towers = new List<GameObject>();
	}

	protected override void FireProjectile()
	{
		if (target && Random.value < stealChance && towers.Count <= carryLimit && target.GetComponent<SpecialTower>() == null)
		{
			//mapman.RemoveTower(target.transform.position);
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
			//tower.gameObject.SetActive(true);
			//tower.GetComponent<Tower>().canFire = false;
			//tower.transform.position = this.transform.position;
			GameObject drop = Instantiate(towerDrop, transform.position, Quaternion.identity);

			drop.GetComponent<TowerDrop>().target = tower;

			//drop special drop for towers
			//towerdrop.target = tower
			//in towerdrop:
			//move towards target.position
			//when at target (or close to target)
			//target.setActive = true
		}
		Destroy(gameObject);

		//base.Die();
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Base baseScript = collision.GetComponent<Base>();
		if (baseScript != null)
		{
			baseScript.LoseHealth(1);
			Die();
		}
	}
}
