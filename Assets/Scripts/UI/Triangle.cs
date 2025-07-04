using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float minSpeed = 10f;  
    public float maxSpeed = 360f;
    public float cycleDuration = 4f; 

    private float angle = 0f;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float t = (Time.time % cycleDuration) / cycleDuration;
        float speedFactor = Mathf.Sin(t * Mathf.PI);
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedFactor);


        angle += currentSpeed * Time.deltaTime;


        transform.rotation = initialRotation * Quaternion.Euler(0f, 0f, -angle);
    }
}