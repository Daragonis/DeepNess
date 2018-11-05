using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Uncomment the following line after replacing "MyScript" with your script name:
[CustomEditor(typeof(Water))]
[CanEditMultipleObjects]
public class WaterEditor : Editor {

    public override void OnInspectorGUI() {
        Water myTarget = (Water)target;
        Object waterm = myTarget.watermesh as Object;
        myTarget.WaterCoordinates[0] = EditorGUILayout.FloatField("X Position", myTarget.WaterCoordinates[0]);
        myTarget.WaterCoordinates[1] = EditorGUILayout.FloatField("X Scale", myTarget.WaterCoordinates[1]);
        myTarget.WaterCoordinates[3] = EditorGUILayout.FloatField("Y BottomPosition", myTarget.WaterCoordinates[3]);
        myTarget.WaterCoordinates[2] = EditorGUILayout.FloatField("Y TopPosition", myTarget.WaterCoordinates[2]);
        myTarget.watermesh = (GameObject)EditorGUILayout.ObjectField("Mesh for Water", myTarget.watermesh, typeof(GameObject), false);
        myTarget.splash = (GameObject)EditorGUILayout.ObjectField("Splash FX", myTarget.splash, typeof(GameObject), false);
        Undo.RecordObject(myTarget, "myTargetChange");
    }

    void OnSceneGUI() {
        Water myTarget = (Water)target;
        Vector3 rectangleCoordinate = new Vector3(myTarget.WaterCoordinates[0], myTarget.WaterCoordinates[3]);
        Vector3 rectangleXscale = new Vector3(myTarget.WaterCoordinates[0] + myTarget.WaterCoordinates[1], myTarget.WaterCoordinates[3]);
        Vector3 rectangleYscale = new Vector3(myTarget.WaterCoordinates[0], myTarget.WaterCoordinates[2]);
        Vector3 rectangleEnd = new Vector3(myTarget.WaterCoordinates[0] + myTarget.WaterCoordinates[1], myTarget.WaterCoordinates[2]);
        Handles.DrawLine(rectangleCoordinate, rectangleXscale);
        Handles.DrawLine(rectangleCoordinate, rectangleYscale);
        Handles.DrawLine(rectangleXscale, rectangleEnd);
        Handles.DrawLine(rectangleYscale, rectangleEnd);
    }
}
