using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapType mapType;

    public int mapWidth;
    public int mapHeight;

    public int seed;

    [Range(0, 1)]
    public float seaLevel;
    
    public float scale;
    public int octaves;

    [Range(0, 1)]
    public float persisitance;
    public float lacunarity;

    public Vector2 offset;

    public bool autoUpdate; 

    public void GenerateMap(){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persisitance, lacunarity, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        switch(mapType){
            case MapType.NOISEMAP:
                display.DrawNoiseMap(noiseMap);
                break;
            case MapType.SIMPLE:
                display.DrawSimpleMap(noiseMap, seaLevel);
                break;
        }
        
    }

    void OnValidate(){
        if(mapWidth < 1){
            mapWidth = 1;
        }

        if (mapHeight < 1){
            mapHeight = 1;
        }

        if(lacunarity < 1){
            lacunarity = 1;
        }

        if(octaves < 0){
            octaves = 1;
        }
    }

    public enum MapType{
        NOISEMAP,
        SIMPLE
    }
}
