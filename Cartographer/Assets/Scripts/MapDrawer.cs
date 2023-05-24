using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDrawer
{
    private static float theta = 0.392699081699f;


    public static Color[] DrawNoiseMap(float[,] noiseMap){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];

        for(int y = 0, index = 0; y < height; y++){
            for(int x = 0; x < width; x++, index++){

                colourMap[index] = Color.Lerp(Color.black, Color.white,noiseMap[x, y]);
            }
        }

        return colourMap;
    }

    public static Color[] DrawMap(float[,] noiseMap, MapSettings settings, Palette palette){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];

        for(int y = 0, index = 0; y < height; y++){
            for(int x = 0; x < width; x++, index++){
            
                if (noiseMap [x, y] < settings.seaLevel){
                    colourMap[index] = palette.sea.Evaluate(Mathf.InverseLerp(0, settings.seaLevel, noiseMap[x, y]));
                }
                else if (IsShoreline(noiseMap, x, y, settings.seaLevel, settings.lineThickness)){
                    colourMap[index] = palette.line;
                }
                else{
                    colourMap[index] = palette.land.Evaluate(Mathf.InverseLerp(settings.seaLevel, 1, noiseMap[x, y]));
                }
            }
        }

        colourMap = Decorate(noiseMap, colourMap, settings, palette);

        return colourMap;
    }

    public static bool IsShoreline(float[,] noiseMap, int xPos, int yPos, float seaLevel, int lineThickness){
        for (int y = -lineThickness; y <= lineThickness; y++){
            for (int x = -lineThickness; x <= lineThickness; x++){
                if( !(xPos + x < 0 || xPos + x >= noiseMap.GetLength(0)) &&
                    !(yPos + y < 0 || yPos + y >= noiseMap.GetLength(1))){
                    if(noiseMap[xPos+x, yPos+y] < seaLevel){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static Color[] Decorate(float[,] noiseMap, Color[] colourMap, MapSettings settings, Palette palette){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        colourMap = DrawLines(colourMap, noiseMap, settings, palette.backgroundLine);

        for (int y = 0, index = 0; y < height; y++){
            for (int x = 0; x < width; x++, index++){

                if(IsGridLine(x, y, settings) && 
                noiseMap[x, y] < settings.seaLevel){
                    colourMap[index] = palette.backgroundLine;
                }

                if(IsBorder(x, y, width, height, settings))
                {
                    colourMap[index] = (settings.colourScheme == MapColourScheme.WEATHERED) ? palette.land.Evaluate(0) : Color.white;
                }
            }
        }

        return colourMap;
    }

    private static bool IsGridLine(int x, int y, MapSettings settings){
        return settings.gridLines && (x % settings.lineSpacing == 0 || y % settings.lineSpacing == 0);
    }

    private static bool IsBorder(int x, int y, int width, int height, MapSettings settings){
        return settings.border && 
                (x < settings.borderWidth || x > width - settings.borderWidth - 1 || 
                y < settings.borderWidth || y > height - settings.borderWidth - 1);
    }

    private static Color[] DrawLines(Color[] colourMap, float[,] noiseMap, MapSettings settings, Color lineColour){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float delta_theta = Mathf.PI/8f;

        float theta = settings.roseAngle % delta_theta;

        float[] angles = new float[7];
        for (int i = -3; i < 4; i++)
        {
            angles[i+3] = Mathf.Tan(theta + i * delta_theta);
        }

        for (int i = 0; i < 8; i++)
        {
            Vector3 eq = new Vector3();

            if(i < 7){
                eq = new Vector3(1, -angles[i], (settings.rosePosition.x-settings.rosePosition.y * angles[i]));
            }
            else{
                eq = new Vector3(angles[3], 1, (settings.rosePosition.y+settings.rosePosition.x * angles[3]));
            }

            Vector2[] points = GetEdgeCoords(eq, width, height); 
            colourMap = DrawLine(colourMap, noiseMap, points[0], points[1], lineColour, settings.seaLevel);
        }

        return colourMap;
    }

    private static Color[] DrawLine(Color[] colourMap, float[,] noiseMap, Vector2 p1, Vector2 p2, Color lineColour, float seaLevel){
        Vector2 t = p1;
        float step = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float counter = 0;
        
        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
            t = Vector2.Lerp(p1, p2, counter);
            counter += step;
            if(noiseMap[(int)t.x,(int)t.y] < seaLevel)
                colourMap[(int)t.x + noiseMap.GetLength(0) * (int)t.y] = lineColour;
        }
        return colourMap;
    }

    private static Vector2[] GetEdgeCoords(Vector3 eq, int width, int height){
        
        Vector3[] edges = {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, width-1),
            new Vector3(0, 1, height-1)
        };

        Vector2[] points = new Vector2[2];
        int index = 0;

        foreach (Vector3 edge in edges)
        {
            Vector2 point = GetEdgeCoord(eq, edge);
            
            if(point.x >= 0 && point.x < width && point.y >= 0 && point.y < height){
                points[index] = point;
                index++;
            }
        }

        return points;
    }

    private static Vector2 GetEdgeCoord(Vector3 a, Vector3 b){
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
