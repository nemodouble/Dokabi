using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class DarkController : MonoBehaviour
{
    private Rigidbody2D rigid2D;

    // Start is called before the first frame update
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("PlatForm"))
        {
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            var playerPos = GameObject.Find("Player").GetComponent<PlayerController>().transform.position;
            collision.collider.GetComponent<PlayerController>()
                .StartAttacked(1, new Vector2((playerPos.x - transform.position.x > 0) ? 1 : -1, 0));
            gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        var playerPos = GameObject.Find("Player").GetComponent<PlayerController>().transform.position;
        var toPlayer = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);
        
        rigid2D.AddForce(3.0f *toPlayer.normalized);
    }
}
