using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    public RawImage img;

    public Palette grayscale;
    public Palette coloured;
    public Palette weathered;

    [HideInInspector]
    public Texture2D currentMapTexture;

    public void DrawMap(float[,] noiseMap, MapSettings mapSettings){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);       
        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];

        
        switch(mapSettings.colourScheme){
            case MapColourScheme.NOISEMAP:
                colourMap = MapDrawer.DrawNoiseMap(noiseMap);
                break;
            case MapColourScheme.SIMPLE_GRYSCL:
                colourMap = MapDrawer.DrawMap(noiseMap, mapSettings, grayscale);
                break;
            case MapColourScheme.SIMPLE_COLOUR:
                colourMap = MapDrawer.DrawMap(noiseMap, mapSettings, coloured);
                break;
            case MapColourScheme.WEATHERED:
                colourMap = MapDrawer.DrawMap(noiseMap, mapSettings, weathered);
                break;
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        currentMapTexture = texture;

        img.texture = texture;

        RectTransform rect = img.gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
    }
}

[System.Serializable]
public struct Palette{
    public Color line;
    public Color backgroundLine;
    public Color border;
    public Gradient land;
    public Gradient sea;
}

