using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float durationNow;

    private Collider2D collider2D;

    private void Start()
    {
        collider2D = gameObject.GetComponent<Collider2D>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (durationNow < duration)
            durationNow += Time.deltaTime;
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        durationNow = 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            col.GetComponent<PlayerController>().StartAttacked(damage, Vector2.zero);
            //gameObject.SetActive(false);
        }
    }
}
