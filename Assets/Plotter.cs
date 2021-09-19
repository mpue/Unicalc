using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plotter : MonoBehaviour
{
    LineRenderer lineRenderer;
    public float theta_scale = 0.1f;  // Circle resolution
    public Color lineColor;
    public float radius;
    public int numPoints;

    void Start()
    {        
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
                
        lineRenderer.startColor = lineColor;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        float x = 0;
        float y = 0;

        int i = 0;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += theta_scale)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, y, 0);
            lineRenderer.SetPosition(i, pos);
            i += 1;
        }
    }
}
