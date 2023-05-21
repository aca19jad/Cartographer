using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapColourScheme colourScheme;

    public int mapWidth;
    public int mapHeight;

    [Range(1,5)]
    public int lineThickness = 1;
    public bool border = false;
    public bool gridLines = false;

    [Range(0,1)]
    public float seaLevel;

    public Vector2 offset;

    public NoiseSettings noiseSettings;

    public bool autoUpdate; 

    public void GenerateMap(){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(noiseMap, colourScheme, lineThickness, seaLevel, border, gridLines);
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
    }
}

public enum MapColourScheme{
    NOISEMAP,
    SIMPLE_GRYSCL,
    SIMPLE_COLOUR, 
    WEATHERED,
}
