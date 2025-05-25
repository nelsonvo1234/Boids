using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    Vector3[] points;
    // Start is called before the first frame update
    void Start()
    {
        points = fibonacci_sphere(1000);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point, 0.01f);
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
