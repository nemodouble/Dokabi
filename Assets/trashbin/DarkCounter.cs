using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkCounter : MonoBehaviour
{
    GameObject darkSpawner;
    public int count = 1;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        darkSpawner = GameObject.Find("DarkCountroller");
    }

    // Update is called once per frame
    void Update()
    {
        text.text = count + "개";
    }
}
