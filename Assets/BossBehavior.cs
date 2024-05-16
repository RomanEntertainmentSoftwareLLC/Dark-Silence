using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    // Starting time
    private float time;

    // Reference to Level1 script
    private Level1 levelScript;

    // Starting position
    private Vector2 startPos;

    private Vector2 map_pos;

    // Whether the boss has started its movement pattern
    private bool hasStartedPattern;

    // Delay before starting the pattern
    private const float patternStartDelay = 5f;

    // Movement parameters
    private const float offsetX = 1f;
    private const float offsetY = 0f;
    private const float xSize = 5.0f;
    private const float ySize = 2.5f;
    private const float speed = 10f;

    private void Start()
    {
        // Get Level1 script from the parent GameObject
        levelScript = transform.parent.GetComponent<Level1>();

        // Store the initial position
        startPos = transform.position;
    }

    void Update()
    {
        // Get the map position from the Level1 script
        map_pos = levelScript.GetMapPosition();

        if (map_pos.y <= Level1.LevelMaxPosition)
        {
            // Start the pattern after the specified delay
            Invoke("StartPattern", patternStartDelay);
        }

        // Check if the boss has started its movement pattern
        if (hasStartedPattern)
        {
            // Compute new local position
            float xSin = (speed * time + Mathf.PI / 2) * Mathf.Deg2Rad;
            float ySin = (2 * speed * time) * Mathf.Deg2Rad;

            float x = offsetX + startPos.x + map_pos.x + (xSize * Mathf.Sin(xSin));
            float y = offsetY + startPos.y + map_pos.y + (ySize * Mathf.Sin(ySin));

            Debug.Log("x: " + x + ", " + "y: " + y + ", " + "Mathf.Sin(xSin): " + Mathf.Sin(xSin) + ", " + "Mathf.Sin(ySin): " + Mathf.Sin(ySin));

            // Apply new local position relative to the map
            transform.position = new Vector2(x, y);

            // Increase time
            time += Time.deltaTime;
        }
    }

    void StartPattern()
    {
        // Set the flag to true, indicating that the boss has started its movement pattern
        hasStartedPattern = true;
    }
}
