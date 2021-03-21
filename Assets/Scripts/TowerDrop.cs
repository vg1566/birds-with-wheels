using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDrop : MonoBehaviour
{
	public GameObject target;
	bool respawningTower = false;
	public float speed = 1.0f;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (respawningTower)
		{
			transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
		}
		if(Vector2.Distance(transform.position, target.transform.position) < .05)
		{
			target.SetActive(true);
			Destroy(gameObject);
		}
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			respawningTower = true;
		}
	}
}
