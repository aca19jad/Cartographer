using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapGeneratorIO : MonoBehaviour
{
    // PUBLIC
    public RectTransform mapRect;
    public GameObject compassRoseObj;

    public TMP_InputField seedField;

    // PRIVATE
    private MapGenerator mapGen;
    private MapDisplay display;

    private Vector2Int scaledMousePos;

    private bool updateCompassRosePosition;
    private bool updateCompassRoseRotation;

    private RawImage compassImg;

    void Start()
    {
        mapGen = gameObject.GetComponent<MapGenerator>();
        display = gameObject.GetComponent<MapDisplay>();

        compassImg = compassRoseObj.GetComponent<RawImage>();
        mapGen.autoUpdate = true;

        updateCompassRosePosition = false;
        updateCompassRoseRotation = false;
        compassRoseObj.SetActive(false);

        seedField.text = mapGen.noiseSettings.seed.ToString();
    }

    void Update()
    {
        // rescale the mouse position coordinates to match the map size
        CalculateMousePosition(mapRect.localScale);
        Debug.Log(Input.mousePosition - new Vector3(Screen.width/2f, Screen.height/2f, 0));
    
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
        updateCompassRosePosition = toggle.isOn;
        compassRoseObj.SetActive(toggle.isOn);
    }

    public void RepositionCompassRose(){
        updateCompassRosePosition = true;
    }

    public void ReorientCompassRose(){
        updateCompassRoseRotation = true;
    }

    public void UpdateSeed(TMP_InputField field){
        mapGen.noiseSettings.seed = int.Parse(field.text);
    }

    public void UpdateMapType(TMP_Dropdown dropdown){
        Color compassColor = new Color();
        switch(dropdown.value){
            case 0:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_GRYSCL;
                compassColor = display.grayscale.backgroundLine;         
                break;
            case 1:
                mapGen.mapSettings.colourScheme = MapColourScheme.SIMPLE_COLOUR;
                compassColor = display.coloured.backgroundLine;
                break;
            case 2:
                mapGen.mapSettings.colourScheme = MapColourScheme.WEATHERED;
                compassColor = display.weathered.backgroundLine;
                break;
        }
        compassColor.a = 1f;
        compassImg.color = compassColor;
    }

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
            compassRoseObj.transform.position = (Input.mousePosition - new Vector3(Screen.width/2f, Screen.height/2f, 0)) / (Screen.height / 10f);
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
            compassRoseObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // if the left mouse button is clicked once: stop updating position and start rotation
        if(Input.GetMouseButtonDown(0) && updateCompassRosePosition){
            updateCompassRosePosition = false;
            updateCompassRoseRotation = true;
        }
        else if(Input.mouseScrollDelta.y != 0 && updateCompassRosePosition){
            compassRoseObj.transform.localScale = Vector3.Max(
                compassRoseObj.transform.localScale + Vector3.one * Input.mouseScrollDelta.y * 0.1f,
                Vector3.one * 0.1f);
        }
        // if the left mouse button is clicked again: stop updating rotation 
        else if(Input.GetMouseButtonDown(0) && updateCompassRoseRotation){
            updateCompassRoseRotation = false;
        }
    }
}
