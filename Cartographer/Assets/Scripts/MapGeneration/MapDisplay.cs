using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    public RawImage img;

    [HideInInspector]
    //public Texture2D currentMapTexture;

    public void DrawMap(float[,] noiseMap, MapSettings mapSettings){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);       
        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];

        if(mapSettings.colourScheme == MapColourScheme.NOISEMAP){
            colourMap = MapDrawer.DrawNoiseMap(noiseMap);
        }
        else{
            colourMap = MapDrawer.DrawMap(noiseMap, mapSettings);
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        //currentMapTexture = texture;

        img.texture = texture;

        RectTransform rect = img.gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
    }
}


