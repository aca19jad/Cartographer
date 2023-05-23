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

        for (int y = 0, index = 0; y < height; y++){
            for (int x = 0; x < width; x++, index++){

                if(IsGridLine(x, y, settings) && 
                noiseMap[x, y] < settings.seaLevel){
                    colourMap[index] = palette.backgroundLine;
                }

                if(settings.compassRose && noiseMap[x, y] < settings.seaLevel){
                    if (IsOnRoseLine(x, y, 
                    new Vector2Int(settings.rosePosition.x+width/2, settings.rosePosition.y+height/2), settings.roseAngle)){
                        colourMap[index] = palette.backgroundLine;
                    }
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

    private static bool IsOnRoseLine(int x, int y, Vector2Int pos, float offset){
        bool onLine = false;
        offset = offset % theta;

        // need to revisit drawing algorithms
        for (int i = 0; i < 8; i++)
        {
            if(i!=4){

                if(i == 3 && y == 730){
                    Debug.Log(Mathf.Tan(theta*i + offset) * x + (pos.y - (pos.x * Mathf.Tan(theta*i + offset))));
                }
                
                onLine = y == (int)Mathf.Floor(Mathf.Tan(theta*i + offset) * x + (pos.y - (pos.x * Mathf.Tan(theta*i + offset))));
                if(onLine)
                    break;
            }
            
        }
        return onLine || x == (int)Mathf.Floor(-Mathf.Tan(offset) * y + (pos.x - (pos.y * Mathf.Tan(-offset))));
    }
}
