using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : AttackingEntity
{
    [SerializeField]
    int hp = 5;
    [SerializeField]
    int attackPower = 1;
    [SerializeField]
    int range = 5;
    [SerializeField]
    float rateOfFire = 1;

    float elapsedTime;
    Vector3 position;
    GameObject target;
    GameObject player;

    public int HP 
    { 
        get { return hp; } 
    }

    public Vector3 Position 
    { 
        get { return position; }
    }

    public int AttackPower
    {
        get { return attackPower; }
    }

    public int Range
    {
        get { return range; }
    }

    public float RateOfFire
    { 
        get { return rateOfFire; } 
    }

    public GameObject Target
    {
        get { return target; }
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //FindTarget();
        FireProjectile();

    }

    public void FireProjectile()
    {
        elapsedTime += Time.deltaTime;
        // fire a projectile when enemy is in range and when fire rate cooldown is up
        if (Vector3.Distance(target.transform.position, position) <= range && elapsedTime > rateOfFire)
        {
            // Create the projectile
            //Projectile newProjectile = Instantiate(projectile, position, Quaternion.identity);
            elapsedTime = 0f;
        }
    }

    public void LoseHealth(int amount)
    {
        hp -= amount;
    }

    public void GainHealth(int amount)
    {
        hp += amount;
    }

    public void Die()
    {
        if ( hp == 0)
        {
            // Destroy is not coming up for some reason?
            //Destroy(gameObject);

            // Adding resources
            //player.numWheels += 1;
            //player.numBirds += 1;

        }
    }
}
