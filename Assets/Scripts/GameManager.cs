using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Location of first cuboid")]
    [SerializeField] private Transform originalCuboid;
    [Header("Spawning location of left cuboid")]
    [SerializeField] private Transform leftCuboidSpawn;
    [Header("Spawning location of right cuboid")]
    [SerializeField] private Transform rightCuboidSpawn;
    [Header("Prefab of a cuboid")]
    [SerializeField] private GameObject cuboidPrefab;
    [Header("Parent of all the cuboids")]
    [SerializeField] private Transform parentOfCuboids;

    [Header("Game constants")]
    // How many frames allowed for perfection
    [SerializeField] private int perfectionThreshhold = 1;
    // Cuboid speed is represented in units per frame update
    [SerializeField] private double cuboidSpeed = 1.00f;
    [SerializeField] private double cuboidOscillationDistance = 30.0f;
    // Determines the spacing between cuboids
    [SerializeField] private double cuboidHeight = 2.0f;

    [Header("Player data")]
    [SerializeField] private int score = 0;
    [SerializeField] private double xAxisWidth = 0;
    [SerializeField] private double zAxisWidth = 0;

    
    // GameObject of cuboid currently being stacked
    private GameObject currentCuboid;
    
    // GameObject of previous cuboid that was stacked
    private GameObject prevCuboid;
    // Whether the next cuboid is spawning left or right
    private bool isSpawningLeft = false;

    void Start()
    {
        InitialiseGame();
    }

    // The main game loop
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            // Check for stacked cuboid position and compare to initial


            Debug.Log("Clicked");
        }

        // If either score is less than 0, the game ends
        if (xAxisWidth < 0 || zAxisWidth < 0)
        {
            // Game over
            Debug.Log("Game over");
        }
        else
        {
            score++;
        }
    }

    // This is called when a new game is started to reset the player data
    void InitialiseGame()
    {
        score = 0;
        xAxisWidth = cuboidPrefab.transform.localScale.x;
        zAxisWidth = cuboidPrefab.transform.localScale.z;

        // Delete all existing cuboids
        foreach (Transform child in parentOfCuboids.transform)
        {
            Destroy(child.gameObject);
        }

        // Create the first cuboid
        GameObject firstCuboid = Instantiate(cuboidPrefab,
                                             originalCuboid.position,
                                             Quaternion.identity,
                                             parentOfCuboids) as GameObject;     
        // Set a random colour for the first cuboid
        StackingCuboid firstStacking = firstCuboid.GetComponent<StackingCuboid>();
        firstStacking.SetColour(GetRandomColour());
        // Disable StackingCuboid component
        firstStacking.enabled = false;
        // When SpawnCuboid is called, prevCuboid will be set to firstCuboid
        currentCuboid = firstCuboid;
        isSpawningLeft = Random.value > 0.5f;
        SpawnCuboid();
    }

    // Spawns a new cuboid for the game
    void SpawnCuboid()
    {
        Transform spawnLocation = leftCuboidSpawn;
        if (isSpawningLeft)
        {
            spawnLocation = rightCuboidSpawn;
        }
        // Ensure that cuboids do not overlap
        Vector3 spawnCoords = spawnLocation.position;
        spawnCoords.y = spawnCoords.y + (float)cuboidHeight;

        GameObject newCuboid = Instantiate(cuboidPrefab,
                                           spawnCoords,
                                           Quaternion.identity,
                                           parentOfCuboids) as GameObject;
        StackingCuboid cuboidProperties = newCuboid.GetComponent<StackingCuboid>();
        cuboidProperties.SetProperties(cuboidSpeed,
                                       originalCuboid.transform.position,
                                       cuboidOscillationDistance);
        // Set a random colour for the new cuboid
        cuboidProperties.SetColour(GetRandomColour());
        prevCuboid = currentCuboid;
        currentCuboid = newCuboid;
    }

    // Helper function to generate a random colour
    private Color GetRandomColour()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
