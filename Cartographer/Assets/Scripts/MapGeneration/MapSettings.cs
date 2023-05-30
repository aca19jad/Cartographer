using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapSettings
{
    public MapColourScheme colourScheme;

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

    public Vector2Int rosePosition;

    [Range(-Mathf.PI, Mathf.PI)]
    public float roseAngle;

    public Texture2D roseTexture;
}
