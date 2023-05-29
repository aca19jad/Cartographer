using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSaver : MonoBehaviour
{
    //PUBLIC

    public Camera saveCam;
    public GameObject menus;

    public Canvas canvas;

    //PRIVATE
    
    void Start(){
        saveCam.gameObject.SetActive(false);
        canvas.worldCamera = Camera.main;
    }

    public void SaveImage(){
        menus.SetActive(false);
        saveCam.gameObject.SetActive(true);
        canvas.worldCamera = saveCam;
    }
}
