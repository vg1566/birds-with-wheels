using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for a base gameobject
// Notes for github merging:
//      Special Tower must use base's StartRespawn() when special tower is destroyed
//      Tower must remove StartRespawn
public class Base : Entity
{
    public Vector3 position = Vector3.zero;

    public bool gameOver = false;

    public GameObject avatarPrefab;
    public GameObject currentAvatar;
    public GameObject mapManager;

    // Start is called before the first frame update
    void Start()
    {
        currentAvatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity);
        currentAvatar.GetComponent<Avatar>().playerBase = gameObject;
        currentAvatar.GetComponent<Avatar>().mapManager = mapManager;
        currentAvatar.transform.position += new Vector3(0, 0, -3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Ends game (destroys everything and displays game over message)
    /// </summary>
    protected override void Die()
    {
        gameOver = true;
    }
}
