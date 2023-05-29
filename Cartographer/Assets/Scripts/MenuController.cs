using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject[] menus;

    void Start(){
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }
    
    public void OpenMenu(int index){
        for (int i = 0; i < menus.Length; i++)
        {
            if(i == index){
                menus[i].SetActive(!menus[i].activeSelf);
            }
            else{
                menus[i].SetActive(false);
            }
        }
    }
}
