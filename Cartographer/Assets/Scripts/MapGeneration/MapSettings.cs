using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapSettings
{
    public string mapName;
    public MapColourScheme colourScheme;
    public Palette palette;
    
    [Range(0, 1)]
    public float seaLevel;

    [Range(1, 5)]
    public int lineThickness;

    public bool border;
    public int borderWidth;
    
    public bool gridLines;
    public bool gridLineUnder;
    public int lineSpacing;

    public bool compassRose;
    public bool compassRoseUnder;

    public bool compassRays;
    public bool compassRaysUnder;

    public Vector2Int rosePosition;

    [Range(-Mathf.PI, Mathf.PI)]
    public float roseAngle;

    public Texture2D roseTexture;
}

// different choices of map to render
public enum MapColourScheme{
    NOISEMAP,
    SIMPLE_GRYSCL,
    SIMPLE_COLOUR, 
    WEATHERED,
}

[System.Serializable]
public struct Palette{
    public Color line;
    public Color backgroundLine;
    public Color border;
    public Gradient land;
    public Gradient sea;
}
