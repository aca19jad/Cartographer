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
    public Transform compassTF;

    public RenderTexture rTex;

    //PRIVATE

    private Vector3 mapScale;
    
    void Start(){
        canvas.worldCamera = Camera.main;
        mapScale = mapTF.localScale;
    }

    public void ExportImage(){
        menus.SetActive(false);
        canvas.worldCamera = saveCam;
        mapTF.localScale = Vector3.one;
        compassTF.localPosition = compassTF.localPosition / mapScale.x;
        compassTF.localScale = compassTF.localScale / mapScale.x;

        saveCam.Render();

        Texture2D saveTex = toTexture2D(rTex); 
        saveTex.Apply();

        ExportAsPNG(saveTex);

        mapTF.localScale = mapScale;
        compassTF.localScale = compassTF.localScale * mapScale.x;
        compassTF.localPosition = compassTF.localPosition * mapScale.x;
        menus.SetActive(true);
        canvas.worldCamera = Camera.main;
    }

    private Texture2D toTexture2D(RenderTexture rTex){
        Texture2D tex = new Texture2D(1000, 750, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private void ExportAsPNG(Texture2D tex){
        byte[] bytes = tex.EncodeToPNG();

        string dirPath = Application.dataPath + "/../Maps/";

        if(!Directory.Exists(dirPath)){
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + Random.Range(1, 1000).ToString() + ".png", bytes);
    }
}
