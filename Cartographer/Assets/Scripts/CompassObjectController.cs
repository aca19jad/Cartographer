using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassObjectController : MonoBehaviour
{
    // PUBLIC
    public MapGeneratorIO mapIO;

    // PRIVATE
    private bool updateCompassRosePosition;
    private bool updateCompassRoseRotation;

    private Vector3 centring;

    private RectTransform rect;
    private RawImage img;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<RawImage>();

        updateCompassRosePosition = false;
        updateCompassRoseRotation = false;

        centring = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {  
        // Input handlers 
        // if the left mouse button is clicked once: stop updating position and start rotation
        if(Input.GetMouseButtonDown(0)){
            if(updateCompassRosePosition){
                updateCompassRosePosition = false;
                updateCompassRoseRotation = true;
            }
            // if the left mouse button is clicked again: stop updating rotation 
            else if (updateCompassRoseRotation){
            updateCompassRoseRotation = false;
            // find which pixels to hide from the height map
            } 
        }
        // adds the ability to scale the compass rose while the position or orientation is being adjusted
        else if(Input.mouseScrollDelta.y != 0 && (updateCompassRosePosition || updateCompassRoseRotation)){
            // ads a minimum value so the rose scale cannot be negative
            transform.localScale = Vector3.Max(
                transform.localScale + Vector3.one * Input.mouseScrollDelta.y * 0.1f,
                Vector3.one * 0.1f);
        }

        // perform updates if the relevant flag is true
        if(updateCompassRosePosition){
            rect.localPosition = Input.mousePosition - centring;
        }
        else if(updateCompassRoseRotation){
            float angle = 180 + Vector3.SignedAngle(Vector3.up, rect.localPosition - Input.mousePosition + centring, Vector3.forward);
            transform.rotation = Quaternion.Euler(0,0,angle);
        }
    }

    
    // public variables to be called from the UI buttons
    public void ToggleCompassRose(Toggle toggle){
        img.enabled = toggle.isOn;
        updateCompassRosePosition = toggle.isOn;
    }

    public void RepositionCompassRose(){
        updateCompassRosePosition = true;
    }

    public void ReorientCompassRose(){
        updateCompassRoseRotation = true;
    }
}
