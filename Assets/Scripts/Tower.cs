using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : AttackingEntity
{
    public GameObject projectilePrefab;

    float elapsedTime;
    GameObject player;
    GameObject baseVar;
    bool isSpecial = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        baseVar = GameObject.Find("Base");
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (Vector3.Distance(target.transform.position, transform.position) <= range && elapsedTime > rateOfFire)
        {
            FireProjectile();
            elapsedTime = 0f;
        }
    }

    protected override void FireProjectile()
    {
        // Create the projectile
        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileEntity = projectileObject.GetComponent<Projectile>();
        projectileEntity.target = this.target.gameObject;

    }

    protected override void Die()
    {
        if ( hp == 0)
        {
            Destroy(gameObject);          
        }
        if (isSpecial)
        {
            baseVar.GetComponent<Base>().StartRespawn();
        }
       
    }
}
