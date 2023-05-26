using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapGeneratorInput : MonoBehaviour
{
    public Vector2 scaledMousePos;

    private MapGenerator mapGen;
    // Start is called before the first frame update
    void Start()
    {
        mapGen = gameObject.GetComponent<MapGenerator>();
        mapGen.autoUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        scaledMousePos = Input.mousePosition;
        Debug.Log(scaledMousePos);
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
}
