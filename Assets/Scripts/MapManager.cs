using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Anything that can fill a space on the map. For now, that's:
///   <list type="number">
///     <item><see cref="MapTiles.Empty"/> - Nothing, empty space</item>
///     <item><see cref="MapTiles.Path"/> - Navigable path enemies can travel on.</item>
///     <item><see cref="MapTiles.Base"/> - The player's base they're protecting</item>
///     <item><see cref="MapTiles.EnemyStart"/>Where enemies are spawned in the map. 
///           Appearance-wise, this is just a normal path tile.</item>
///   </list>
/// </summary>
public enum MapTiles
{
    Empty = 0,
    Path = 1,
    Base = 2,

    EnemyStart = 99
}

public enum EnemyType
{
	Basic,
	Buff,
	Speedy,
	Thief
}

/// <summary>
///   <para>
///     MapManager manages the map :)
///   </para>
/// 
///   <para>
///     This includes:
///     <list type="bullet">
///       <item>Generating the map using <see cref="GenerateMap"/></item>
///       <item>Spawning enemies from <see cref="currentWaveEnemies"/> using <see cref="SpawnEnemy"/> and <see cref="secondsPerEnemy"/></item>
///       <item>Creating new waves using <see cref="CreateWave(int, int)"/> and <see cref="StartWave"/></item>
///       <item>Timing spawning of new waves using <see cref="secondsBetweenWaves"/> and <see cref="WaitThenSpawnNextWave"/></item>
///       <item>Keeping track of the layout of the map in <see cref="mapOutline"/></item>
///       <item>Keeping track of towers on the map using <see cref="towerGrid"/></item>
///       <item>Handling and validating tower placement in <see cref="PlaceTower(int, int, TowerType)"/></item>
///       <item>Hendling and validating removal of towers in <see cref="RemoveTower(int, int, bool)"/></item>
///     </list>
///   </para>
/// </summary>
public class MapManager : MonoBehaviour
{
    // All the towers in the scene
    private List<Entity> towers;
    // What wave we're on
    public int currentWave = 1;
    // The enemies that will be spawned into the level this wave
    private Queue<Enemy> currentWaveEnemies;
    // The enemies that are already spawned into the level
    // TODO: assess if we need this
    private List<Entity> enemiesOnScreen;
    // The player base, need to pass to Tower
    private GameObject playerBase;
    // Parent Transform to any created towers
    private Transform towerContainer;
    // Where the enemy spawns
    private Vector3 enemySpawnPosition;

    [SerializeField]
    private GameObject basicEnemy;
	[SerializeField]
	private GameObject buffEnemy;
	[SerializeField]
	private GameObject thiefEnemy;
	[SerializeField]
	//private GameObject buffEnemy;
	// Tower prefab struct will be visible in inspector
	[Serializable]
    public struct TowerPrefab
    {
        public TowerType towerType;
        public GameObject towerGameObject;
    }

    [SerializeField]
    [Tooltip("This array of structs will be turned into a dictionary of " +
        "<TowerType, GameObject> and will be used for placing towers.")]
    private TowerPrefab[] towerPrefabs;

    private Dictionary<TowerType, GameObject> towerPrefabDictionary;

	[SerializeField]
	private EnemyPrefab[] enemyPrefabs;

	private Dictionary<EnemyType, GameObject> enemyPrefabDictionary;

	// Prefabs used to generate the map from the below array
	[SerializeField]
    [Tooltip("The tile that will be used as empty space")]
    private GameObject emptyTile;
    [SerializeField]
    [Tooltip("The tile that will be used as a path")]
    private GameObject pathTile;
    [SerializeField]
    [Tooltip("The tile that will be used as the base")]
    private GameObject baseTile;

    [SerializeField]
    [Tooltip("How many seconds pass before " +
        "each subsequent enemy spawns during a wave.")]
    private float secondsPerEnemy = 0.5f;

    [SerializeField]
    private float secondsBetweenWaves = 10;

    // The map, represented as ints based on the MapSquares enum
    private struct TowerGridSpace
    {
        public TowerType? type;
        public GameObject tower;

        public TowerGridSpace(TowerType? towerType, GameObject towerGameObject)
        {
            type = towerType;
            tower = towerGameObject;
        }
    }
	
	[Serializable]
	public struct EnemyPrefab
	{
		public EnemyType enemyType;
		public GameObject enemyGameObject;
	}

	private TowerGridSpace[,] towerGrid;

    // This is how the map will be generated
    private readonly int[,] mapOutline =
    {
            { 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2 },
            { 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 }
    };

    // Start is called before the first frame update
    void Start()
    {
		Time.timeScale = 1f;

		// If there are no prefabs, don't start the game.
		if (towerPrefabs == null || towerPrefabs.Length == 0)
        {
            throw new InvalidOperationException("Cannot begin the game with no tower prefabs.");
        }

		// Populate tower and enemy prefab dictionary with all the tower and enemy 
		//prefabs created in the inspector.
        towerPrefabDictionary = new Dictionary<TowerType, GameObject>();
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            // Prefab to add
            TowerPrefab pta = towerPrefabs[i];
            towerPrefabDictionary.Add(pta.towerType, pta.towerGameObject);
        }

		enemyPrefabDictionary = new Dictionary<EnemyType, GameObject>();
		for (int i = 0; i < enemyPrefabs.Length; i++)
		{
			// Prefab to add
			EnemyPrefab pta = enemyPrefabs[i];
			enemyPrefabDictionary.Add(pta.enemyType, pta.enemyGameObject);
		}

		towerContainer = new GameObject("Towers").transform;

        towers = new List<Entity>();
        enemiesOnScreen = new List<Entity>();
        currentWaveEnemies = new Queue<Enemy>();

        // Generate the map based on mapOutline
        towerGrid = new TowerGridSpace[mapOutline.GetLength(0), mapOutline.GetLength(1)];
        GenerateMap();

        // Center the camera to the map
        CenterCamera();

        // Start the game
        StartWave();
    }

    private void Update()
    {
        // Find a target for all towers and enemies
        for (int i = 0; i < towers.Count; i++)
        {
			((Tower)towers[i]).FindTarget(enemiesOnScreen);
        }
        for (int i = 0; i < enemiesOnScreen.Count; i++)
        {
            // Make sure the enemy is alive before finding a target
            if (((Enemy)enemiesOnScreen[i]) == null)
            {
                enemiesOnScreen.RemoveAt(i);
                i--;
                continue;
            } 
            ((Enemy)enemiesOnScreen[i]).FindTarget(towers.Where(t => t.gameObject.activeInHierarchy).ToList());
        }
    }

    // ------------------------------------
    // --------  HELPER FUNCTIONS  --------
    // ------------------------------------

    private void CenterCamera()
    {
        int rows = mapOutline.GetLength(0);
        int columns = mapOutline.GetLength(1);

        // Lowest map positions are 0,0 so we only need the highest
        // in order to center the camera
        float furthestRight = GetPositionAtMapCoordinate(columns - 1, 0).x;
        float furthestUp = GetPositionAtMapCoordinate(0, rows - 1).y;

        Camera.main.transform.position = new Vector3(furthestRight / 2.0f, furthestUp / 2.0f, Camera.main.transform.position.z);
    }

    /// <summary>
    /// Returns the world position of the map square at the specified coordinate
    /// </summary>
    /// <returns>World coords at specified map coordinate</returns>
    private Vector3 GetPositionAtMapCoordinate(int x, int y)
    {
        // TODO: reflect changes once sprites are implemented instead of boxes
        return new Vector3(x, y);
    }

    /// <summary>
    /// Returns the map coordinate that corresponds to the given world position
    /// </summary>
    /// <param name="position">The world position that you want map coords for</param>
    /// <returns>The proper map coordinates based on given position</returns>
    private Vector2Int GetMapCoordinateAtWorldPosition(Vector3 position)
    {
        // TODO: reflect changes that occur once sprites are implemented instead of boxes.
        return new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// Generates the map using the <see cref="mapGrid"/> and 
    /// the various tile types
    /// </summary>
    private void GenerateMap()
    {
        // All map squares will be added as children to this 
        // transform, so as not make the hierarchy messy
        Transform mapParent = new GameObject("MapParent").transform;

        // Iterate through the every map square
        for (int i = 0; i < mapOutline.GetLength(0); i++)
        {
            for (int j = 0; j < mapOutline.GetLength(1); j++)
            {
                // Get the current map square and create an object of 
                // the proper type
                var mapSquareType = (MapTiles)mapOutline[i, j];

                GameObject newMapSquare;
                switch (mapSquareType)
                {
                    case MapTiles.Empty:
                        newMapSquare = Instantiate(emptyTile, mapParent.transform);
                        break;
                    case MapTiles.EnemyStart:
                        enemySpawnPosition = GetPositionAtMapCoordinate(j, i);
                        newMapSquare = Instantiate(pathTile, mapParent.transform);
                        break;
                    case MapTiles.Path:
                        newMapSquare = Instantiate(pathTile, mapParent.transform);
                        break;
                    case MapTiles.Base:
                        newMapSquare = Instantiate(baseTile, mapParent.transform);
                        newMapSquare.GetComponent<Base>().mapManager = gameObject;
                        playerBase = newMapSquare;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid map square specified : '{mapOutline[i, j]}' at coordinate {i}, {j}");
                }

                // Set position and name
                newMapSquare.transform.position = new Vector3(j, i);
                newMapSquare.name += $" at index {i}, {j}";
            }
        }
    }

    /// <summary>
    /// Places a tower at the given world position
    /// </summary>
    /// <param name="position">World position to place the tower at</param>
    /// <param name="towerType">The type  of tower to place</param>
    public bool PlaceTower(Vector3 position, TowerType towerType)
    {
        Vector2Int mapCoords = GetMapCoordinateAtWorldPosition(position);
		return PlaceTower(mapCoords.x, mapCoords.y, towerType);
    }

    /// <summary>
    /// Places a tower at the specified map coordinate (not world position)
    /// </summary>
    /// <param name="x">X map coord</param>
    /// <param name="y">Y map coord</param>
    /// <param name="towerType">The tower to place</param>
    /// <returns>Whether the tower placement succeeded</returns>
    public bool PlaceTower(int x, int y, TowerType towerType)
    {

        // If the map space is not empty, don't place a tower
        if (towerGrid[y, x].tower != null
            || mapOutline[y, x] != (int)MapTiles.Empty)
        {
            // TODO: Give player feedback that tower placement failed
            Debug.Log("Tower placement failed");
            return false;
        }

        // Make a new tower of the specified type, at the correct map position
        GameObject newTower = Instantiate(towerPrefabDictionary[towerType], towerContainer);
        newTower.transform.position = GetPositionAtMapCoordinate(x, y)
            + new Vector3(0, 0, -1);

		// Grab tower component
		Tower towerScript = newTower.GetComponent<Tower>();
		if (towerScript is SpecialTower)
		{
			((SpecialTower)towerScript).mode = towerType;
		}

        // Name it accordingly
        newTower.name = $"{towerType.ToString()} tower at position {x}, {y}.";

        // Add the new tower to the list of towers
        towers.Add(towerScript);
        towerScript.SetMapManager(this);

        // Update towerGrid to reflect new tower
        towerGrid[y, x] = new TowerGridSpace(towerType, newTower);

		// A tower was placed
		return true;
    }

    /// <summary>
    /// Removes tower at the given world position
    /// </summary>
    /// <param name="position">World position of the tower to remove</param>
    /// <returns>Type of the destroyed tower, or null if there was no tower on the space</returns>
    public TowerType? RemoveTower(Vector3 position)
    {
        Vector2Int mapCoords = GetMapCoordinateAtWorldPosition(position);
        return RemoveTower(mapCoords.x, mapCoords.y);
    }
    
    /// <summary>
    /// Removes the tower at map coordinate x, y
    /// </summary>
    /// <param name="x">X coord of the tower in map coords</param>
    /// <param name="y">Y coord of the tower in map coords</param>
    /// <param name="dropResources">Whether this tower should drop anything</param>
    /// <returns>Type of the destroyed tower, or null if there was no tower on the space</returns>
    public TowerType? RemoveTower(int x, int y, bool dropResources = false)
    {
        // If there's no tower on this space, don't do anything
        TowerGridSpace towerInfo = towerGrid[y, x];
        if (towerInfo.tower == null)
        {
            return null;
        }

        // Get the tower, remove it from the list of towers, then save its type
        Tower tower = towerInfo.tower.GetComponent<Tower>();
        towers.Remove(tower);
        TowerType type = towerInfo.type.Value;

        // Destroy the tower
        Destroy(towerInfo.tower);

        // Replace the TowerGridSpace with an empty one
        towerGrid[y, x] = new TowerGridSpace(null, null);
        return type;
    }

    /// <summary>
    /// Pops the next wave of enemies from <see cref="waves"/> 
    /// into <see cref="currentWaveEnemies"/> and starts the new wave
    /// </summary>
    private void StartWave()
    {
        int basicEnemies = currentWave % 4;
        int buffEnemies = currentWave / 4;
		int speedyEnemies = currentWave / 8;
		int thiefEnemies = currentWave / 12;

		

        currentWaveEnemies = CreateWave(basicEnemies, buffEnemies, speedyEnemies, thiefEnemies);
        StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// Creates a wave with the specified number of each enemy type
    /// </summary>
    /// <param name="basicEnemies">Number of basic enemies in the wave</param>
    /// <returns>A stack with the specified amount of enemies</returns>
    private Queue<Enemy> CreateWave(int basicEnemies = 0, int buffEnemies = 0, int speedyEnemies = 0, int thiefEnemies = 0)
    {
        Queue<Enemy> enemies = new Queue<Enemy>();
        for (int i = 0; i < basicEnemies; i++)
        {
			enemies.Enqueue(enemyPrefabDictionary[EnemyType.Basic].GetComponent<Enemy>());
        }
        for (int i = 0; i < buffEnemies; i++)
        {
            enemies.Enqueue(enemyPrefabDictionary[EnemyType.Buff].GetComponent<Enemy>());
		}
		for (int i = 0; i < speedyEnemies; i++)
		{
			enemies.Enqueue(enemyPrefabDictionary[EnemyType.Speedy].GetComponent<Enemy>());
		}
		for (int i = 0; i < thiefEnemies; i++)
		{
			enemies.Enqueue(enemyPrefabDictionary[EnemyType.Thief].GetComponent<Enemy>());
		}

		return enemies;
    }

    /// <summary>
    /// Spawns enemy from <see cref="currentWaveEnemies"/> 
    /// every <see cref="secondsPerEnemy"/> seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(secondsPerEnemy);

        // Spawn a new enemy after waiting secondsPerEnemy seconds
        
        GameObject newEnemy = Instantiate(currentWaveEnemies.Dequeue().gameObject);
        newEnemy.transform.position = GetPositionAtMapCoordinate(0,2);
		newEnemy.GetComponent<Enemy>().SetMap(this.mapOutline);		
        enemiesOnScreen.Add(newEnemy.GetComponent<Enemy>());
        //newEnemy.GetComponent<Enemy>().mapManager = this;
        //newEnemy.GetComponent<Enemy>().SetMap(mapOutline);

        // Only continue to spawn enemies 
        // if there are more to spawn.
        if (currentWaveEnemies.Count != 0)
        {
            StartCoroutine(SpawnEnemy());
        }
        else
        {
            // move to the next wave
            currentWave++;
            StartCoroutine(WaitThenSpawnNextWave());
        }
    }

    private IEnumerator WaitThenSpawnNextWave()
    {
		yield return new WaitUntil(()=>{
			return enemiesOnScreen.Count < 1;
		});
        StartWave();
    }
	
}