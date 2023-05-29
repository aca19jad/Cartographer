using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDrawer
{
    private static int width;
    private static int height;

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
        width = noiseMap.GetLength(0);
        height = noiseMap.GetLength(1);

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

        colourMap = Decorate(colourMap, noiseMap, settings, palette);

        return colourMap;
    }

    private static bool IsShoreline(float[,] noiseMap, int xPos, int yPos, float seaLevel, int lineThickness){
        for (int y = -lineThickness; y <= lineThickness; y++){
            for (int x = -lineThickness; x <= lineThickness; x++){
                if( !(xPos + x < 0 || xPos + x >= width) &&
                    !(yPos + y < 0 || yPos + y >= height)){
                    if(noiseMap[xPos+x, yPos+y] < seaLevel){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static Color[] Decorate(Color[] colourMap, float[,] noiseMap,  MapSettings settings, Palette palette){

        if(settings.compassRose)
            colourMap = DrawCompassRoseRays(colourMap, noiseMap, settings, palette.backgroundLine);
    
        if(settings.gridLines)
            colourMap = DrawGridLines(colourMap, noiseMap, settings, palette.backgroundLine);

        if(settings.border)
            colourMap = DrawBorder(colourMap, settings.borderWidth, palette.border);

        return colourMap;
    }

    private static Color[] DrawBorder(Color[] colourMap, int borderWidth, Color borderColour){
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(y < borderWidth || y >= height-borderWidth-1){
                    colourMap[x + y * width] = borderColour;
                }
                else if(x < borderWidth || x >= width-borderWidth-1){
                    colourMap[x + y * width] = borderColour;
                }
                else{
                    x = width-borderWidth-1;
                }
            }
        }

        return colourMap;
    }

    private static Color[] DrawGridLines(Color[] colourMap, float[,] noiseMap, MapSettings settings, Color lineColour){
        for (int x = 0; x < width; x+=settings.lineSpacing)
        {
            colourMap = LineDrawer.DrawLine(
                colourMap, 
                noiseMap, 
                new Vector2(x, 0), 
                new Vector2(x, height-1), 
                lineColour, 
                settings.seaLevel
            );
        }
        
        for (int y = 0; y < height; y+=settings.lineSpacing)
        {
            colourMap = LineDrawer.DrawLine(
                colourMap, 
                noiseMap, 
                new Vector2(0,y), 
                new Vector2(width-1,y), 
                lineColour, 
                settings.seaLevel
            );
        }
        
        return colourMap;
    }

    // function that draws the rays extending from the compass rose 
    private static Color[] DrawCompassRoseRays(Color[] colourMap, float[,] noiseMap, MapSettings settings, Color lineColour){

        // stores a value equal to 1/16 of a full turn or 22.5 degrees
        float delta_theta = Mathf.PI/8f;

        // modulo the given angle so tht each ray only rotates by a maximum 22.5 degree angle
        float theta = settings.roseAngle % delta_theta;

        // define angles used in equations to simplify later expressions
        float[] angles = new float[7];
        for (int i = -3; i < 4; i++)
        {
            angles[i + 3] = Mathf.Tan(theta + i * delta_theta);
        }

        // calculates the equation for each ray and draws it to the colourMap
        for (int i = 0; i < 8; i++)
        {
            Vector3 eq = new Vector3(); // equation vector (A, B, C) -> Ax + By = C

            // sets up the equation vector
            // special case for when angle could equal 90 because tan(90) = infinity so change the frame from x to y 
            if(i < 7){
                eq = new Vector3(1, -angles[i], (settings.rosePosition.x-settings.rosePosition.y * angles[i]));
            } 
            else{
                eq = new Vector3(angles[3], 1, (settings.rosePosition.y+settings.rosePosition.x * angles[3]));
            }

            // gets the intersections with the edge of the map
            Vector2[] points = LineDrawer.GetEdgeCoords(eq, width, height);

            // draws the line on the colourMap;
            colourMap = LineDrawer.DrawLine(colourMap, noiseMap, points[0], points[1], lineColour, settings.seaLevel);
        }

        return colourMap;
    }
}
