using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Basic,
    Special
}

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for an avatar gameobject
public class Avatar : Entity
{
    public Vector3 position = Vector3.zero;

    public GameObject specialTower;
    public GameObject playerBase;
    public GameObject mapManager;
    public int numWheels = 0;
    public int numBirds = 0;

    // TODO: swap to dictionary
    // Stores costs of tower types at the index of the enum value. e.g. a tower.Basic tower costs birdCosts[tower.Basic] birds and wheelCosts[tower.Basic] wheels
    public int[] birdcosts = { 1, 0 };
    public int[] wheelCosts = { 1, 0 };

    public struct TowerCost
    {
        int birds;
        int wheels;

        public TowerCost(int birds, int wheels)
        {
            this.birds = birds;
            this.wheels = wheels;
        }
    }

    public Dictionary<TowerType, TowerCost> prices = new Dictionary<TowerType, TowerCost>
    {
        { TowerType.Special, new TowerCost(birds: 0, wheels: 0) },
        { TowerType.Basic, new TowerCost(birds: 1, wheels: 1) }
    };

    // Start is called before the first frame update
    void Start()
    {
        position = playerBase.transform.position;
        transform.position = position;
        Debug.Log(wheelCosts.Length);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /// <summary>
    /// Tells the base it needs to respawn and deletes itself. The base gets resource amounts to keep track of
    /// </summary>
    protected override void Die()
    {
        playerBase.GetComponent<Base>().StartRespawn(numBirds, numWheels);
        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the game object in the direction of the mouse keys at speed * Time.deltaTime
    /// </summary>
    public void Move()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }
        position += direction.normalized * speed * Time.deltaTime;
        transform.position = position;
    }

    /// <summary>
    /// Checks for enough resources, then subtracts the relevant amount, and sends the tower type and position to the map manager
    /// </summary>
    public void PlaceTower(TowerType towerType)
    {
        int intTower = (int)towerType;
        if (numBirds >= birdcosts[intTower] && numWheels >= wheelCosts[intTower])
        {
            numBirds -= birdcosts[intTower];
            numWheels -= wheelCosts[intTower];
            mapManager.GetComponent<MapManager>().PlaceTower(position, towerType);
        }
    }


    /// <summary>
    /// Creates special tower, tells base to use base GUI, and destroys self
    /// </summary>
    public void TransformIntoTower()
    {
        PlaceTower(TowerType.Special);
        playerBase.GetComponent<Base>().StartGUI(numBirds, numWheels);
        Destroy(gameObject);
    }

    /// <summary>
    /// Sends position to map manager to destroy appropriate towers (any touching)
    /// </summary>
    public void BreakTower()
    {
        mapManager.GetComponent<MapManager>().RemoveTower(position);
    }

    /// <summary>
    /// Displays stats and tower placement buttons
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 90, 100));
        GUILayout.Box("Birds: " + numBirds);
        GUILayout.Box("Wheels: " + numWheels);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0, (Screen.height - 120), 200, 120));
        if (GUILayout.Button("Break nearest tower"))
        {
            BreakTower();
        }
        if (GUILayout.Button("Transform into special tower"))
        {
            TransformIntoTower();
        }
        if (GUILayout.Button("Place basic tower"))
        {
            PlaceTower(TowerType.Basic);
        }
        if (GUILayout.Button("Lose Health"))
        {
            LoseHealth(1);
        }
        if (GUILayout.Button("Gain Resources"))
        {
            numBirds++;
            numWheels++;
        }
        GUILayout.EndArea();
    }
}
