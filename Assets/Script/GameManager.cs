using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // --- GameManager --- \\
    // Manage the global parameters of Gameplay. \\

    // --- Variables of GameManager --- \\
    GameObject canvas;
    [HideInInspector]public bool Freeze = false;
    GameObject player;
    Transform ennemiesGroup;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas");
        player = GameObject.Find("PlayerController");
        ennemiesGroup = GameObject.Find("[Ennemies]").transform;
        for (int i = 0; i < ennemiesGroup.childCount; i++) {
            ennemiesGroup.GetChild(i).GetComponent<Ennemies>().player = player;
        }
    }

    public UIManager GetCanvas() {
        return canvas.GetComponent<UIManager>();
    }

    public void LaunchMenu() {
        canvas.GetComponent<UIManager>().LaunchMenu();
        Freeze = !Freeze;
        if (Freeze) {
            player.GetComponent<PlayerController>().Freeze();
            Time.timeScale = 0;
        }
        else {
            player.GetComponent<PlayerController>().Unfreeze();
            Time.timeScale = 1;
        }
    }

    public void Respawn() {
        for (int i = 0; i < ennemiesGroup.childCount; i++) {
            ennemiesGroup.GetChild(i).GetComponent<Ennemies>().Respawn();
        }
    }

    public void Instance(GameObject slashPrefab, Transform parent)
    {
        GameObject slashEffect = Instantiate(slashPrefab, parent);
    }
}
