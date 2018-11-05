using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    // --- UI Manager --- \\
    // Manage the UI during the game. \\

    // --- Variables of UIManager --- \\
    Transform life;
    Transform menu;

    // Use this for initialization
    void Start () {
        life = transform.GetChild(1);
        menu = transform.GetChild(2);
	}

    public void ChangeLife(int newLifeValue) {
        for (var i = 0; i < 3; i++) {
            if (i < newLifeValue) 
            life.GetChild(i).GetComponent<Image>().enabled = true;
            else life.GetChild(i).GetComponent<Image>().enabled = false;
        }
    }

    public void LaunchMenu() {
        if (menu.GetChild(0).GetComponent<Text>().isActiveAndEnabled)
        menu.GetChild(0).GetComponent<Text>().enabled = false;
        else menu.GetChild(0).GetComponent<Text>().enabled = true;
    }
}
