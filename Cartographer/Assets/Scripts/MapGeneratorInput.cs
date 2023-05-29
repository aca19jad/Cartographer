using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapGeneratorInput : MonoBehaviour
{
    public RectTransform mapRect;

    private Vector2Int scaledMousePos;

    private MapGenerator mapGen;

    void Start()
    {
        mapGen = gameObject.GetComponent<MapGenerator>();
        mapGen.autoUpdate = true;
    }

    void Update()
    {
        // rescale the mouse position coordinates to match the map size
        if(RectTransformUtility.RectangleContainsScreenPoint(mapRect, Input.mousePosition)){
            CalculateMousePosition(mapRect.localScale);
        }
    }

    public void Generate(){
        mapGen.GenerateMap();
    }

    public void ToggleAutoUpdate(Toggle toggle){
        mapGen.autoUpdate = toggle.isOn;
    }

    public void ToggleBorder(Toggle toggle){
        mapGen.mapSettings.border = toggle.isOn;
    }

    public void ToggleGridLines(Toggle toggle){
        mapGen.mapSettings.gridLines = toggle.isOn;
    }

    public void ToggleCompassRose(Toggle toggle){
        mapGen.mapSettings.compassRose = toggle.isOn;
    }

    public void UpdateMapType(TMP_Dropdown dropdown){
        switch(dropdown.value){
            case 0:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_GRYSCL;
                break;
            case 1:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_COLOUR;
                break;
            case 2:
                mapGen.mapSettings.colourScheme = MapColourScheme.WEATHERED;
                break;
        }
    }

    public void UpdateSeaLevel(Slider slider){
        mapGen.mapSettings.seaLevel = slider.value;
    }

    public void UpdateLineThickness(Slider slider){
        mapGen.mapSettings.lineThickness = (int)slider.value;
    }

    private void CalculateMousePosition(Vector3 scale){
        // rescale (0, 0) to the bottom left corner of the map instead of the screen
        Vector3 pixelOffset = new Vector3(
            (Screen.width - mapGen.mapWidth * scale.x ) / 2f,
            (Screen.height - mapGen.mapHeight * scale.y ) / 2f,
            0
        );

        // update the scaled mouse position with the new zero point and scaled to the correct width and height
        scaledMousePos.x = (int) ((Input.mousePosition.x - pixelOffset.x) / scale.x);
        scaledMousePos.y = (int) ((Input.mousePosition.y - pixelOffset.y) / scale.y);
    }
}
