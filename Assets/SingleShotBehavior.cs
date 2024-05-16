using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SingleShotBehavior : MonoBehaviour
{
    private Player player;
    [SerializeField] private Rigidbody2D rb;
    private EnemyBehavior enemy;

    void Start()
    {
        enemy = gameObject.GetComponent<EnemyBehavior>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBehavior>())
        {
            if (collision.gameObject.GetComponent<EnemyBehavior>().dead == true)
            {
                collision.GetComponent<BoxCollider2D>().enabled = false;
                collision.GetComponent<CircleCollider2D>().enabled = false;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) // Player bullet colliding with enemy
        {
            if (collision.gameObject.GetComponent<EnemyBehavior>())
            {
                if (collision.gameObject.GetComponent<EnemyBehavior>().dead == false)
                {
                    Destroy(gameObject);
                }
            }

            if (collision.gameObject.GetComponent<GunTurret>())
            {
                if (collision.gameObject.GetComponent<GunTurret>().dead == false)
                {
                    Destroy(gameObject);
                }
            }

            if (collision.gameObject.GetComponent<BossEyeBehavior>())
            {
                if (collision.gameObject.GetComponent<BossEyeBehavior>().dead == false)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (collision.gameObject.layer == 11) // Player bullet colliding with wall
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Debug.Log("player.current_weapon: " + player.current_weapon);
        rb.velocity = player.weapon[player.current_weapon].velocity;

        if (gameObject.transform.position.y >= 6f)
            Destroy(gameObject);
    }
}
