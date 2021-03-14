using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed;
	public int damage;
	public GameObject target;
	Vector3 targetPos;
	Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//get target position
		targetPos = target.transform.position;
		//point at target
		direction = (targetPos - this.transform.position).normalized;
		//move towards target
		transform.Translate(direction * speed);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Entity hitObject = collision.gameObject.GetComponent<Entity>();
		//makes sure it can collide with something that can take damage (i.e. not another projectile)
		if (hitObject != null)
		{
			hitObject.LoseHealth(damage);
			Destroy(this.gameObject);
		}
	}
}
