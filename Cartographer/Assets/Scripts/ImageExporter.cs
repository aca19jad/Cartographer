using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class ImageExporter : MonoBehaviour
{
    //PUBLIC
    public Camera saveCam;
    public GameObject menus;

    public Canvas canvas;
    public Transform mapTF;
    public Transform OverlayPanelTF;

    //public RenderTexture rTex;

    //PRIVATE

    private Vector3 mapScale;
    private MapGenerator mapGen;
    
    void Start(){
        canvas.worldCamera = Camera.main;
        mapScale = mapTF.localScale;
        mapGen = FindObjectOfType<MapGenerator>();
    }

    public void ExportImage(string fileType){
        menus.SetActive(false);
        canvas.worldCamera = saveCam;
        mapTF.localScale = Vector3.one;
        OverlayPanelTF.localPosition = OverlayPanelTF.localPosition / mapScale.x;
        OverlayPanelTF.localScale = OverlayPanelTF.localScale / mapScale.x;

        if(saveCam.targetTexture != null){
            saveCam.targetTexture.Release();
        }

        saveCam.targetTexture = new RenderTexture(mapGen.mapWidth, mapGen.mapHeight, 24);

        saveCam.Render();

        Texture2D saveTex = toTexture2D(saveCam.targetTexture); 
        saveTex.Apply();

        if(fileType == "png"){
            SaveBytesToFile(saveTex.EncodeToPNG(), ".png", Application.dataPath + "/../Maps/");
        }
        else if(fileType == "jpg"){
            SaveBytesToFile(ImageConversion.EncodeToJPG(saveTex), ".jpg", Application.dataPath + "/../Maps/");
        }
            

        Object.Destroy(saveTex);
        //saveCam.targetTexture.Release();

        mapTF.localScale = mapScale;
        OverlayPanelTF.localScale = OverlayPanelTF.localScale * mapScale.x;
        OverlayPanelTF.localPosition = OverlayPanelTF.localPosition * mapScale.x;
        menus.SetActive(true);
        canvas.worldCamera = Camera.main;
    }

    private Texture2D toTexture2D(RenderTexture rTex){
        Texture2D tex = new Texture2D(mapGen.mapWidth, mapGen.mapHeight, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private void SaveBytesToFile(byte[] bytes, string fileType, string dirPath){
        if(!Directory.Exists(dirPath)){
            Directory.CreateDirectory(dirPath);
        }

        File.WriteAllBytes(dirPath + "Image" + Random.Range(1, 1000).ToString() + fileType, bytes);
    }
}
