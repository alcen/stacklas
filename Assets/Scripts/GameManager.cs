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
    // Cuboid speed is represented in cuboid lengths per frame update
    [SerializeField] private double cuboidSpeed = 0.2f;
    [SerializeField] private double cuboidOscillationDistance = 30.0f;
    // Width of original cuboid in units
    [SerializeField] private float originalCuboidWidth = 25.0f;
    // Determines the spacing between cuboids
    [SerializeField] private double cuboidHeight = 2.0f;

    [Header("Player data")]
    [SerializeField] private int score = 0;
    [SerializeField] private float xAxisWidth = 0;
    [SerializeField] private float zAxisWidth = 0;

    
    // GameObject of cuboid currently being stacked
    private GameObject currentCuboid;
    
    // GameObject of previous cuboid that was stacked
    private GameObject prevCuboid;
    // Whether the next cuboid is spawning left or right
    private bool isSpawningLeft = false;
    private bool disableClicks = false;
    private float prevCuboidXWidth = 0.0f;
    private float prevCuboidZWidth = 0.0f;

    void Start()
    {
        xAxisWidth = 1;
        zAxisWidth = 1;
        InitialiseGame();
    }

    // The main game loop
    void Update()
    {
        if (!disableClicks && Input.GetButton("Fire1"))
        {
            disableClicks = true;
            // Check for stacked cuboid position and compare to initial
            Debug.Log("Clicked");
            currentCuboid.GetComponent<StackingCuboid>().enabled = false;     
            Vector3 currentCuboidPos = currentCuboid.transform.position;
            Vector3 prevCuboidPos = prevCuboid.transform.position;
            Vector3 distanceFromCentre = currentCuboidPos - prevCuboidPos;
            // // Assume cuboids are axis-aligned
            // bool isZAlignedCuboid = distanceFromCentre.z > distanceFromCentre.x;
            // float originalCuboidWidth = isZAlignedCuboid ? prevCuboidZWidth : prevCuboidXWidth;
            // float newCuboidWidthRatio = distanceFromCentre.magnitude / originalCuboidWidth;
            // Debug.Log(distanceFromCentre.magnitude);
            // Debug.Log(newCuboidWidthRatio);
            // if (isZAlignedCuboid)
            // {
            //     zAxisWidth = newCuboidWidthRatio;
            //     prevCuboidZWidth *= newCuboidWidthRatio;
            // }
            // else
            // {
            //     xAxisWidth = newCuboidWidthRatio;
            //     prevCuboidXWidth *= newCuboidWidthRatio;
            // }
            // currentCuboid.GetComponent<StackingCuboid>().SetScale(xAxisWidth, zAxisWidth);

            // If either score is less than 0, the game ends
            if (xAxisWidth < 0 || zAxisWidth < 0)
            {
                // Game over
                Debug.Log("Game over");
            }
            else
            {
                score++;            
                xAxisWidth = 1;
                zAxisWidth = 1;
                SpawnCuboid();
            }
        }
    }

    // This is called when a new game is started to reset the player data
    void InitialiseGame()
    {
        score = 0;
        xAxisWidth = cuboidPrefab.transform.localScale.x;
        zAxisWidth = cuboidPrefab.transform.localScale.z;
        prevCuboidXWidth = originalCuboidWidth;
        prevCuboidZWidth = originalCuboidWidth;

        // Delete all existing cuboids
        foreach (Transform child in parentOfCuboids.transform)
        {
            // Destroy(child.gameObject);
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
        // Set scale of the new cuboid
        cuboidProperties.SetScale(xAxisWidth, zAxisWidth);
        prevCuboid = currentCuboid;
        currentCuboid = newCuboid;
        isSpawningLeft = !isSpawningLeft;
    }

    // Helper function to generate a random colour
    private Color GetRandomColour()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
