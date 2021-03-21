using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDetection : MonoBehaviour
{
    [SerializeField]
    private float range = 5f;

    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private Transform parent;

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = range;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // If the player is within the detection circle, 
        // move the drop closer to them.
        if (collision.tag == "Player")
        {
            parent.position =
                Vector3.MoveTowards(
                    parent.position,
                    collision.transform.position,
                    speed * Time.deltaTime);
        }
    }
}
