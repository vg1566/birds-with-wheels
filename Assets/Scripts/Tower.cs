using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : AttackingEntity
{
    public GameObject projectilePrefab;

    float elapsedTime;
    GameObject player;
    GameObject baseVar;
	//DELETE
    public bool isSpecial = false;
	public bool canFire = true;

    MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
		//DELETE
		baseVar = GameObject.Find("Base(Clone) at index 2, 13");
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (target != null && elapsedTime > rateOfFire && canFire)
        {
            FireProjectile();
            elapsedTime = 0f;
        }
    }

    public void SetMapManager(MapManager mm)
    {
        mapManager = mm;
    }

    protected override void FireProjectile()
    {
        // Create the projectile
        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileEntity = projectileObject.GetComponent<Projectile>();
        projectileEntity.damage = attackPower;
        projectileEntity.target = this.target.gameObject;

    }

    protected override void Die()
    {
		//DELETE
        if (isSpecial)
        {
            baseVar.GetComponent<Base>().StartRespawn();
        }

        mapManager.RemoveTower(transform.position);
    }
}
