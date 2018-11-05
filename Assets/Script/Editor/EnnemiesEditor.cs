using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Uncomment the following line after replacing "MyScript" with your script name:
[CustomEditor(typeof(Ennemies))]
[CanEditMultipleObjects]
public class EnnemiesEditor : Editor {

    SerializedProperty type;

    public override void OnInspectorGUI() {
        Ennemies myTarget = (Ennemies)target;
        GUILayout.Label("Enemies Parameters");
        GUILayout.Space(5);

        myTarget.typeOfCreature = (Ennemies.CreatureType)EditorGUILayout.EnumPopup("Creature Type", myTarget.typeOfCreature);

        if(myTarget.typeOfCreature != Ennemies.CreatureType.Projectile) { 
        GUILayout.BeginHorizontal();
        myTarget.healthPoint = EditorGUILayout.IntField("Health Point", myTarget.healthPoint);
        myTarget.moveSpeed = EditorGUILayout.FloatField("Move Speed", myTarget.moveSpeed);
        GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        myTarget.knockBackStrength = EditorGUILayout.FloatField("Knockback Strength", myTarget.knockBackStrength);
        myTarget.damageDealt = EditorGUILayout.IntField("Damage Dealt", myTarget.damageDealt);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (myTarget.typeOfCreature == Ennemies.CreatureType.Grounded)
            myTarget.gravity = EditorGUILayout.FloatField("Gravity", myTarget.gravity);
        else myTarget.gravity = 0;
        if (myTarget.typeOfCreature != Ennemies.CreatureType.Projectile)
            myTarget.attackRange = EditorGUILayout.FloatField("Attack Range", myTarget.attackRange);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        if (myTarget.typeOfCreature != Ennemies.CreatureType.Projectile) {
            myTarget.canShootProjectile = EditorGUILayout.Toggle("Shoot Projectile ?", myTarget.canShootProjectile);
            if (myTarget.canShootProjectile) {
                myTarget.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", myTarget.projectile, typeof(GameObject), false);
                GUILayout.BeginHorizontal();
                myTarget.timeBetweenShoot = EditorGUILayout.FloatField("Time Between Shoot", myTarget.timeBetweenShoot);
                myTarget.shootSpeed = EditorGUILayout.FloatField("Shoot Velocity", myTarget.shootSpeed);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }


            myTarget.patrol = EditorGUILayout.Toggle("Activate Patrol", myTarget.patrol);
            if (myTarget.patrol) {
                GUILayout.BeginHorizontal();
                myTarget.patrolCenter = EditorGUILayout.FloatField("Patrol Center", myTarget.patrolCenter);
                myTarget.patrolScale = EditorGUILayout.FloatField("Patrol Scale", myTarget.patrolScale);
                GUILayout.EndHorizontal();
            }
        }
        Undo.RecordObject(myTarget, "myTargetChange");
    }

    void OnSceneGUI() {
        Ennemies myTarget = (Ennemies)target;
        if (myTarget.patrol) {
            Vector3 leftCoordinate = new Vector3(myTarget.patrolCenter - myTarget.patrolScale / 2, myTarget.transform.position.y);
            Vector3 rightCoordinate = new Vector3(myTarget.patrolCenter + myTarget.patrolScale / 2, myTarget.transform.position.y);
            Handles.DrawLine(leftCoordinate, rightCoordinate);
        }
    }
}
