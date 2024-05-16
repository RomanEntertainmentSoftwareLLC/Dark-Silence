using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GunTurret : MonoBehaviour
{
    public enum State
    {
        HoldStill,
        AimRotator,
    }
    public State state;

    public enum FireState
    {
        None,
        ShotGun,
        AlternatingFire,
    }
    public FireState fireState;

    [Header("Player References")]
    public Player player;

    [Header("Health")]
    [SerializeField] private float health = 100f;

    [Header("Firing")]
    [SerializeField] private float firerate = 0.5f;
    [SerializeField] private float bullet_velocity = 5f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform[] firePoint;

    [Header("Movement")]
    [SerializeField] private Vector3 rot;
    private Vector3 dir;

    [Header("Helper Variables")]
    [SerializeField] private SpriteRenderer sR;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Vector3 posTarget;

    [SerializeField] private AudioSource clanksound;
    [SerializeField] private AudioSource explosion;
    [SerializeField] private AudioSource laser;
    [SerializeField] private int points;

    public bool dead;
    Animator animator;

    private bool alternate;
    private bool visible;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        StartCoroutine(FireBullet());

        switch (state)
        {
            case State.HoldStill:
                break;
            case State.AimRotator:
                break;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.HoldStill:
                break;
            case State.AimRotator:
                AiAimRotator();
                break;
        }

        if (health <= 0)
        {
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

        // If enemy is offscreen on the bottom, despawn it.
        if (transform.position.y <= -6f)
            Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
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
                case FireState.ShotGun:
                    ShotGun();
                    break;
                case FireState.AlternatingFire:
                    AlternatingFire();
                    break;
            }

            StartCoroutine(FireBullet());
        }
    }

    //Fire Behavior
    private void ShotGun()
    {
        if (dead == false)
        {
            if (visible == true)
            {
                //Shoot a bullet at position.
                var bull1 = Instantiate(bullet, firePoint[0].position, Quaternion.identity);
                bull1.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * bullet_velocity;
                bull1.SendMessage("startTimedDestroy");
                var bull2 = Instantiate(bullet, firePoint[1].position, Quaternion.identity);
                bull2.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * bullet_velocity;
                bull2.SendMessage("startTimedDestroy");
                if (laser != null)
                    laser.Play();
            }
        }
    }

    private void AlternatingFire()
    {
        if (dead == false)
        {
            if (visible == true)
            {
                if (alternate == false)
                {
                    var bull = Instantiate(bullet, firePoint[0].position, Quaternion.identity);
                    bull.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * bullet_velocity;
                    alternate = true;
                }
                else
                {
                    var bull = Instantiate(bullet, firePoint[1].position, Quaternion.identity);
                    bull.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * bullet_velocity;
                    alternate = false;
                }

                if (laser != null)
                    laser.Play();
            }

        }
    }

    private void AiAimRotator()
    {
        Vector2 directionToTarget = player.transform.position - transform.position;

        //This assumes that your bullet sprite points to the right
        //Get the angle above the horizontal where the target is
        float angle = Vector3.Angle(Vector3.right, directionToTarget);
        //This will always be positive, so lets flip the sign if it should be negative
        if (player.transform.position.y < transform.position.y) angle *= -1;

        transform.eulerAngles = new Vector3(0f, 0f, angle);
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
            health -= player.weapon[0].damage;

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
    }
}
