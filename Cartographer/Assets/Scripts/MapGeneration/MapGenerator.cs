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

    MapDisplay display;
    private int check_mapWidth;
    private int check_mapHeight;
    private MapSettings check_mapSettings;
    private NoiseSettings check_noiseSettings;

    private float[,] noiseMap;

    void Start(){
        display = FindObjectOfType<MapDisplay>();
        noiseSettings.seed = 1;//System.DateTime.Now.ToString().GetHashCode();

        Debug.Log(noiseSettings.seed);

        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings);
        display.DrawMap(noiseMap, mapSettings);

        UpdateCheckVariables();
    }

    void Update(){
        if(autoUpdate && CheckAllSettings()){
            //Debug.Log("Auto Updating");
            GenerateMap();
        }
    }

    // callback to generate a map with given settings
    public void GenerateMap(){
        if(CheckAllSettings()){
            if(CheckNoiseMapSettings()){
                Debug.Log("Updating Noise Map...");
                noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseSettings);
            }
                
            display.DrawMap(noiseMap, mapSettings);
            UpdateCheckVariables();
        }
    }

    private bool CheckNoiseMapSettings(){
        return !(
            mapWidth == check_mapWidth && 
            mapHeight == check_mapHeight && 
            noiseSettings.Equals(check_noiseSettings));
    }

    private bool CheckAllSettings(){
        return!(
            mapWidth == check_mapWidth && 
            mapHeight == check_mapHeight && 
            noiseSettings.Equals(check_noiseSettings) &&
            mapSettings.Equals(check_mapSettings));
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

    private void UpdateCheckVariables(){
        check_mapWidth = mapWidth;
        check_mapHeight = mapHeight;
        check_mapSettings = mapSettings;
        check_noiseSettings = noiseSettings;
    }
}

// different choices of map to render
public enum MapColourScheme{
    NOISEMAP,
    SIMPLE_GRYSCL,
    SIMPLE_COLOUR, 
    WEATHERED,
}

