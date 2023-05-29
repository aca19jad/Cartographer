using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineDrawer
{
    // function to draw a line of LineColour across the colourmap given its start and end points
    // noiseMap and seaLevel are used to stop the line drawing over regions above seaLevel.
    // adapted from: https://answers.unity.com/questions/244417/create-line-on-a-texture.html
    public static Color[] DrawLine(Color[] colourMap, float[,] noiseMap, Vector2 p1, Vector2 p2, Color lineColour, float seaLevel){
        // stores the interpolated value calculated at each step 
        Vector2 t = p1; 
        // calculates how how far to travel along the line to step by an integer
        float step = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float counter = 0;
        // repeats until t reaches the end point
        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
            t = Vector2.Lerp(p1, p2, counter);
            counter += step;
            // draw the line to the colourMap if below sealevel
            if(noiseMap[(int)t.x,(int)t.y] < seaLevel)
                colourMap[(int)t.x + noiseMap.GetLength(0) * (int)t.y] = lineColour;
        }
        return colourMap;
    }

    // function that finds the intersection of a line equation with the edge of the map
    public static Vector2[] GetEdgeCoords(Vector3 eq, int width, int height){
        
        // defines equations (Ax + By = C) for the edges of the map
        Vector3[] edges = {
            new Vector3(1, 0, 0), // x = 0
            new Vector3(0, 1, 0), // y = 0
            new Vector3(1, 0, width-1), // x = width - 1
            new Vector3(0, 1, height-1) // y = height - 1
        };

        // stores the intersection data
        Vector2[] points = new Vector2[2];
        int index = 0;

        // checks the intersection of each edge with the line
        foreach (Vector3 edge in edges)
        {
            Vector2 point = GetIntersection(eq, edge);
            
            // add the intersection point if it is within the bounds of the map
            if(point.x >= 0 && point.x < width && point.y >= 0 && point.y < height){
                points[index] = point;
                index++;
            }
            if(index == 2){
                break;
            }
        }

        return points;
    }

    // function that finds the intersection point of 2 lines
    // Vector3(A, B, C) -> Ax + By = C (equation of line)
    // adapted from algorithm found at:
    // https://www.topcoder.com/thrive/articles/Geometry%20Concepts%20part%202:%20%20Line%20Intersection%20and%20its%20Applications
    private static Vector2 GetIntersection(Vector3 a, Vector3 b){
        float delta = a.x * b.y - a.y * b.x;
        if(delta == 0){
            return new Vector2(-1,-1);
        }
        else{
            int x = (int)((b.y*a.z-a.y*b.z) / delta);
            int y = (int)((a.x*b.z-b.x*a.z) / delta);
            return new Vector2(x, y);
        }
    }
}
