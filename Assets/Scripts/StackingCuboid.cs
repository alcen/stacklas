using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A script for a generated cuboid in the stacking game
public class StackingCuboid : MonoBehaviour
{
    [SerializeField] private Vector3 oscillationPoint;
    [SerializeField] private double speed;
    [SerializeField] private double oscillationDistance;

    // Helper fields for motion of the cuboid
    private Vector3 movingDirection;
    private bool isGoingInReverse = false;
    private bool hasEnteredOscillationZone = false;

    public void SetProperties(double cuboidSpeed,
                              Vector3 oscillationPoint,
                              double oscillationDistance)
    {
        this.speed = cuboidSpeed;
        this.oscillationPoint = oscillationPoint;
        this.oscillationDistance = oscillationDistance;

        // Assume the new cuboid is spawned in a position different from the oscillation point
        movingDirection = (oscillationPoint - gameObject.transform.position).normalized;
    }

    public void SetColour(Color newColour)
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColour);
    }

    public void SetScale(float scaleX, float scaleZ)
    {
        gameObject.transform.localScale = new Vector3(
            scaleX, gameObject.transform.localScale.y, scaleZ);
    }

    void Update()
    {
        Vector3 motionVector = movingDirection;
        if (isGoingInReverse)
        {
            motionVector = -motionVector;
        }
        motionVector *= (float)speed;

        Vector3 newPosition = gameObject.transform.position + motionVector;
        // If we exceed the oscillation distance, reverse the direction
        double distanceFromOscillationPoint = (newPosition - oscillationPoint).magnitude;
        if (distanceFromOscillationPoint > oscillationDistance)
        {
            if (hasEnteredOscillationZone)
            {
                isGoingInReverse = !isGoingInReverse;
                // Simulate the cuboid going to the limit and "bouncing" back
                newPosition = gameObject.transform.position - motionVector *
                                (float)(distanceFromOscillationPoint - oscillationDistance);
            }
        }
        else
        {
            hasEnteredOscillationZone = true;
        }
        // Ignore y value so that the height does not change
        gameObject.transform.position = new Vector3(
            newPosition.x, gameObject.transform.position.y, newPosition.z);
    }
}
