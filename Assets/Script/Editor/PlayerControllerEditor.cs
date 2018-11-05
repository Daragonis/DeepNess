using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// Uncomment the following line after replacing "MyScript" with your script name:
[CustomEditor(typeof(PlayerController))]
[CanEditMultipleObjects]
public class PlayerControllerEditor : Editor
{

    bool showBtn = false;
    public override void OnInspectorGUI()
    {
        PlayerController myTarget = (PlayerController)target;
        GUILayout.Label("General Parameters");
        GUILayout.Space(5);
        myTarget.moveSpeedMultiplier = EditorGUILayout.Slider("Movement Speed", myTarget.moveSpeedMultiplier, 0.01f, 4);
        myTarget.swimSpeedMultiplier = EditorGUILayout.Slider("Swim Speed", myTarget.swimSpeedMultiplier, 0.01f, 4);
        GUILayout.Space(20);
        GUILayout.Label("Fight Parameters");
        GUILayout.Space(5);
        myTarget.healthPoint = EditorGUILayout.IntField("Health Point", myTarget.healthPoint);
        myTarget.damageHitBox[0] = EditorGUILayout.Vector2Field("Damage Hitbox Up Left", myTarget.damageHitBox[0]);
        myTarget.damageHitBox[1] = EditorGUILayout.Vector2Field("Damage Hitbox Down Right", myTarget.damageHitBox[1]);
        GUILayout.Space(20);
        GUILayout.Label("Jump Parameters");
        GUILayout.Space(5);
        myTarget.gravityMultiplier = EditorGUILayout.Slider("Gravity", myTarget.gravityMultiplier, 0.01f, 4);
        myTarget.jumpStrengthMultiplier = EditorGUILayout.Slider("Jump Strength", myTarget.jumpStrengthMultiplier, 0.01f, 4);
        GUILayout.BeginHorizontal();
        myTarget.jumpChartMaxTime = EditorGUILayout.FloatField("Jump Chart Max Time", myTarget.jumpChartMaxTime);
        myTarget.jumpChartNumberOfIncretions = EditorGUILayout.IntField("Number of Increment", myTarget.jumpChartNumberOfIncretions);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        myTarget.jumpChartSpeedDeceleration = EditorGUILayout.Slider("Deceleration Multiplier", myTarget.jumpChartSpeedDeceleration, 0.01f, 1);
        GUILayout.EndHorizontal();

        showBtn = EditorGUILayout.Toggle("Toggle Jump Explanation", showBtn);
        if (showBtn)
        {
            GUILayout.Label("Le saut fonctionne selon 5 paramètres.");
            GUILayout.Label("Gravity détermine à quelle vitesse le personnage sera attiré au \nsol.");
            GUILayout.Label("Jump Strength détermine la hauteur que le personnage gagne \nlors de la première frame de son saut");
            GUILayout.Label("Jump Chart Time détermine le temps que dure le nuancier de \nsaut. C'est à dire le temps maximal pendant laquel après avoir \nappuyé sur le saut, le saut continue à gagner en hauteur si on \nmaintiens le bouton appuyé");
            GUILayout.Label("Pendant le nuancier de saut, le personnage gagne de la vitesse \nun nombre de fois correspondant à Number of Increment, les \ngains de vitesse sont répartis équitablement sur toute la durée \ndu nuancier.");
            GUILayout.Label("Chaque ajout de vitesse injecté par Number of Increment est \nréduit en étant multiplié à Deceleration Multiplier. Plus la valeur \nest basse, moins le nuancier a d'effet et vice-versa.");
        }

        GUILayout.Space(20);
        GUILayout.Label("Graphical Parameters");
        GUILayout.Space(5);
        myTarget.spriteAxolotl = (Sprite)EditorGUILayout.ObjectField("Axolotl Spritesheet", myTarget.spriteAxolotl, typeof(Sprite), false);
        myTarget.spriteSlime = (Sprite)EditorGUILayout.ObjectField("Slime Spritesheet", myTarget.spriteSlime, typeof(Sprite), false);
        myTarget.slashPrefab = (GameObject)EditorGUILayout.ObjectField("Slash FX Prefab", myTarget.slashPrefab, typeof(GameObject), false);

        GUILayout.Space(20);
        GUILayout.Label("Sound Parameters");
        GUILayout.Space(5);
        Undo.RecordObject(myTarget, "myTargetChange");
    }
    void OnSceneGUI()
    {
        PlayerController myTarget = (PlayerController)target;
        Vector3 rectangleLeftUp = new Vector3(myTarget.damageHitBox[0].x + myTarget.transform.position.x, myTarget.damageHitBox[0].y + myTarget.transform.position.y);
        Vector3 rectangleLeftDown = new Vector3(myTarget.damageHitBox[0].x + myTarget.transform.position.x, myTarget.damageHitBox[1].y + myTarget.transform.position.y);
        Vector3 rectangleRightUp = new Vector3(myTarget.damageHitBox[1].x + myTarget.transform.position.x, myTarget.damageHitBox[0].y + myTarget.transform.position.y);
        Vector3 rectangleRightDown = new Vector3(myTarget.damageHitBox[1].x + myTarget.transform.position.x, myTarget.damageHitBox[1].y + myTarget.transform.position.y);
        Handles.DrawLine(rectangleLeftUp, rectangleLeftDown);
        Handles.DrawLine(rectangleLeftUp, rectangleRightUp);
        Handles.DrawLine(rectangleLeftDown, rectangleRightDown);
        Handles.DrawLine(rectangleRightUp, rectangleRightDown);
    }
}
