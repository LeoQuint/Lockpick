using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    void Start()
    {
        GameObject.Find("Dropdown").GetComponent<Dropdown>().value = (int)Data.instance.difficulty;
    }

    public void SetDifficulty()
    {
        Data.instance.SetDifficulty(GameObject.Find("Dropdown").GetComponent<Dropdown>().value);
    }

    public void StartGame()
    {
        Data.instance.LoadScene(1);
    }
}
