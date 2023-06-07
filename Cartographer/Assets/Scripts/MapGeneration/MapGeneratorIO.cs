using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapGeneratorIO : MonoBehaviour
{
    // PUBLIC
    public RectTransform mapRect;

    public TMP_InputField seedField;

    public Palette grayscale;
    public Palette coloured;
    public Palette weathered;

    // PRIVATE
    private MapGenerator mapGen;
    private MapDisplay display;

    private Vector2Int scaledMousePos;
    private Vector3 centring;

    private bool updateCompassRosePosition;
    private bool updateCompassRoseRotation;

    void Start()
    {
        mapGen = gameObject.GetComponent<MapGenerator>();
        display = gameObject.GetComponent<MapDisplay>();

        mapGen.autoUpdate = true;

        centring = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        updateCompassRosePosition = false;
        updateCompassRoseRotation = false;

        seedField.text = mapGen.noiseSettings.seed.ToString();
    }

    void Update()
    {
        // rescale the mouse position coordinates to match the map size
        CalculateMousePosition(mapRect.localScale);
    
        if(mapGen.mapSettings.compassRose){
            UpdateCompassRose();
        }
    }

    #region [Public UI callbacks]
    public void Generate(){
        mapGen.noiseSettings.seed = System.DateTime.Now.ToString().GetHashCode();
        mapGen.GenerateMap();
        seedField.text = mapGen.noiseSettings.seed.ToString();
    }
    
    // TOGGLES

    public void ToggleAutoUpdate(Toggle toggle){
        mapGen.autoUpdate = toggle.isOn;
    }

    public void ToggleBorder(Toggle toggle){
        mapGen.mapSettings.border = toggle.isOn;
    }

    public void ToggleGridLines(Toggle toggle){
        mapGen.mapSettings.gridLines = toggle.isOn;
    }

    public void ToggleGridLinesUnderLand(Toggle toggle){
        mapGen.mapSettings.gridLineUnder = toggle.isOn;
    }

    public void ToggleCompassRose(Toggle toggle){
        mapGen.mapSettings.compassRose = toggle.isOn;
        updateCompassRosePosition = toggle.isOn;
    }

    public void ToggleCompassRoseUnderLand(Toggle toggle){
        mapGen.mapSettings.compassRoseUnder = toggle.isOn;
    }

    // BUTTONS

    public void RepositionCompassRose(){
        updateCompassRosePosition = true;
    }

    public void ReorientCompassRose(){
        updateCompassRoseRotation = true;
    }

    // SLIDERS

    public void UpdateSeaLevel(Slider slider){
        mapGen.mapSettings.seaLevel = slider.value;
    }

    public void UpdateLineThickness(Slider slider){
        mapGen.mapSettings.lineThickness = (int) slider.value;
    }

    public void UpdateGridLineSpacing(Slider slider){
        mapGen.mapSettings.lineSpacing = (int) slider.value;
    }

    public void UpdateBorderThickness(Slider slider){
        mapGen.mapSettings.borderWidth = (int) slider.value;
    }

    // OTHER

    public void UpdateSeed(TMP_InputField field){
        mapGen.noiseSettings.seed = int.Parse(field.text);
    }

    public void UpdateColourScheme(TMP_Dropdown dropdown){

        switch(dropdown.value){
            case 0:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_GRYSCL;
                mapGen.mapSettings.palette = grayscale;
                break;
            case 1:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_COLOUR;
                mapGen.mapSettings.palette = coloured;
                break;
            case 2:
                mapGen.mapSettings.colourScheme = MapColourScheme.WEATHERED;
                mapGen.mapSettings.palette = weathered;
                break;

            case 4:
                mapGen.mapSettings.colourScheme = MapColourScheme.NOISEMAP;
                break;
        }

    }
    #endregion

    // function that scales the mouse position from screen space to map space
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

    // Update callbacks triggered by input for the compass rose
    private void UpdateCompassRose(){
        // updates position
        if(updateCompassRosePosition){
            mapGen.mapSettings.rosePosition = scaledMousePos;
        }

        // updates rotation, sets the angle between 0 - 360 degrees
        if(updateCompassRoseRotation){
            float angle = 180 + Vector3.SignedAngle(
                Vector3.up, 
                new Vector3(
                    mapGen.mapSettings.rosePosition.x, 
                    mapGen.mapSettings.rosePosition.y, 0) - 
                new Vector3(
                    scaledMousePos.x, 
                    scaledMousePos.y, 0), 
                Vector3.forward);

            mapGen.mapSettings.roseAngle = Mathf.Deg2Rad * -angle;
        }

        // if the left mouse button is clicked once: stop updating position and start rotation
        if(Input.GetMouseButtonDown(0) && updateCompassRosePosition){
            updateCompassRosePosition = false;
            updateCompassRoseRotation = true;
        }
        // if the left mouse button is clicked again: stop updating rotation 
        else if(Input.GetMouseButtonDown(0) && updateCompassRoseRotation){
            updateCompassRoseRotation = false;
        }
    }
}
