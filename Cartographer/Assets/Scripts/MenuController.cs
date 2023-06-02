using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour, IDragHandler{

    //PUBLIC
    public GameObject[] subMenus;

    public bool enableDrag = true;

    // PRIVATE

    private Canvas canvas;
    private RectTransform rect;
    
    private void Awake(){
        canvas = FindObjectOfType<Canvas>();
        rect = GetComponent<RectTransform>();
    }

    void Start(){
        foreach(GameObject menu in subMenus)
        {
            menu.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData){
        if(enableDrag)
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void HideMenu(){
        foreach(GameObject submenu in subMenus){
            submenu.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void OpenSubMenu(int index){
        for (int i = 0; i < subMenus.Length; i++)
        {
            if(i == index){
                subMenus[i].SetActive(!subMenus[i].activeSelf);
            }
        }
    }
}
