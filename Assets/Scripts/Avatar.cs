using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Basic,
    SpecialOffense,
    SpecialDefense,
    Buff,
    Angry,
    Vigilant
}

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for an avatar gameobject
// Merge Notes:
//      Map manager needs to work with SpecialOffense and SpecialDefense TowerTypes
public class Avatar : Entity
{
    public Vector3 position = Vector3.zero;

    public bool isDead = false;
    public bool isTower = false;
    private string status = "Error";

    public GameObject specialTower;
    public GameObject playerBase;
    public GameObject mapManager;

    [HideInInspector]
    public int numWheels = 1;
    [HideInInspector]
    public int numBirds = 1;

    public float respawnTimer = 0;
    private float totalRespawnTime = 2;
    [HideInInspector]
    public bool respawning = false;

    // TODO: swap to dictionary
    // Stores costs of tower types at the index of the enum value. e.g. a tower.Basic tower costs birdCosts[tower.Basic] birds and wheelCosts[tower.Basic] wheels
    public int[] birdcosts = { 1, 0 };
    public int[] wheelCosts = { 1, 0 };

    public struct TowerCost
    {
        public int birds;
        public int wheels;

        public TowerCost(int birds, int wheels)
        {
            this.birds = birds;
            this.wheels = wheels;
        }
    }

    public Dictionary<TowerType, TowerCost> prices = new Dictionary<TowerType, TowerCost>
    {
        { TowerType.SpecialOffense, new TowerCost(birds: 0, wheels: 0) },
        { TowerType.SpecialDefense, new TowerCost(birds: 0, wheels: 0) },
        { TowerType.Basic, new TowerCost(birds: 1, wheels: 1) },
        { TowerType.Buff, new TowerCost(birds: 2, wheels: 1) },
        { TowerType.Angry, new TowerCost(birds: 1, wheels: 2) },
        { TowerType.Vigilant, new TowerCost(birds: 2, wheels: 2) },

    };

    // Start is called before the first frame update
    void Start()
    {
        position = playerBase.transform.position;
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead && !isTower)
        {
            Move();
        }
        if(respawning)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > totalRespawnTime)
            {
                respawning = false;
                respawnTimer = 0;
                status = "Special Tower";
                position = playerBase.transform.position;
                position += new Vector3(0, 0, -3);
                transform.position = position;
                gameObject.GetComponent<Renderer>().enabled = true;
                isDead = false;
            }
        }
    }

    /// <summary>
    /// Tells the base it needs to respawn and deletes itself. The base gets resource amounts to keep track of
    /// </summary>
    protected override void Die()
    {
        isDead = true;
        gameObject.GetComponent<Renderer>().enabled = false;
        respawning = true;
        status = "Respawning";
    }

    /// <summary>
    /// For use when tower is destroyed
    /// </summary>
    public void TowerDie()
    {
        isTower = false;
        Die();
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
        if (numBirds >= prices[towerType].birds && numWheels >= prices[towerType].wheels)
        {
            if(mapManager.GetComponent<MapManager>().PlaceTower(position, towerType))
            {
                numBirds -= prices[towerType].birds;
                numWheels -= prices[towerType].wheels;
            }
        }
    }


    /// <summary>
    /// Creates special tower, tells base to use base GUI, and destroys self
    /// </summary>
    public void TransformIntoTower(TowerType mode)
    {
        if (mapManager.GetComponent<MapManager>().PlaceTower(position, mode))
        {
            isTower = true;
            gameObject.GetComponent<Renderer>().enabled = false;
            status = "Special Tower";
        }
    }

    /// <summary>
    /// Sends position to map manager to destroy appropriate towers (any touching)
    /// </summary>
    public void BreakTower()
    {
        TowerType towerType = mapManager.GetComponent<MapManager>().RemoveTower(position);
        numBirds += prices[towerType].birds;
        numWheels += prices[towerType].wheels;
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

        if(isDead || isTower)
        {
            GUILayout.BeginArea(new Rect(0, (Screen.height - 50), 200, 50));
            GUILayout.Box("Status: " + status);
            if (respawning)
            {
                GUILayout.Box("Timer: " + (totalRespawnTime - respawnTimer));
            }
            GUILayout.EndArea();
        }
        else if(!isDead && !isTower)
        {
            GUILayout.BeginArea(new Rect(0, (Screen.height - 150), 200, 150));
            if (GUILayout.Button("Break nearest tower"))
            {
                BreakTower();
            }
            if (GUILayout.Button("Transform into defensive tower"))
            {
                TransformIntoTower(TowerType.SpecialDefense);
            }
            if (GUILayout.Button("Transform into offensive tower"))
            {
                TransformIntoTower(TowerType.SpecialOffense);
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
}
