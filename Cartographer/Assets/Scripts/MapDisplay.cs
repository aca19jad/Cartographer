using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRend;

    [Range(1, 20)]
    public int lineThickness = 1;

    public Color sea;
    public Color land;
    public Color line;

    public void DrawNoiseMap(float[,] noiseMap){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                colourMap[x + y*width] = Color.Lerp(Color.black, Color.white,noiseMap[x, y]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        textureRend.sharedMaterial.mainTexture = texture;
        textureRend.transform.localScale = new Vector3((float) width/height, 1, 1);
    }

    public void DrawSimpleMap(float[,] noiseMap, float seaLevel){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){

                
                if (noiseMap [x, y] < seaLevel){
                    colourMap[x + y*width] = sea;
                }
                else if (IsShoreline(noiseMap, seaLevel, lineThickness, x, y)){
                    colourMap[x + y*width] = line;
                }
                else{
                    colourMap[x + y*width] = land;
                }
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        textureRend.sharedMaterial.mainTexture = texture;
        textureRend.transform.localScale = new Vector3((float) width/height, 1, 1);
        
    }

    bool IsShoreline(float[,] noiseMap, float seaLevel, int lineThickness, int xPos, int yPos){
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

