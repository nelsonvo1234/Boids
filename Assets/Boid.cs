using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Boid : MonoBehaviour
{
    Transform boidContainer;
    HashSet<Transform> boids;
    public float velocity;
    public float distance;
    public float rotationSpeed;
    public float wrapRadius;
    public float avoidDistance;
    public int sphereSamples;
    static Vector3[] sphere;
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        boids = new HashSet<Transform>();
        boidContainer = transform.parent.transform;
        if (sphere == null)
        {
            sphere = fibonacci_sphere(sphereSamples);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBoids();
        Vector3 avgDir = Vector3.zero;
        Vector3 avgPosDir = Vector3.zero;
        Vector3 avoidDir = Vector3.zero;
        Vector3 avgPos = transform.position / boids.Count;
        //print(boids.Count);
        boids.Remove(transform);
        float smallestDistance = 5f;
        //print(boids.Count);
        foreach (Transform boid in boids)
        {
            //direction is the average direction of the surrounding boids
            avgDir += boid.forward / boids.Count;

            //point away from other boids
            avoidDir -= (boid.position - transform.position) / Vector3.Distance(boid.position, transform.position);
            if (Vector3.Distance(boid.position, transform.position) < smallestDistance)
            {
                smallestDistance = Vector3.Distance(boid.position, transform.position);
            }
            //find average position of the boids
            avgPos += boid.position / (boids.Count + 1);
        }
        //point towards center of boids
        //print("avgPos: " + avgPos);
        avgPosDir = avgPos - transform.position;
        avgPosDir = avgPosDir.normalized;
        avgDir = avgDir.normalized;
        avoidDir = avoidDir.normalized;

        //print(avgPosDir+ " " + avgDir+ " " +  avoidDir);

        Ray ray;
        RaycastHit hitData;
        int i = 0;
        //LayerMask layerMask = LayerMask.NameToLayer("Boid");
        Vector3 obstacleDir = transform.forward;
        float minDistance = avoidDistance;

        //tendency to hug walls, maybe get them to do something to avoid hugging walls? like broadcast in all directions instead of 
        //find unobstructed direction
        do
        {
            Vector3 obstacle = Quaternion.AngleAxis(90, transform.right) * transform.TransformDirection(sphere[i]);
            ray = new Ray(transform.position, obstacle);
            //Debug.DrawRay(transform.position, obstacle, Color.red);
            i++;
            if (Physics.Raycast(ray, out hitData, avoidDistance, layerMask))
            {
                if (hitData.distance < minDistance)
                {
                    minDistance = hitData.distance;
                }
                obstacleDir -= obstacle;
                //print(hitData.transform.name);
            }
        } while (i < sphereSamples);
        //print(obstacleDir);
        obstacleDir = obstacleDir.normalized;
        Debug.DrawRay(transform.position, obstacleDir, Color.green);

        //print(Mathf.Pow(Mathf.InverseLerp(avoidDistance, 0, minDistance), 1));
        //print(Mathf.Pow(Mathf.InverseLerp(distance, 0, smallestDistance), 6));
        //Mathf.Lerp(0, 1, Mathf.InverseLerp(0, distance, avgPosDir.magnitude))
        Vector3 dir = Vector3.Lerp(avgDir, avgPosDir, 0.5f);
        dir = Vector3.Lerp(dir, avoidDir, Mathf.Pow(Mathf.InverseLerp(distance, 0, smallestDistance), 6));
        dir = Vector3.Lerp(dir, obstacleDir, Mathf.Pow(Mathf.InverseLerp(avoidDistance, 0, minDistance), 1));
        print(minDistance);
        Debug.DrawRay(transform.position, dir);

        if (dir == Vector3.zero)
        {
            dir = transform.forward;
        }
        print("Dir: " + dir);
        //change boid position and rotation
        transform.forward = Vector3.Lerp(transform.forward, dir.normalized, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * velocity * Time.deltaTime;

        // if (transform.position.sqrMagnitude > wrapRadius)
        // {
        //     transform.position = -transform.position;
        // }  
    }

    void UpdateBoids()
    {
        boids.Clear();
        for (int i = 0; i < boidContainer.childCount; i++)
        {
            Transform boid = boidContainer.GetChild(i);
            if (Vector3.Distance(transform.position, boid.transform.position) < distance)
            {
                boids.Add(boid);
            }
        }
    }
    
    Vector3[] fibonacci_sphere(int samples= 1000) {
        Vector3[] points = new Vector3[samples];
        float phi = Mathf.PI * (Mathf.Sqrt(5f) - 1f);  //golden angle in radians

        for (int i = 0; i < samples; i++) {
            float y = 1 - (i / (samples - 1f)) * 2;  // y goes from 1 to -1
            float radius = Mathf.Sqrt(1 - y * y);  // radius at y

            float theta = phi * i;  // golden angle increment

            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;

            points[i] = new Vector3(x, y, z);      
        }
        return points;
    }
}
