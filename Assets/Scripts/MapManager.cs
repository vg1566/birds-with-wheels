using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anything that can fill a map space. On generation, this would mean one of:
///   <list type="number">
///     <item>Empty - self-explanatory</item>
///     <item>Path - Navigable path enemies can travel on.</item>
///     <item>Base - The player's base they're protecting</item>
///   </list>
/// Once the game gets going, the following may also occupy a space:
///   <list type="number">
///     <item>Player</item>
///     <item>Tower</item>
///   </list>
/// </summary>
public enum MapTiles
{
    Empty = 0,
    Path = 1,
    Base = 2,

    Player = 99,
    Tower = 98,
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
///       <item>Spawning waves of enemies from <see cref="currentWave"/></item>
///       <item>Storing future waves that will be spawned in <see cref="waves"/></item>
///       <item>Keeping track of where everything is on the map in <see cref="mapOutline"/></item>
///       <item>Handling and validating tower placement in <see cref="PlaceTower(GameObject, int, int)"/></item>
///     </list>
///   </para>
/// </summary>
public class MapManager : MonoBehaviour
{
    // All the towers in the scene
    private List<Entity> towers;
    // All the waves in the game
    private Queue<Queue<Enemy>> waves;
    // The enemies that will be spawned into the level
    private Queue<Enemy> currentWave;
    // The enemies that are already spawned into the level
    // TODO: assess if we need this
    private List<Entity> enemiesOnScreen;
    // The player avatar
    public Avatar avatar;
    // Parent Transform to any created towers
    private Transform towerContainer;

    [SerializeField]
    private GameObject basicEnemy;
    [SerializeField]
    private GameObject buffEnemy;

    [SerializeField]
    private GameObject basicTower;
    [SerializeField]
    private GameObject specialTower;

    private Dictionary<TowerType, GameObject> towerPrefabs;

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
    private KeyValuePair<MapTiles, GameObject>[,] mapGrid;

    // This is how the map will be generated
    private readonly int[,] mapOutline =
    {
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    // Start is called before the first frame update
    void Start()
    {
        towerPrefabs = new Dictionary<TowerType, GameObject> 
        {
            { TowerType.Basic, basicTower },
            { TowerType.SpecialDefense, specialTower }
        };

        towerContainer = new GameObject("Towers").transform;

        enemiesOnScreen = new List<Entity>();
        towers = new List<Entity>();
        currentWave = new Queue<Enemy>();

        #region Wave initialization
        waves = new Queue<Queue<Enemy>>();

        // Make waves
        for (int i = 1; i < 12; i++)
        {
            int basicEnemies = i % 4;
            int buffEnemies = i / 4;

            waves.Enqueue(CreateWave(basicEnemies, buffEnemies));
        }

        #endregion

        // Generate the map based on mapOutline
        mapGrid = new KeyValuePair<MapTiles, GameObject>[mapOutline.GetLength(0), mapOutline.GetLength(1)];
        GenerateMap();

        // Start the game
        StartWave();
    }

    /// <summary>
    /// Creates a wave with the specified number of each enemy type
    /// </summary>
    /// <param name="basicEnemies">Number of basic enemies in the wave</param>
    /// <returns>A stack with the specified amount of enemies</returns>
    private Queue<Enemy> CreateWave(int basicEnemies = 0, int buffEnemies = 0)
    {
        Queue<Enemy> enemies = new Queue<Enemy>();
        for (int i = 0; i < basicEnemies; i++)
        {
            enemies.Enqueue(basicEnemy.GetComponent<Enemy>());
        }
        for (int i = 0; i < buffEnemies; i++)
        {
            enemies.Enqueue(buffEnemy.GetComponent<Enemy>());
        }

        return enemies;
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

            ((Enemy)enemiesOnScreen[i]).FindTarget(towers);
        }
    }

    // ------------------------------------
    // --------  HELPER FUNCTIONS  --------
    // ------------------------------------

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
        for (int i = 0; i < mapGrid.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrid.GetLength(1); j++)
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
                    case MapTiles.Path:
                        newMapSquare = Instantiate(pathTile, mapParent.transform);
                        break;
                    case MapTiles.Base:
                        newMapSquare = Instantiate(baseTile, mapParent.transform);
                        newMapSquare.GetComponent<Base>().mapManager = gameObject;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid map square specified : '{mapGrid[i, j]}' at coordinate {i}, {j}");
                }

                // Set position and name
                newMapSquare.transform.position = new Vector3(j, i);
                newMapSquare.name += $" at index {i}, {j}";

                // Add the new tile to the mapGrid
                mapGrid[i, j] = new KeyValuePair<MapTiles, GameObject>(mapSquareType, newMapSquare);
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
        PlaceTower(mapCoords.x, mapCoords.y, towerType);
        return true;
    }

    /// <summary>
    /// Places a tower at the specified map coordinate (not world position)
    /// </summary>
    /// <param name="x">X map coord</param>
    /// <param name="y">Y map coord</param>
    /// <param name="towerType">The tower to place</param>
    public void PlaceTower(int x, int y, TowerType towerType)
    {
        // If the map space is not empty, don't place a tower
        if (mapGrid[y, x].Key != MapTiles.Empty)
        {
            // TODO: Give player feedback that tower placement failed
            Debug.Log("Tower placement failed");
            return;
        }

        // Make a new tower of the specified type, at the correct map position
        GameObject newTower = Instantiate(towerPrefabs[towerType], towerContainer);
        newTower.transform.position = GetPositionAtMapCoordinate(x, y)
            + new Vector3(0, 0, -1);

        // Grab tower component
        Tower towerScript = newTower.GetComponent<Tower>();

        // Name it accordingly
        newTower.name = $"{towerType.ToString()} tower at position {x}, {y}.";

        // Add the new tower to the list of towers
        towers.Add(towerScript);
        towerScript.SetMapManager(this);

        // Update mapGrid to reflect new tower
        mapGrid[y, x] = new KeyValuePair<MapTiles, GameObject>(MapTiles.Tower, newTower);
    }

    /// <summary>
    /// Removes tower at the given world position
    /// </summary>
    /// <param name="position">World position of the tower to remove</param>
    public TowerType RemoveTower(Vector3 position)
    {
        Vector2Int mapCoords = GetMapCoordinateAtWorldPosition(position);
        RemoveTower(mapCoords.x, mapCoords.y);
        return TowerType.Basic;
    }
    
    /// <summary>
    /// Removes the tower at map coordinate x, y
    /// </summary>
    /// <param name="x">X coord of the tower in map coords</param>
    /// <param name="y">Y coord of the tower in map coords</param>
    /// <param name="dropResources">Whether this tower should drop anything</param>
    public void RemoveTower(int x, int y, bool dropResources = false)
    {
        towers.Remove(mapGrid[y, x].Value.GetComponent<Tower>());
        Destroy(mapGrid[y, x].Value);

        // Replace the mapGrid square with the empty one under it
        mapGrid[y, x] = new KeyValuePair<MapTiles, GameObject>(MapTiles.Empty, GameObject.Find($"Empty at index {y}, {x}"));
    }

    /// <summary>
    /// Pops the next wave of enemies from <see cref="waves"/> 
    /// into <see cref="currentWave"/> and starts the new wave
    /// </summary>
    private void StartWave()
    {
        // TODO: Get Enemy class and set enemies stack 
        // to whatever we want the wave to look like
        currentWave = waves.Dequeue();
        StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// Spawns enemy from <see cref="currentWave"/> 
    /// every <see cref="secondsPerEnemy"/> seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(secondsPerEnemy);

        // Spawn a new enemy after waiting secondsPerEnemy seconds
        
        GameObject newEnemy = Instantiate(currentWave.Dequeue().gameObject);
        newEnemy.transform.position = mapGrid[2, 0].Value.transform.position;
        enemiesOnScreen.Add(newEnemy.GetComponent<Enemy>());
        

        // Only continue to spawn enemies 
        // if there are more to spawn.
        if (currentWave.Count != 0)
        {
            StartCoroutine(SpawnEnemy());
        }
        else
        {
            // Start the next wave in 10 seconds
            StartCoroutine(SpawnNextWaveInSeconds());
        }
    }

    private IEnumerator SpawnNextWaveInSeconds()
    {
        yield return new WaitForSeconds(secondsBetweenWaves);
        StartWave();
    }
}
