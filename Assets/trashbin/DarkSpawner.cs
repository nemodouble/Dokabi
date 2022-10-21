using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSpawner : MonoBehaviour
{
    GameObject text;
    [SerializeField] GameObject dark = null;
    float maxCool = 30.0f;
    float curCool = 0f;
    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("Text");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 randomPos = GameObject.Find("Player").gameObject.transform.position;
        if (Random.Range(0, 1) == 0)
            randomPos.x = randomPos.x + 1.0f;
        else
            randomPos.x = randomPos.x - 1.0f;
           //    randomPos.x = randomPos.x + Random.Range(-1.0f, 1.0f);
           randomPos.y = randomPos.y + 1.0f;
        if (curCool >= maxCool)
        {
            Instantiate(dark, randomPos, Quaternion.identity);
            text.GetComponent<DarkCounter>().count++;
            curCool = 0;
        }
        else
        {
            curCool += 0.1f;
        }
    }
}
