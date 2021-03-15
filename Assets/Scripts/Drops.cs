using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drops : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float attractRange = 5f;
    
    private int birdValue = 0;
    private int wheelValue = 0;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
    }

    public void SetDropValues(int bird, int wheel)
    {
        birdValue = bird;
        wheelValue = wheel;
    }

    private void MoveToPlayer()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= attractRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Avatar>().numBirds += birdValue;
            player.GetComponent<Avatar>().numWheels += wheelValue;
            Destroy(gameObject);
        }
    }

}
