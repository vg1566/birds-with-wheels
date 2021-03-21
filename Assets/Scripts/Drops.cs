using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drops : MonoBehaviour
{
    private int birdValue = 0;
    private int wheelValue = 0;

    public void SetDropValues(int bird, int wheel)
    {
        birdValue = bird;
        wheelValue = wheel;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the drop itself collides with the player, the player picks up the drop
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Avatar>().numBirds += birdValue;
            other.gameObject.GetComponent<Avatar>().numWheels += wheelValue;
            Destroy(gameObject);
        }
    }
}
