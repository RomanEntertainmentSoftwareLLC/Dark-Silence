using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemyBehavior;

public class BossEyeBehavior : MonoBehaviour
{
    private Player player;
    private float timer;

    [Header ("References")]
    [SerializeField] private GameObject otherEye;
    [SerializeField] private GameObject droneZone;
    [SerializeField] private GameObject boss;
    SpriteRenderer sr;

    [Header ("Firing")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform[] firePoint;

    [Header("Helper Variables")]
    public int health;
    [SerializeField] private Transform level;

    private Vector3 targetDir;
    private float angle;

    private Vector3 dir;

    private UnityEngine.UI.Image healthbar_image;
    private Vector2 healthbar;
    private int initial_health;
    private float initial_healthbar_width;
    private Color32 healthbar_color;
    private bool half_dead = false;

    [SerializeField] private AudioSource explosion_sound;
    [SerializeField] private AudioSource blob_sound;
    [SerializeField] private AudioSource scream_sound;
    [SerializeField] private AudioSource laser_sound;
    [SerializeField] private int points;
    private Animator animator;

    public bool dead;

    private void Awake()
    {
        health = 1000;
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        sr = boss.GetComponent<SpriteRenderer>();
        healthbar_image = GameObject.Find("Boss Health Bar").GetComponent<UnityEngine.UI.Image>();
        healthbar.x = healthbar_image.rectTransform.sizeDelta.x;
        healthbar.y = healthbar_image.rectTransform.sizeDelta.y;
        initial_health = health;
        initial_healthbar_width = healthbar.x;
        healthbar_color.g = 255;
        healthbar_color.a = 255;
        healthbar_image.color = new Color32(healthbar_color.r, healthbar_color.g, healthbar_color.b, healthbar_color.a);
    }

    private void Start()
    {
        StartCoroutine(FireBullet());
    }

    void Update()
    {
        //Eye tracking
        targetDir = transform.position - player.transform.position;
        angle = (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg) - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (half_dead == false)
        {
            if (health <= initial_health * 0.5f)
            {
                half_dead = true;

                if (scream_sound != null)
                    scream_sound.Play();
                
                sr.sprite = Resources.Load<Sprite>("Graphics\\Game\\Sprites\\Bosses\\Level 1\\Li ii Skull");
            }
        }

        //Destroy when killed
        if (health <= 0)
        {
            Destroy(droneZone);
            health = 0;
            if (animator.GetBool("Death") == false)
            {
                player.score += points;
                animator.SetBool("Death", true);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 4f)
                {
                    Destroy(gameObject);

                    timer += Time.deltaTime;

                    if (timer >= 5f)
                    {
                        SceneManager.UnloadSceneAsync("Game");
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
                    
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        otherEye.SendMessage("DamageHealth");
        DamageHealth();
    }

    private void DamageHealth()
    {
        health -= (int)player.weapon[0].damage;
        healthbar.x = ((float)health / initial_health) * initial_healthbar_width;
        healthbar_image.rectTransform.sizeDelta = new Vector2(healthbar.x, healthbar.y);
        float color_ratio = ((float)health / initial_health) * 255f;
        healthbar_color.r = (byte)(255f - color_ratio);
        healthbar_color.g = (byte)color_ratio;
        healthbar_image.color = new Color32(healthbar_color.r, healthbar_color.g, healthbar_color.b, healthbar_color.a);

        //Debug.Log(health + ", " + initial_health + ", " + initial_healthbar_width + ", " + healthbar.x);


        if (health > 0)
        {
            if (blob_sound != null)
                blob_sound.Play();
        }
        else
        {
            explosion_sound.Play();
            scream_sound.Play();
            dead = true;
        }
    }

    IEnumerator FireBullet()
    {
        yield return new WaitForSeconds(0.5f);

        if (level.position.y <= -629.25f)
        {
            //Get current player's position during this frame.
            dir = player.transform.position - transform.position;
            //target position - current position = direction and distance towards target from current position.

            const float BULLET_SPEED = 5.0f;

            //Shoot a bullet at position.
            var bull = Instantiate(bullet, transform.position, Quaternion.identity);
            bull.GetComponent<BulletBehavior>().rb.velocity = dir.normalized * BULLET_SPEED;
            bull.SendMessage("startTimedDestroy");

            if (laser_sound != null)
                laser_sound.Play();
        }

        StartCoroutine(FireBullet());
    }
}