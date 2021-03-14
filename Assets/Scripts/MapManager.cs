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
///     <item>Enemy</item>
///   </list>
/// </summary>
public enum MapTiles
{
    Empty = 0,
    Path = 1,
    Base = 2,

    Player = 99,
    Tower = 98,
    Enemy = 97
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
///       <item>Keeping track of where everything is on the map in <see cref="mapGrid"/></item>
///       <item>Handling and validating tower placement in <see cref="PlaceTower(GameObject, int, int)"/></item>
///     </list>
///   </para>
/// </summary>
public class MapManager : MonoBehaviour
{
    // All the towers in the scene
    private List<AttackingEntity> towers;
    // All the waves in the game
    private Stack<AttackingEntity> waves;
    // The enemies that will be spawned into the level
    private Stack<AttackingEntity> currentWave;
    // The enemies that are already spawned into the level
    // TODO: assess if we need this
    private List<AttackingEntity> enemiesOnScreen;
    // The player avatar
    private MovingEntity avatar;
    // Parent Transform to any created towers
    private Transform towerContainer;

    // Only using for testing, delete when done!
    [SerializeField]
    private GameObject towerTest;

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
    private float secondsPerEnemy = 1;

    // The map, represented as ints based on the MapSquares enum
    private int[,] mapGrid =
    {
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 2 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 }
    };

    // Start is called before the first frame update
    void Start()
    {
        towerContainer = new GameObject("Towers").transform;

        GenerateMap();
    }

    private void Update()
    {
        // Tower placement tests
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Valid, bottom left
            PlaceTower(towerTest, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Valid, top right
            PlaceTower(
                towerTest, 
                mapGrid.GetLength(1) - 1, 
                mapGrid.GetLength(0) - 1);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Invalid, on a path
            PlaceTower(towerTest, 2, 2);
        }
    }

    // ------------------------------------
    // --------  HELPER FUNCTIONS  --------
    // ------------------------------------

    /// <summary>
    /// Pops the next wave of enemies from <see cref="waves"/> 
    /// into <see cref="currentWave"/> and starts the new wave
    /// </summary>
    private void StartWave()
    {
        // TODO: Get Enemy class and set enemies stack 
        // to whatever we want the wave to look like
        // currentWave = waves.Pop();
        // StartCoroutine(SpawnEnemy());
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
                var mapSquareType = (MapTiles)mapGrid[i, j];

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
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid map square specified : '{mapGrid[i,j]}' at coordinate {i}, {j}");
                }

                // Set position and name
                newMapSquare.transform.position = new Vector3(j, i);
                newMapSquare.name += $" at index {i}, {j}";
            }
        }
    }

    // Note: tower param is a GameObject right now, will probably 
    // want to change that to a Tower script or something
    /// <summary>
    /// Places a tower at the specified map coordinate (not world position)
    /// </summary>
    /// <param name="tower">The tower to place</param>
    /// <param name="x">X map coord</param>
    /// <param name="y">Y map coord</param>
    private void PlaceTower(GameObject tower, int x, int y)
    {
        // If the map space is not empty, don't place a tower
        if (mapGrid[y, x] != (int)MapTiles.Empty)
        {
            // TODO: Give player feedback that tower placement failed
            Debug.Log("Tower placement failed");
            return;
        }

        GameObject newTower = Instantiate(tower, towerContainer);
        newTower.transform.position = GetPositionAtMapCoordinate(x, y)
            + new Vector3(0, 0, -1);
        newTower.name = $"TowerType tower at position {x}, {y}.";

        // Update mapGrid to reflect new tower
        mapGrid[y, x] = (int)MapTiles.Tower;
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
        currentWave.Pop();

        // Only continue to spawn enemies 
        // if there are more to spawn.
        if (currentWave.Count != 0)
        {
            StartCoroutine(SpawnEnemy());
        }
    }
}
