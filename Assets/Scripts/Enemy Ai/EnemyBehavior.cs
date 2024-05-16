using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class EnemyBehavior : MonoBehaviour
{
    public enum State
    {
        HoldStill,
        Random,
        Teleport,
        SideToSide,
        StraightDown,
        SinePattern,
        CosinePattern,
        SinePatternRotatable,
        CosinePatternRotatable,
        NZag,
        OppositeNZag,
    }
    public State state;

    public enum FireState
    {
        None,
        Single,
        Straight,
        Spread,
        Cardinal,
        Diagnal,
        Both,
        Cyclone,
    }
    public FireState fireState;

    [Header("Player References")]
    public Player player;

    [Header("Health")]
    [SerializeField] private bool invincible = false;
    [SerializeField] private float health = 100f;

    [Header("Firing")]
    [SerializeField] private float firerate = 0.5f;
    [SerializeField] private float bullet_velocity = 5f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform[] firePoint;
    
    [Header("Movement")]
    [SerializeField] private Vector3 rot;
    private Vector3 dir;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool rotate = true;
    [SerializeField] private float amplitude = 2f; // For sine, cosine, and side to side movements;
    [SerializeField] private float frequency = 0.5f; // For sine, cosine, and side to side movements;

    [Header("Helper Variables")]
    [SerializeField] private SpriteRenderer sR;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Vector3 posTarget;

    [SerializeField] private AudioSource warpsound;
    [SerializeField] private AudioSource clanksound;
    [SerializeField] private AudioSource explosion;
    [SerializeField] private AudioSource laser;

    [SerializeField] private int points;

    private float side_to_side_angle;

    private float cyclone_angle;
    private int move_step;
    public bool dead;
    Animator animator;
    private float sine_center_x;
    private Vector2 old_pos;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        //player_obj = player.GetComponent<Player>();
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sine_center_x = transform.position.x;

        StartCoroutine(FireBullet());

        switch (state)
        {
            case State.HoldStill:
                break;
            case State.Random:
                StartCoroutine(AiRandomSpawnStraightMovement());
                break;
            case State.Teleport:
                Vector2 pos = new Vector3(UnityEngine.Random.Range(8.3f, -8.3f), UnityEngine.Random.Range(4.45f, -4.45f), 0);
                transform.position = pos;
                if (warpsound != null)
                    warpsound.Play();
                StartCoroutine(AiTeleportMovement());
                break;
            case State.SideToSide:
                break;
            case State.SinePattern:
                break;
            case State.CosinePattern:
                break;
            case State.SinePatternRotatable:
                break;
            case State.CosinePatternRotatable:
                break;
            case State.StraightDown:
                AiStraightDownMovement();
                break;
            case State.NZag:
                break;
            case State.OppositeNZag:
                break;
        }

        old_pos = rb.transform.position;
    }

    void Update()
    {
        //Debug.Log(cyclone_angle);

        switch (state)
        {
            case State.HoldStill:
                break;
            case State.Random:
                break;
            case State.Teleport:
                break;
            case State.SideToSide:
                AiSideToSideMovement();
                break;
            case State.SinePattern:
                AiSinePatternMovement();
                break;
            case State.CosinePattern:
                AiCosinePatternMovement();
                break;
            case State.SinePatternRotatable:
                AiSinePatternRotatableMovement();
                break;
            case State.CosinePatternRotatable:
                AiCosinePatternRotatableMovement();
                break;
            case State.StraightDown:
                break;
            case State.NZag:
                AiNZagMovement();
                break;
            case State.OppositeNZag:
                AiOppositeNZagMovement();
                break;
        }

        if (rotate)
        {
            transform.Rotate(rot);
        }

        if (health <= 0)
        {
            if (warpsound != null)
            {
                if (warpsound.isPlaying)
                    warpsound.Stop();
            }

            if (laser != null)
            {
                if (laser.isPlaying)
                    laser.Stop();
            }

            if (animator.GetBool("Death") == false)
            {
                player.score += points;
                animator.SetBool("Death", true);
            }
            
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 4f)
                    Destroy(gameObject);
        }


        // If enemy is offscreen on the sides, despawn it.
        if (transform.position.x <= -14.18f || transform.position.x >= 14.18f)
            Destroy(gameObject);

        // If enemy is offscreen on the bottom, despawn it.
        if (transform.position.y <= -6f)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead == false)
            DamageHealth();
    }

    //Fire Bullet
    IEnumerator FireBullet()
    {
        if (dead == false)
        {
            yield return new WaitForSeconds(firerate);

            //Get current player's position during this frame.
            dir = player.transform.position - transform.position;
            //target position - current position = direction and distance towards target from current position.

            switch (fireState)
            {
                case FireState.None:
                    // The enemy doesn't fire at all.
                    break;
                case FireState.Single:
                    SingleFire();
                    break;
                case FireState.Straight:
                    StraightFire();
                    break;
                case FireState.Spread:
                    SpreadFire();
                    break;
                case FireState.Cardinal:
                    CardinalFire();
                    break;
                case FireState.Diagnal:
                    DiagnalFire();
                    break;
                case FireState.Both:
                    BothFire();
                    break;
                case FireState.Cyclone:
                    Cyclone();
                    break;
            }

            StartCoroutine(FireBullet());
        }
    }

    //Fire Behavior
    private void SingleFire()
    {
        if (dead == false)
        {
            //Shoot a bullet at position.
            var bull = Instantiate(bullet, firePoint[0].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * bullet_velocity;
            bull.SendMessage("startTimedDestroy");
            if (laser != null)
                laser.Play();
        }
    }

    private void StraightFire()
    {
        if (dead == false)
        {
            var bull = Instantiate(bullet, firePoint[0].position, Quaternion.identity);
            Vector2 velocity = new Vector2(0.0f, -bullet_velocity);
            bull.GetComponent<BulletBehavior>().rb.velocity = velocity;
            bull.SendMessage("startTimedDestroy");
            if (laser != null)
                laser.Play();
        }
    }

    private void SpreadFire()
    {
        if (dead == false)
        {
            //Shoots Down
            var bull = Instantiate(bullet, firePoint[2].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.up) * bullet_velocity;
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-right
            bull = Instantiate(bullet, firePoint[7].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = (transform.right * bullet_velocity) + -(transform.up * bullet_velocity);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-left
            bull = Instantiate(bullet, firePoint[8].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right * bullet_velocity) + -(transform.up * bullet_velocity);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();
        }
    }

    private void CardinalFire()
    {
        if (dead == false)
        {
            //Shoots Up 1
            var bull = Instantiate(bullet, firePoint[1].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = transform.up * bullet_velocity;//new Vector3( 0, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down 2
            bull = Instantiate(bullet, firePoint[2].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.up) * bullet_velocity;//new Vector3(0, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Left 3
            bull = Instantiate(bullet, firePoint[3].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right) * bullet_velocity;//new Vector3(-5, 0, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Right 4
            bull = Instantiate(bullet, firePoint[4].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = transform.right * bullet_velocity;//new Vector3(5, 0, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();
        }
    }

    private void DiagnalFire()
    {
        const float bullet_velocity = 3.0f;

        if (dead == false)
        {
            //Shoots Up-right 5
            var bull = Instantiate(bullet, firePoint[5].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = (transform.right * bullet_velocity) + (transform.up * bullet_velocity);//new Vector3(5, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Up-left 6
            bull = Instantiate(bullet, firePoint[6].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right * bullet_velocity) + (transform.up * bullet_velocity);//new Vector3(-5, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-right 7
            bull = Instantiate(bullet, firePoint[7].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = (transform.right * bullet_velocity) + -(transform.up * bullet_velocity);//new Vector3(5, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-left 8
            bull = Instantiate(bullet, firePoint[8].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right * bullet_velocity) + -(transform.up * bullet_velocity);//new Vector3(-5, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();
        }
    }

    private void BothFire()
    {
        const float bullet_velocity = 10.0f;

        if (dead == false)
        {
            //Shoots Up 1
            var bull = Instantiate(bullet, firePoint[1].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = transform.up * bullet_velocity;//new Vector3( 0, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down bullet_velocity
            bull = Instantiate(bullet, firePoint[2].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.up) * bullet_velocity;//new Vector3(0, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Left 3
            bull = Instantiate(bullet, firePoint[3].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right) * bullet_velocity;//new Vector3(-5, 0, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Right 4
            bull = Instantiate(bullet, firePoint[4].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = transform.right * bullet_velocity;//new Vector3(5, 0, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Diagonal

            //Shoots Up-right 5
            bull = Instantiate(bullet, firePoint[5].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = (transform.right * bullet_velocity) + (transform.up * bullet_velocity);//new Vector3(5, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Up-left 6
            bull = Instantiate(bullet, firePoint[6].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right * bullet_velocity) + (transform.up * bullet_velocity);//new Vector3(-5, 5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-right 7
            bull = Instantiate(bullet, firePoint[7].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = (transform.right * bullet_velocity) + -(transform.up * bullet_velocity);//new Vector3(5, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();

            //Shoots Down-left 8
            bull = Instantiate(bullet, firePoint[8].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = -(transform.right * bullet_velocity) + -(transform.up * bullet_velocity);//new Vector3(-5, -5, 0);
            bull.SendMessage("startTimedDestroy");//Shoot a bullet at said position.
            if (laser != null)
                laser.Play();
        }
    }

    private void Cyclone()
    {
        // Fires a whole slew of bullets in a berserk like circle!

        Vector2 initial_direction;
        Vector2 new_direction;
        float s;
        float c;

        if (dead == false)
        {
            initial_direction = new Vector2(1f, 0f);

            s = (float)Math.Sin(Math.PI * cyclone_angle / 180.0f);
            c = (float)Math.Cos(Math.PI * cyclone_angle / 180.0f);

            cyclone_angle += 25f;

            if (cyclone_angle >= 360.0f)
                cyclone_angle = 0.0f;

            new_direction = new Vector2(initial_direction.x * c - initial_direction.y * s, initial_direction.x * s + initial_direction.y * c);

            //Shoot a bullet at position.
            var bull = Instantiate(bullet, firePoint[0].position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = new_direction * bullet_velocity;
            bull.SendMessage("startTimedDestroy");
            if (laser != null)
                laser.Play();
        }
    }

    //Flight Patterns
    IEnumerator AiRandomSpawnStraightMovement()
    {
        if (dead == false)
        {
            yield return null;

            posTarget = new Vector3(UnityEngine.Random.Range(8.3f, -8.3f), transform.position.y, 0f);
            rb.velocity = new Vector2(0f, -speed);
            transform.position = posTarget;
        }
    }

    IEnumerator AiTeleportMovement()
    {
        if (dead == false)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5)); //Random movement would work better on a timer.

            posTarget = new Vector3(UnityEngine.Random.Range(8.3f, -8.3f), UnityEngine.Random.Range(4.45f, -4.45f), 0);
            if (warpsound != null)
                warpsound.Play();
            transform.position = posTarget;
            StartCoroutine(AiTeleportMovement());
        }
    }

    private void AiSideToSideMovement()
    {
        // Enemy only moves side to side going along with the map level to appear in place.

        const float SPEED = 0.25f;
        const float map_size = 26f;
        const float half_map_size = map_size / 2f;
        Vector2 pos;
        float x;

        if (dead == false)
        {
            side_to_side_angle += 0.25f;
            x = (float)(half_map_size * Math.Sin(Math.PI * side_to_side_angle / 180.0f));
            if (side_to_side_angle >= 360.0f) side_to_side_angle = 0.0f;
            
            pos = rb.transform.position;
            pos.x = x;
            rb.transform.position = pos;
            rb.velocity = new Vector2(0f, -SPEED);
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiSinePatternMovement()
    {
        // Heavy sine pattern for enemies which are ideal for flight groups.
        Vector2 pos;
        float sine;

        if (dead == false)
        {
            rb.velocity = new Vector2(0f, -speed);
            pos = rb.transform.position;
            sine = (float)Math.Sin(pos.y * frequency) * amplitude;
            pos.x = sine_center_x + sine;
            rb.transform.position = pos;
            old_pos = pos;
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiCosinePatternMovement()
    {
        // Heavy cosine pattern for enemies which are ideal for flight groups.
        const float NINETY_DEGREES_IN_RADIANS = 1.5708f;
        Vector2 pos;
        float cosine;

        if (dead == false)
        {
            rb.velocity = new Vector2(0f, -speed);
            pos = rb.transform.position;
            cosine = (float)Math.Cos(pos.y * frequency + NINETY_DEGREES_IN_RADIANS) * amplitude;
            pos.x = sine_center_x + cosine;
            rb.transform.position = pos;
            old_pos = pos;
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiSinePatternRotatableMovement()
    {
        // Heavy sine pattern for enemies which are ideal for flight groups.
        const float NINETY_DEGREES_IN_RADIANS = 1.5708f;
        Vector2 pos;
        float sine;
        float angle;

        if (dead == false)
        {
            rb.velocity = new Vector2(0f, -speed);
            pos = rb.transform.position;
            sine = (float)Math.Sin(pos.y * frequency) * amplitude;
            pos.x = sine_center_x + sine;
            angle = (float)Math.Sin(pos.y * frequency - NINETY_DEGREES_IN_RADIANS) * 45f;
            rb.transform.position = pos;
            rb.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
            old_pos = pos;
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiCosinePatternRotatableMovement()
    {
        // Heavy cosine pattern for enemies which are ideal for flight groups.
        const float NINETY_DEGREES_IN_RADIANS = 1.5708f;
        Vector2 pos;
        float cosine;
        float angle;

        if (dead == false)
        {
            rb.velocity = new Vector2(0f, -speed);
            pos = rb.transform.position;
            cosine = (float)Math.Cos(pos.y * frequency + NINETY_DEGREES_IN_RADIANS) * amplitude;
            pos.x = sine_center_x + cosine;
            angle = (float)Math.Cos(pos.y * frequency) * 45f;
            rb.transform.position = pos;
            rb.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
            old_pos = pos;
        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiStraightDownMovement()
    {
        if (dead == false)
            rb.velocity = new Vector2(0f, -speed);
        else
            rb.velocity = Vector2.zero;
    }

    private void AiNZagMovement()
    {
        // Enemies in flight groups will be moving in a N-Shape pattern.

        if (dead == false)
        {
            if (move_step == 0 && rb.transform.position.y > -3f)
            {
                rb.velocity = new Vector2(0f, -speed);
            }
            else if (move_step == 0 && rb.transform.position.y <= -3f)
            {
                rb.velocity = new Vector2(speed, speed);
                move_step++;
            }

            if (move_step == 1 && rb.transform.position.y < 3f)
            {
                rb.velocity = new Vector2(speed, speed);
            }
            else if (move_step == 1 && rb.transform.position.y >= 3f)
            {
                rb.velocity = new Vector2(0f, -speed);
                move_step++;
            }

        }
        else
            rb.velocity = Vector2.zero;
    }

    private void AiOppositeNZagMovement()
    {
        // Enemies in flight groups will be moving in an opposite N-Shape pattern.

        if (dead == false)
        {
            if (move_step == 0 && rb.transform.position.y > -3f)
            {
                rb.velocity = new Vector2(0f, -speed);
            }
            else if (move_step == 0 && rb.transform.position.y <= -3f)
            {
                rb.velocity = new Vector2(-speed, speed);
                move_step++;
            }

            if (move_step == 1 && rb.transform.position.y < 3f)
            {
                rb.velocity = new Vector2(-speed, speed);
            }
            else if (move_step == 1 && rb.transform.position.y >= 3f)
            {
                rb.velocity = new Vector2(0f, -speed);
                move_step++;
            }

        }
        else
            rb.velocity = Vector2.zero;
    }

    //Damage Feedback
    IEnumerator Flash()
    {
        sR.color = Color.red;

        yield return new WaitForSeconds(0.02f);

        sR.color = Color.white;
    }

    public void DamageHealth()
    {
        if (dead == false)
        {
            if (invincible == false)
            {
                health -= player.weapon[player.current_weapon].damage;

                if (health > 0)
                {
                    if (clanksound != null)
                        clanksound.Play();
                }
                else
                {
                    if (explosion != null)
                        explosion.Play();
                    dead = true;
                    gameObject.layer = 9; // Switch to explosion layer so bullets go through it rather than collide.
                }


                StartCoroutine(Flash());
            }
            else
            {
                if (clanksound != null)
                    clanksound.Play();
            }
        }
    }
}
