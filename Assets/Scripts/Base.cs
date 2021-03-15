using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for a base gameobject
// Notes for github merging:
//      Tower must use base's StartRespawn() when special tower is destroyed
public class Base : Entity
{
    public Vector3 position = Vector3.zero;

    private bool usingGUI = false;
    private string status = "Special Tower";

    public int numBirds = 0;
    public int numWheels = 0;

    public float respawnTimer = 0;
    private float totalRespawnTime = 2;
    [HideInInspector]
    public bool respawning = false;
    private bool gameOver = false;

    public GameObject avatarPrefab;
    public GameObject currentAvatar;
    public GameObject mapManager;

    // Start is called before the first frame update
    void Start()
    {
        currentAvatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity);
        currentAvatar.GetComponent<Avatar>().playerBase = gameObject;
        currentAvatar.GetComponent<Avatar>().numBirds += numBirds;
        currentAvatar.GetComponent<Avatar>().numWheels += numWheels;
        currentAvatar.GetComponent<Avatar>().mapManager = mapManager;
        currentAvatar.transform.position += new Vector3(0, 0, -3);
    }

    // Update is called once per frame
    void Update()
    {
        if(respawning) // Respawn timer stuff (only active when respawning)
        {
            respawnTimer += Time.deltaTime;
            if(respawnTimer > totalRespawnTime)
            {
                respawning = false;
                usingGUI = false;
                respawnTimer = 0;
                status = "Special Tower";
                currentAvatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity);
                currentAvatar.GetComponent<Avatar>().playerBase = gameObject;
                currentAvatar.GetComponent<Avatar>().numBirds += numBirds;
                currentAvatar.GetComponent<Avatar>().numWheels += numWheels;
                currentAvatar.GetComponent<Avatar>().mapManager = mapManager;
                currentAvatar.transform.position += new Vector3(0, 0, -3);
            }
        }
    }

    /// <summary>
    /// Ends game (destroys everything and displays game over message)
    /// </summary>
    protected override void Die()
    {
        foreach (Transform child in transform.parent)
        {
            if(child != transform)
            {
                Destroy(child.gameObject);
            }
        }
        usingGUI = false;
        gameOver = true;
    }

    /// <summary>
    /// Starts respawn and transfers resource information.
    /// </summary>
    public void StartRespawn(int birds, int wheels)
    {
        StartGUI(birds, wheels);
        respawning = true;
        status = "Respawning";
    }

    /// <summary>
    /// Starts respawn with the base's resource variables. For use by special tower when special tower is destroyed.
    /// </summary>
    public void StartRespawn()
    {
        StartRespawn(numBirds, numWheels);
    }

    /// <summary>
    /// Updates stats and activates GUI
    /// </summary>
    public void StartGUI(int birds, int wheels)
    {
        numBirds = birds;
        numWheels = wheels;
        usingGUI = true;
    }

    /// <summary>
    /// Shows GUI, if activated
    /// </summary>
    void OnGUI()
    {
        if(usingGUI)
        {
            GUILayout.BeginArea(new Rect(0, 0, 90, 100));
            GUILayout.Box("Birds: " + numBirds);
            GUILayout.Box("Wheels: " + numWheels);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, (Screen.height - 50), 200, 50));
            GUILayout.Box("Status: " + status);
            if(respawning)
            {
                GUILayout.Box("Timer: " + (totalRespawnTime - respawnTimer));
            }
            GUILayout.EndArea();
        }
        if(gameOver) // Game over message
        {
            GUILayout.BeginArea(new Rect((Screen.width/2 - 45), (Screen.height/2 - 50), 90, 100));
            GUILayout.Box("Game Over!");
            GUILayout.EndArea();
        }
    }
}
