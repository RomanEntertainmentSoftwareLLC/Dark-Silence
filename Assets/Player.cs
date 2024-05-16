using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class Weapon_Type
{
    public string name;
    public double firing_rate;
    public float damage;
    public Vector2 velocity;
    public int num_firepoints;
    public GameObject weapon;
    public GameObject[] firePoint;
    public double timer;
    public Rigidbody2D rb;
    public AudioSource sound;
}

public class Player : MonoBehaviour
{
    private const int MAX_WEAPONS = 2;

    [SerializeField] private int health = 100;

    private Camera camera;
    private Animator animator;
    private Animation animation;
    private Vector2 movement_vector;
    private float movement_speed;
    [SerializeField] private Rigidbody2D rb;
    private bool lock_x;
    private bool lock_y;
    private bool idle;
    private bool turn_left;
    private bool turn_right;
    private bool release_left;
    private bool release_right;
    private bool key_held = false;
    public Weapon_Type[] weapon = new Weapon_Type[MAX_WEAPONS];
    public int current_weapon;
    [SerializeField] private bool dead;
    private bool temporary_invincibility;
    private float timer;
    [SerializeField] private SpriteRenderer sR;

    [SerializeField] private AudioSource explosion_sound;
    [SerializeField] private AudioSource clank_sound;

    [Header("PointEffector")]
    [SerializeField] private GameObject magnet;
    [SerializeField] private TextMeshProUGUI scoreboard;
    private UnityEngine.UI.Image healthbar_image;
    private Vector2 healthbar;
    private float initial_healthbar_width;
    private Color32 healthbar_color;
    private UnityEngine.UI.Image[] life_image = new UnityEngine.UI.Image[5];
    private int lives;
    private Pause pause;
    public int score;
    private int old_score;
    

    void Awake()
    {
        score = 0;
        old_score = 0;
        current_weapon = 0;
        pause = GameObject.Find("PauseObject").GetComponent<Pause>();

        lives = 3;

        life_image[0] = GameObject.Find("Life 0").GetComponent<UnityEngine.UI.Image>();
        life_image[1] = GameObject.Find("Life 1").GetComponent<UnityEngine.UI.Image>();
        life_image[2] = GameObject.Find("Life 2").GetComponent<UnityEngine.UI.Image>();
        life_image[3] = GameObject.Find("Life 3").GetComponent<UnityEngine.UI.Image>();
        life_image[4] = GameObject.Find("Life 4").GetComponent<UnityEngine.UI.Image>();

        //for (int i = 0; i < 5; i++)
        //{
        //    life_image[i] = GameObject.Find("Life " + i.ToString()).GetComponent<UnityEngine.UI.Image>();
        //}

        healthbar_image = GameObject.Find("Health Bar").GetComponent<UnityEngine.UI.Image>();
        healthbar.x = healthbar_image.rectTransform.sizeDelta.x;
        healthbar.y = healthbar_image.rectTransform.sizeDelta.y;
        initial_healthbar_width = healthbar.x;
        healthbar_color.g = 255;
        healthbar_color.a = 255;
        healthbar_image.color = new Color32(healthbar_color.r, healthbar_color.g, healthbar_color.b, healthbar_color.a);
        animator = GameObject.Find("Player").GetComponent<Animator>();
        //animation = GameObject.Find("Player").GetComponent<Animation>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator.SetBool("Release Left", false);
        animator.SetBool("Release Right", false);
        animator.SetBool("Turn Right", false);
        animator.SetBool("Turn Left", false);
        animator.SetBool("Dead", false);
        animator.SetBool("Idle", true);
        idle = true;
        movement_speed = 5.0f;

        weapon[0] = new Weapon_Type();
        weapon[0].name = "Single Shot";
        weapon[0].firing_rate = 0.25f;
        weapon[0].damage = 10f;
        weapon[0].velocity = new Vector2(0f, 10f);
        weapon[0].num_firepoints = 1;
        weapon[0].firePoint = new GameObject[weapon[0].num_firepoints];
        weapon[0].firePoint[0] = GameObject.Find("Firepoint1");
        weapon[0].weapon = Resources.Load("Graphics\\Game\\Sprites\\Weapons\\Prefabs\\single shot") as GameObject;
        weapon[0].rb = weapon[0].weapon.GetComponent<Rigidbody2D>();
        weapon[0].sound = GameObject.Find("Single Shot Sound").GetComponent<AudioSource>();

        weapon[1] = new Weapon_Type();
        weapon[1].name = "Beam";
        weapon[1].firing_rate = 0.005f;
        weapon[1].damage = 1f;
        weapon[1].velocity = new Vector2(0f, 10f);
        weapon[1].num_firepoints = 1;
        weapon[1].firePoint = new GameObject[weapon[1].num_firepoints];
        weapon[1].firePoint[0] = GameObject.Find("Firepoint1");
        weapon[1].weapon = Resources.Load("Graphics\\Game\\Sprites\\Weapons\\Prefabs\\beam") as GameObject;
        weapon[1].rb = weapon[1].weapon.GetComponent<Rigidbody2D>();
        weapon[1].sound = GameObject.Find("Single Shot Sound").GetComponent<AudioSource>();

    }
    private void life_controller()
    {
        switch (lives)
        {
            case 0:
                // TODO: Continue Screen

                life_image[0].enabled = false;
                life_image[1].enabled = false;
                life_image[2].enabled = false;
                life_image[3].enabled = false;
                life_image[4].enabled = false;
                break;
            case 1:
                life_image[0].enabled = true;
                life_image[1].enabled = false;
                life_image[2].enabled = false;
                life_image[3].enabled = false;
                life_image[4].enabled = false;
                break;
            case 2:
                life_image[0].enabled = true;
                life_image[1].enabled = true;
                life_image[2].enabled = false;
                life_image[3].enabled = false;
                life_image[4].enabled = false;
                break;
            case 3:
                life_image[0].enabled = true;
                life_image[1].enabled = true;
                life_image[2].enabled = true;
                life_image[3].enabled = false;
                life_image[4].enabled = false;
                break;
            case 4:
                life_image[0].enabled = true;
                life_image[1].enabled = true;
                life_image[2].enabled = true;
                life_image[3].enabled = true;
                life_image[4].enabled = false;
                break;
            case 5:
                life_image[0].enabled = true;
                life_image[1].enabled = true;
                life_image[2].enabled = true;
                life_image[3].enabled = true;
                life_image[4].enabled = true;
                break;
            default:
                break;
        }
    }

    private void Shoot()
    {
        Instantiate(weapon[current_weapon].weapon, weapon[current_weapon].firePoint[0].transform.position, Quaternion.identity);
        weapon[current_weapon].sound.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead == false)
        {
            if (temporary_invincibility == false)
            {
                if (collision.gameObject.layer == 3) // Player to enemy collision
                {
                    animator.SetBool("Release Left", false);
                    animator.SetBool("Release Right", false);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Turn Right", false);
                    animator.SetBool("Turn Left", false);
                    animator.SetBool("Dead", true);
                    release_left = false;
                    release_right = false;
                    idle = false;
                    turn_right = false;
                    turn_left = false;
                    dead = true;
                    lives -= 1;

                    if (lives <= 0)
                        lives = 0;
                    
                    explosion_sound.Play();
                    gameObject.layer = 9; // Switch to explosion layer so enemies go through it rather than collide.
                    key_held = false;
                }

                if (collision.gameObject.layer == 11) // Player to wall collision
                {
                    animator.SetBool("Release Left", false);
                    animator.SetBool("Release Right", false);
                    animator.SetBool("Idle", false);
                    animator.SetBool("Turn Right", false);
                    animator.SetBool("Turn Left", false);
                    animator.SetBool("Dead", true);
                    release_left = false;
                    release_right = false;
                    idle = false;
                    turn_right = false;
                    turn_left = false;
                    dead = true;
                    lives -= 1;

                    if (lives <= 0)
                        lives = 0;

                    explosion_sound.Play();
                    gameObject.layer = 9; // Switch to explosion layer so enemies go through it rather than collide.
                    key_held = false;
                }

                if (collision.gameObject.layer == 8) // Player to enemy bullet collision
                {
                    health -= 10;
                    healthbar.x -= 15;
                    healthbar_image.rectTransform.sizeDelta = new Vector2(healthbar.x, healthbar.y);
                    healthbar_color.r += 25;
                    healthbar_color.g -= 25;
                    healthbar_image.color = new Color32(healthbar_color.r, healthbar_color.g, healthbar_color.b, healthbar_color.a);

                    if (health > 0)
                        clank_sound.Play();
                    else
                    {
                        animator.SetBool("Release Left", false);
                        animator.SetBool("Release Right", false);
                        animator.SetBool("Idle", false);
                        animator.SetBool("Turn Right", false);
                        animator.SetBool("Turn Left", false);
                        animator.SetBool("Dead", true);
                        release_left = false;
                        release_right = false;
                        idle = false;
                        turn_right = false;
                        turn_left = false;
                        dead = true;
                        lives -= 1;

                        if (lives <= 0)
                            lives = 0;

                        explosion_sound.Play();
                        gameObject.layer = 9; // Switch to explosion layer so enemies go through it rather than collide.
                        key_held = false;
                    }


                    StartCoroutine(Flash());
                }
            }
        }
    }

    void Update()
    {
        old_score = score;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync("Game");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (pause.pause == false)
        {
            //animation.Play();
            life_controller();

            if (dead == false)
            {
                if (temporary_invincibility == true)
                {
                    timer += Time.deltaTime;
                    if (gameObject.GetComponent<SpriteRenderer>().enabled == true)
                        gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    else
                        gameObject.GetComponent<SpriteRenderer>().enabled = true;

                    if (timer >= 5.0f)
                    {
                        timer = 5.0f;
                        gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        temporary_invincibility = false;
                        gameObject.layer = 6; // Set layer back to player
                    }
                }

                //if (Input.GetKeyDown(KeyCode.E))
                //{
                //    //Pushing bullets away from player.
                //    StartCoroutine(ForceMagnitudeOut());
                //}

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    //Increasing speed to allow the player to "dodge"
                    StartCoroutine(IncreaseMoveSpeed());
                }

                if (idle == false)
                {
                    if (Input.GetAxisRaw("Horizontal") == 0.0f)
                    {
                        if (turn_left)
                        {
                            animator.SetBool("Release Left", true);
                            animator.SetBool("Release Right", false);
                            animator.SetBool("Idle", false);
                            animator.SetBool("Turn Right", false);
                            animator.SetBool("Turn Left", false);
                            animator.SetBool("Dead", false);
                            release_left = true;
                            release_right = false;
                            idle = false;
                            turn_right = false;
                            turn_left = false;
                        }
                        else if (turn_right)
                        {
                            animator.SetBool("Release Left", false);
                            animator.SetBool("Release Right", true);
                            animator.SetBool("Idle", false);
                            animator.SetBool("Turn Right", false);
                            animator.SetBool("Turn Left", false);
                            animator.SetBool("Dead", false);
                            release_left = false;
                            release_right = true;
                            idle = false;
                            turn_right = false;
                            turn_left = false;
                        }
                    }

                    if (release_left)
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("release left") && (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) && !animator.IsInTransition(0))
                        {
                            animator.SetBool("Release Left", false);
                            animator.SetBool("Release Right", false);
                            animator.SetBool("Idle", true);
                            animator.SetBool("Turn Right", false);
                            animator.SetBool("Turn Left", false);
                            animator.SetBool("Dead", false);
                            release_left = false;
                            release_right = false;
                            idle = true;
                            turn_right = false;
                            turn_left = false;
                        }
                    }
                    else if (release_right)
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("release right") && (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) && !animator.IsInTransition(0))
                        {
                            animator.SetBool("Release Left", false);
                            animator.SetBool("Release Right", false);
                            animator.SetBool("Idle", true);
                            animator.SetBool("Turn Right", false);
                            animator.SetBool("Turn Left", false);
                            animator.SetBool("Dead", false);
                            release_left = false;
                            release_right = false;
                            idle = true;
                            turn_right = false;
                            turn_left = false;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    key_held = true;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    key_held = false;
                }

                if (key_held == true)
                {
                    weapon[current_weapon].timer += Time.deltaTime;

                    if (weapon[current_weapon].timer >= weapon[current_weapon].firing_rate)
                    {
                        weapon[current_weapon].timer = 0f;
                        Shoot();

                    }
                }
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2f)
                    {
                        animator.SetBool("Release Left", false);
                        animator.SetBool("Release Right", false);
                        animator.SetBool("Turn Right", false);
                        animator.SetBool("Turn Left", false);
                        animator.SetBool("Dead", false);
                        animator.SetBool("Idle", true);
                        release_left = false;
                        release_right = false;
                        turn_right = false;
                        turn_left = false;
                        dead = false;
                        idle = true;
                        timer = 0.0f;
                        gameObject.layer = 10; // Set layer of player to invincibility;
                        rb.transform.position = new Vector2(0.0f, 0.0f);
                        temporary_invincibility = true;
                        health = 100;
                        healthbar_image.rectTransform.sizeDelta = new Vector2(initial_healthbar_width, healthbar.y);
                        healthbar.x = healthbar_image.rectTransform.sizeDelta.x;
                        healthbar_color.r = 0;
                        healthbar_color.g = 255;
                        healthbar_image.color = new Color32(healthbar_color.r, healthbar_color.g, healthbar_color.b, healthbar_color.a);
                    }
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            //animation.Stop();
        }

        scoreboard.text = score.ToString();
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        movement_vector.x = Input.GetAxisRaw("Horizontal");
        movement_vector.y = Input.GetAxisRaw("Vertical");
        movement_vector.Normalize();

        rb.velocity = movement_vector * movement_speed;

        if (transform.position.x <= -13f)
        {
            pos.x = -13f;
        }

        if (transform.position.x >= 13f)
        {
            pos.x = 13f;
        }

        if (transform.position.y <= -4.42f)
        {
            pos.y = -4.42f;
        }

        if (transform.position.y <= -4.42f)
        {
            pos.y = -4.42f;
        }

        if (transform.position.y >= 4.42f)
        {
            pos.y = 4.42f;
        }

        transform.position = pos;


        if (movement_vector.x < 0.0f)
        {
            animator.SetBool("Release Left", false);
            animator.SetBool("Release Right", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Turn Right", false);
            animator.SetBool("Turn Left", true);
            release_left = false;
            release_right = false;
            idle = false;
            turn_right = false;
            turn_left = true;
        }
        else if (movement_vector.x > 0.0f)
        {
            animator.SetBool("Release Left", false);
            animator.SetBool("Release Right", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Turn Right", true);
            animator.SetBool("Turn Left", false);
            release_left = false;
            release_right = false;
            idle = false;
            turn_right = true;
            turn_left = false;
        }
    }

    IEnumerator IncreaseMoveSpeed()
    {
        var x = movement_speed;
        movement_speed = movement_speed * 3;

        yield return new WaitForSeconds(.2f);

        movement_speed = x;
    }

    IEnumerator ForceMagnitudeOut()
    {
        magnet.GetComponent<PointEffector2D>().forceMagnitude = 1000;

        yield return new WaitForSeconds(.2f);

        magnet.GetComponent<PointEffector2D>().forceMagnitude = 0;
    }

    //Damage Feedback
    IEnumerator Flash()
    {
        sR.color = Color.red;

        yield return new WaitForSeconds(0.02f);

        sR.color = Color.white;
    }
}


