using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for a base gameobject
// Notes for github merging:
//      Map manager must use base's StartGUI() when special tower is destroyed
public class Base : MonoBehaviour, Entity
{
    public int hp = 4;
    public Vector3 position = Vector3.zero;

    public int HP
    {
        get { return hp; }
    }
    public Vector3 Position
    {
        get { return position; }
    }

    private bool usingGUI = false;
    private string status = "Special Tower";

    public int numBirds = 0;
    public int numWheels = 0;

    public float respawnTimer = 0;
    private float totalRespawnTime = 2;
    public bool respawning = false;

    public GameObject avatarPrefab;
    public GameObject currentAvatar;

    // Start is called before the first frame update
    void Start()
    {
        
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
                currentAvatar.transform.position += new Vector3(0, 0, -3);
            }
        }
    }

    /// <summary>
    /// Lose some amount of health
    /// </summary>
    public void LoseHealth(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Gain some amount of health
    /// </summary>
    public void GainHealth(int amount)
    {
        hp += amount;
    }

    /// <summary>
    /// Ends game (destroys everything)
    /// </summary>
    public void Die()
    {
        foreach (Transform child in transform.parent)
        {
            if(child != transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(gameObject);
        }
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
    /// Starts respawn with the base's resource variables. For use by map manager when special tower is destroyed.
    /// </summary>
    public void startRespawn()
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
    }
}
