using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // PUBLIC
    public int mapWidth;
    public int mapHeight;

    public MapSettings mapSettings;

    public NoiseSettings noiseSettings;

    public bool autoUpdate; 

    //PRIVATE
    private int check_mapWidth;
    private int check_mapHeight;

    private MapSettings check_mapSettings;

    private NoiseSettings check_noiseSettings;

    private float[,] noiseMap;

    void Start(){
        autoUpdate = false;

        
    }

    void Update(){
        if(autoUpdate){
            Debug.Log("AutoUpdating");
        }
    }

    // callback to generate a map with given settings
    public void GenerateMap(){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMap(noiseMap, mapSettings);
    }


    public void ToggleAutoUpdate(){
        autoUpdate = !autoUpdate;
    }

    // Editor only function to bound parameters
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

// different choices of map to render
public enum MapColourScheme{
    NOISEMAP,
    SIMPLE_GRYSCL,
    SIMPLE_COLOUR, 
    WEATHERED,
}

// struct to store all of the cosmetic settings for a map
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

    public bool compassRose;

    public Vector2Int rosePosition;

    [Range(-Mathf.PI, Mathf.PI)]
    public float roseAngle;
}
