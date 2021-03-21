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
	string targetType;

	// Start is called before the first frame update
	void Start()
    {
		targetType = target.GetComponent<Enemy>() != null ? "enemy" : "friend";
	}

    // Update is called once per frame
    void Update()
    {
		if(target == null)
        {
			Destroy(gameObject);
			return;
        }

		//get target position
		targetPos = target != null ? target.transform.position : new Vector3();
		//point at target
		direction = (targetPos - this.transform.position).normalized;

		Debug.DrawLine(transform.position, direction * speed);
		//move towards target
		transform.Translate(direction * speed * Time.deltaTime);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Entity hitObject = collision.gameObject.GetComponent<Entity>();
		string hitObjectType = collision.GetComponent<Enemy>() != null ? "enemy" : "friend";
		//makes sure it can collide with something that can take damage (i.e. not another projectile)
		if (hitObject != null && hitObjectType.Equals(targetType))
		{
			hitObject.LoseHealth(damage);
			Destroy(this.gameObject);
		}
	}
}
