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
    private bool hideUnderLand;

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
        hideUnderLand = false;

        centring = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        img.enabled = false;

        img.texture = mapIO.GetRoseTexture();
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
                if(hideUnderLand){
                    UpdateCompassVisibility();
                }
            } 
        }
        // adds the ability to scale the compass rose while the position or orientation is being adjusted
        else if(Input.mouseScrollDelta.y != 0 && (updateCompassRosePosition || updateCompassRoseRotation)){
            // ads a minimum value so the rose scale cannot be negative
            transform.localScale = Vector3.Max(
                transform.localScale + Vector3.one * Input.mouseScrollDelta.y * 0.1f,
                Vector3.one * 0.1f);
        }

        UpdateCompassRose();
    }

    private void UpdateCompassRose(){
        // perform updates if the relevant flag is true
        if(updateCompassRosePosition){
            rect.localPosition = Input.mousePosition - centring;
        }
        else if(updateCompassRoseRotation){
            float angle = 180 + Vector3.SignedAngle(Vector3.up, rect.localPosition - Input.mousePosition + centring, Vector3.forward);
            transform.rotation = Quaternion.Euler(0,0,angle);
        }

        img.color = mapIO.GetPalette().backgroundLine;
    }

    private void UpdateCompassVisibility(){
        
        Texture2D newTex = new Texture2D(mapIO.GetRoseTexture().width, mapIO.GetRoseTexture().height);
        newTex.SetPixels(mapIO.GetRoseTexture().GetPixels());
        newTex.wrapMode = TextureWrapMode.Clamp;
        newTex.filterMode = FilterMode.Bilinear;
        newTex.Apply();


        Color[] compassColours = newTex.GetPixels();
        float[,] heightMap = mapIO.GetHeightMap();
        Vector2Int pos = new Vector2Int();
        Vector3 texelPos = new Vector3();

        for (int y = 0, index=0; y < img.texture.height; y++)
        {
            for (int x = 0; x < img.texture.width; x++, index++)
            {
                float relativeX = x-(img.texture.width / 2f);
                float relativeY = y-(img.texture.height / 2f);

                float angle = (rect.localRotation.eulerAngles.z * Mathf.Deg2Rad);

                texelPos = new Vector3(
                    (relativeX * Mathf.Cos(angle) - relativeY * Mathf.Sin(angle)) * rect.localScale.x, 
                    (relativeY * Mathf.Cos(angle) + relativeX * Mathf.Sin(angle)) * rect.localScale.y,
                    0);

                if(compassColours[index].a > 0){
                    Vector3 compassPoint = rect.localPosition + new Vector3(Screen.width/2f, Screen.height/2f, 0);
                    pos = mapIO.ScreenPointToMapCoord(compassPoint + texelPos);

                    if(pos.x >= 0 && pos.x < heightMap.GetLength(0) && pos.y >= 0 && pos.y < heightMap.GetLength(0)){
                        if(heightMap[pos.x, pos.y] < mapIO.GetSeaLevel() && hideUnderLand){
                            compassColours[index].a = 1;
                        }
                        else{
                            compassColours[index].a = 0;
                        }
                    }
                    else{
                        compassColours[index].a = 0;
                    }
                    
                    
                }
            }
        }

        newTex.SetPixels(0, 0, img.texture.width, img.texture.height, compassColours, 0);
        newTex.Apply();

        img.texture = newTex;
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

    public void UpdateCompassRoseUnderLand(Toggle toggle){
        hideUnderLand = toggle.isOn;
        UpdateCompassVisibility();
    }
}
