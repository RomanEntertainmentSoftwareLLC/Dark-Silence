using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GigerSnakeLeft : MonoBehaviour
{
    private float timer;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 initial_position;
    private bool fire;
    private bool hold;
    private bool retract;
    [SerializeField] private AudioSource clank_sound;

    // Start is called before the first frame update
    void Start()
    {
        fire = false;
        hold = false;
        retract = false;
        initial_position = new Vector2(rb.transform.position.x, rb.transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (clank_sound != null)
            clank_sound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        const float FIRE_TIME = 2.0f;
        const float HOLD_TIME = 3.0f;
        const float FIRE_SPEED = 5.0f;
        const float RETRACT_SPEED = -1.0f;

        //Debug.Log("fire: " + fire + ", hold: " + hold + ", retract: " + retract);

        if (fire == false && hold == false && retract == false)
        {
            timer += Time.deltaTime;

            if (timer >= FIRE_TIME)
            {
                Vector2 fire_velocity = new Vector2(FIRE_SPEED, 0f);
                rb.velocity = fire_velocity;
                fire = true;
            }
        }
        else if (fire == true && hold == false && retract == false)
        {
            if (rb.transform.position.x - initial_position.x >= 9f)
            {
                fire = false;
                hold = true;
                Vector2 hold_velocity = new Vector2(0f, 0f);
                rb.velocity = hold_velocity;
            }
        }
        else if (fire == false && hold == true && retract == false)
        {
            timer += Time.deltaTime;
            if (timer >= HOLD_TIME)
            {
                hold = false;
                retract = true;
                timer = 0.0f;
                Vector2 retract_velocity = new Vector2(RETRACT_SPEED, 0f);
                rb.velocity = retract_velocity;
            }
        }
        else if (fire == false && hold == false && retract == true)
        {
            if (rb.transform.position.x <= initial_position.x)
            {
                Vector2 velocity = new Vector2(0f, 0f);
                rb.velocity = velocity;
                retract = false;
                timer = 0.0f;
            }
        }

        // If enemy is offscreen on the bottom, despawn it.
        if (transform.position.y <= -6f)
            Destroy(gameObject);
    }
}
