using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public MapSettings mapSettings;

    public NoiseSettings noiseSettings;

    public bool autoUpdate; 

    public void GenerateMap(){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(noiseMap, mapSettings);
    }

    void OnValidate(){
        if(mapWidth < 1){
            mapWidth = 1;
        }

        if (mapHeight < 1){
            mapHeight = 1;
        }

        if(noiseSettings.lacunarity < 1){
            noiseSettings.lacunarity = 1;
        }

        if(noiseSettings.octaves < 0){
            noiseSettings.octaves = 1;
        }

        if(mapSettings.borderWidth < 0){
            mapSettings.borderWidth = 0;
        }

        if(mapSettings.lineSpacing < 1){
            mapSettings.lineSpacing = 1;
        }
    }
}

public enum MapColourScheme{
    NOISEMAP,
    SIMPLE_GRYSCL,
    SIMPLE_COLOUR, 
    WEATHERED,
}

[System.Serializable]
public struct MapSettings{
    public MapColourScheme colourScheme;

    [Range(0, 1)]
    public float seaLevel;

    [Range(1, 5)]
    public int lineThickness;

    public bool border;
    public int borderWidth;

    public bool gridLines;
    public int lineSpacing;
}
