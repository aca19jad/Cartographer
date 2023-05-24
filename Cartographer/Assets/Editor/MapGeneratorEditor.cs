using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// custom editor to add a "Generate" button to the inspector of MapGenerator
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI(){
        MapGenerator mapGen = (MapGenerator) target;
        
        if(DrawDefaultInspector() && mapGen.autoUpdate){
            mapGen.GenerateMap();
        }

        if(GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}
