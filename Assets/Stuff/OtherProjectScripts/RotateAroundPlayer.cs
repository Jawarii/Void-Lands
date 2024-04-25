using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RotateAroundPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    
    private Vector3 rotationCenter; // Center point for rotation
    private float currentAngle = 0f; // Current angle in degrees
    public GameObject prefab;
    public GameObject object1;
    public List<GameObject> rotatingObjects = new List<GameObject>();

    public float amount_ = 3;
    public float baseRadius = 1f;
    public float radiusMulti = 1f;
    public int dmgCoe = 40;
    public int critCoe = 0;
    public float cd;
    public float currentCd;
    public float rotationSpeed = 120f; // Rotation speed in degrees per second
    public float radius = 1f; // Radius of rotation

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not set. Please assign the player transform in the inspector.");
            enabled = false; // Disable the script if the player reference is missing.
            return;
        }

        // Calculate the initial center of rotation based on the player's position and radius.

        CalculateRotationCenter();

        for (int i = 0; i < amount_; i++)
        {
            float radians = (currentAngle + (360f / rotatingObjects.Count * i)) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            Vector3 desiredPosition = rotationCenter + offset;
            rotatingObjects.Add(Instantiate(prefab));
        }
    }

    private void Update()
    {
        radius = baseRadius * radiusMulti;
        // Update the current angle based on the rotation speed.
        currentAngle -= rotationSpeed * Time.deltaTime;
        if (amount_ > rotatingObjects.Count)
        {
            rotatingObjects.Add(Instantiate(prefab));
        }
        else if (amount_ < rotatingObjects.Count)
        {
            Destroy(rotatingObjects[(int)amount_]);
            rotatingObjects.RemoveAt((int)amount_);
        }
        for (int i = 0; i < rotatingObjects.Count; i++)
        {
            // Calculate the desired position based on the current angle and radius.
            float radians = (currentAngle + (360f / rotatingObjects.Count * i)) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            Vector3 desiredPosition = rotationCenter + offset;

            // Move the object to the desired position.
            rotatingObjects[i].transform.position = desiredPosition;
        }

        for (int i = 0; i < rotatingObjects.Count; i++)
        {
            rotatingObjects[i].gameObject.GetComponent<OrbitSkil>().dmgCoe = dmgCoe;
            rotatingObjects[i].gameObject.GetComponent<OrbitSkil>().critCoe = critCoe;
            rotatingObjects[i].gameObject.GetComponent<OrbitSkil>().baseRadius = baseRadius;
            rotatingObjects[i].gameObject.GetComponent<OrbitSkil>().radiusMulti = radiusMulti;
        }
    }

    private void CalculateRotationCenter()
    {
        // Update the rotation center based on the player's position.
        rotationCenter = player.position;
    }

    private void LateUpdate()
    {
        // Call CalculateRotationCenter in LateUpdate to ensure it's called after the player's position is updated.
        CalculateRotationCenter();
    }
}
