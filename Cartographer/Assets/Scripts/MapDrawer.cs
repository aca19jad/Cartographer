using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapDrawer
{
    public static Color[] DrawNoiseMap(float[,] noiseMap){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){

                colourMap[x + y*width] = Color.Lerp(Color.black, Color.white,noiseMap[x, y]);
            }
        }

        return colourMap;
    }

    public static Color[] DrawMap(float[,] noiseMap, MapSettings settings, Palette palette){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
            
                if (noiseMap [x, y] < settings.seaLevel){
                    colourMap[x + y*width] = palette.sea.Evaluate(Mathf.InverseLerp(0, settings.seaLevel, noiseMap[x, y]));
                }
                else if (IsShoreline(noiseMap, x, y, settings.seaLevel, settings.lineThickness)){
                    colourMap[x + y*width] = palette.line;
                }
                else{
                    colourMap[x + y*width] = palette.land;
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

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                
                if(settings.gridLines && 
                (x % settings.lineSpacing == 0 || y % settings.lineSpacing == 0) && 
                noiseMap[x, y] < settings.seaLevel){
                    colourMap[x + width * y] = palette.line;
                }

                if( settings.border && 
                    (x < settings.borderWidth || x > width - settings.borderWidth - 1 || 
                    y < settings.borderWidth || y > height - settings.borderWidth - 1))
                {
                    colourMap[x + width * y] = (settings.colourScheme == MapColourScheme.WEATHERED) ? palette.land : Color.white;
                }
            }
        }
        return colourMap;
    }
}
