using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehavior : MonoBehaviour
{
    public enum FireState
    {
        Single,
        Cardinal,
        Diagnal,
        Both,
    }
    public FireState fireState;

    [Header("Player References")]
    public Player player;

    [Header("Health")]
    [SerializeField] private float health = 100f;

    [Header("Firing")]
    [SerializeField] private GameObject bullet;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private Vector3 dir;

    [Header("Helper Variables")]
    [SerializeField] private SpriteRenderer sR;

    [SerializeField] private Vector3 posTarget;

    private AudioSource warpsound;
    private AudioSource clanksound;
    private AudioSource explosion;
    private AudioSource laser;

    public bool dead;

    Animator animator;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        warpsound = GameObject.Find("Warp").GetComponent<AudioSource>();
        clanksound = GameObject.Find("Metal Clank").GetComponent<AudioSource>();
        explosion = GameObject.Find("Explosion").GetComponent<AudioSource>();
        laser = GameObject.Find("Enemy Laser").GetComponent<AudioSource>();

        animator = gameObject.GetComponent<Animator>();

        StartCoroutine(FireBullet());
    }

    void Update()
    {
        if (health <= 0)
        {
            dead = true;

            animator.SetBool("Death", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 4f)
                    Destroy(gameObject);
        }

        // If enemy is offscreen on the bottom, despawn it.
        if (transform.position.y <= (player.transform.position.y - 12f)) //-6f
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            if (dead == false)
                DamageHealth();
    }

    //Fire Bullet
    IEnumerator FireBullet()
    {
        if (!UnityEngine.Object.ReferenceEquals(this, null))
        {
            if (dead == false)
            {
                yield return new WaitForSeconds(0.5f);

                //Get current player's position during this frame.
                dir = player.transform.position - transform.position;
                //target position - current position = direction and distance towards target from current position.

                SingleFire();

                StartCoroutine(FireBullet());
            }
        }
    }

    //Fire Behavior
    private void SingleFire()
    {
        const float BULLET_SPEED = 5.0f;

        if (!UnityEngine.Object.ReferenceEquals(this, null))
        {
            if (dead == false)
            {
                //Shoot a bullet at position.
                var bull = Instantiate(bullet, transform.position, Quaternion.identity);
                bull.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * BULLET_SPEED;
                bull.SendMessage("startTimedDestroy");
                if (laser != null)
                    laser.Play();
            }
        }
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
        if (!UnityEngine.Object.ReferenceEquals(this, null))
        {
            if (dead == false)
            {
                health -= player.weapon[0].damage;

                if (health > 0)
                    clanksound.Play();
                else
                    explosion.Play();

                StartCoroutine(Flash());
            }
        }
    }
}
