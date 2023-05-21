using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRend;

    public Palette grayscale;
    public Palette coloured;
    public Palette weathered;

    [HideInInspector]
    public Texture2D currentMapTexture;

    public void DrawMap(float[,] noiseMap, MapColourScheme colourScheme, int lineThickness, float seaLevel){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);       
        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];

        switch(colourScheme){
            case MapColourScheme.NOISEMAP:
                colourMap = DrawNoiseMap(noiseMap);
                break;
            case MapColourScheme.SIMPLE_GRYSCL:
                colourMap = DrawSimpleMap(noiseMap, lineThickness, seaLevel, grayscale);
                break;
            case MapColourScheme.SIMPLE_COLOUR:
                colourMap = DrawSimpleMap(noiseMap, lineThickness, seaLevel, coloured);
                break;
            case MapColourScheme.WEATHERED:
                colourMap = DrawSimpleMap(noiseMap, lineThickness, seaLevel, weathered);
                break;
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        currentMapTexture = texture;

        textureRend.sharedMaterial.mainTexture = texture;
        textureRend.transform.localScale = new Vector3((float) width/height, 1, 1)  * 0.95f;
    }

    private Color[] DrawNoiseMap(float[,] noiseMap){
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

    private Color[] DrawSimpleMap(float[,] noiseMap, int lineThickness, float seaLevel, Palette palette){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
            
                if (noiseMap [x, y] < seaLevel){
                    colourMap[x + y*width] = palette.sea.Evaluate(Mathf.InverseLerp(0, seaLevel, noiseMap[x, y]));
                }
                else if (IsShoreline(noiseMap, seaLevel, lineThickness, x, y)){
                    colourMap[x + y*width] = palette.line;
                }
                else{
                    colourMap[x + y*width] = palette.land;
                }
            }
        }

        return colourMap;
    }

    private bool IsShoreline(float[,] noiseMap, float seaLevel, int lineThickness, int xPos, int yPos){
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
}

[System.Serializable]
public struct Palette{
    public Color line;
    public Color land;
    public Gradient sea;
}

