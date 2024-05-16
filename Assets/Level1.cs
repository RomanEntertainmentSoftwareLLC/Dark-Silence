using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Level1 : MonoBehaviour
{
    private float timer;
    private float map_speed;
    //private Camera camera;
    [SerializeField] GameObject Map;
    private AudioSource level_song;
    private AudioSource boss_song;
    private bool boss_time;
    private int enemy_type;
    private Canvas boss_ui;
    private Canvas pause_ui;
    private Canvas continue_ui;
    private Canvas game_over_ui;


    public const float EnemySpawnPosition = -588.47f;
    public const float BossEncounterTriggerPosition = -607.7f;
    public const float LevelMaxPosition = -629.25f;

    private void Awake()
    {
        boss_ui = GameObject.Find("Boss UI").GetComponent<Canvas>();
        pause_ui = GameObject.Find("Pause UI").GetComponent<Canvas>();
        continue_ui = GameObject.Find("Continue UI").GetComponent<Canvas>();
        game_over_ui = GameObject.Find("GameOver UI").GetComponent<Canvas>();

        boss_ui.enabled = false;
        pause_ui.enabled = false;
        continue_ui.enabled = false;
        game_over_ui.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        map_speed = 1.0f;

        level_song = GameObject.Find("Level 1 Song").GetComponent<AudioSource>();
        boss_song = GameObject.Find("Level 1 Boss Song").GetComponent<AudioSource>();
        level_song.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 map_pos = Map.transform.position;

        timer += Time.deltaTime;
        map_pos.y -= map_speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.B) && boss_time == false)
        {
            map_pos.y = BossEncounterTriggerPosition;
        }

            if (map_pos.y > EnemySpawnPosition)
        {
            if (enemy_type == 0 && timer >= 3f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\batship sine flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 1 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\batship cosine flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 2 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\asteroid field"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 3 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\crabship sine flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 4 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\demonic eyeball nzag flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 5 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\stone skull opposite nzag flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 6 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\eye creature random flight group"));
                enemy_type++;
                timer = 0f;
            }
            else if (enemy_type == 7 && timer >= 20f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\gigerball teleport flight group"));
                enemy_type++;
                timer = 0f;

            }
            else if (enemy_type == 8 && timer >= 10f)
            {
                Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Enemies\\Skullship\\skullship"));
                enemy_type = 0;
                timer = 0f;
            }
        }
        else if (map_pos.y <= BossEncounterTriggerPosition && boss_time == false)
        {
            level_song.Stop();
            boss_song.Play();
            boss_time = true;
            boss_ui.enabled = true;
        }

        if (map_pos.y <= LevelMaxPosition)
            map_pos.y = LevelMaxPosition;

        Map.transform.position = map_pos;
    }

    public Vector2 GetMapPosition()
    {
        return Map.transform.position;
    }
}
/*
    private float map_speed;
    //private Camera camera;
    [SerializeField] GameObject Map;
    private AudioSource level_song;
    private AudioSource boss_song;
    private bool boss_time;

    // Start is called before the first frame update
    void Start()
    {
        //camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        map_speed = 1.0f;
        level_song = GameObject.Find("Level 1 Song").GetComponent<AudioSource>();
        boss_song = GameObject.Find("Level 1 Boss Song").GetComponent<AudioSource>();
        level_song.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 map_pos = Map.transform.position;

        map_pos.y += map_speed * Time.deltaTime;

        if (map_pos.y >= 588.47f && boss_time == false)
        {
            level_song.Stop();
            boss_song.Play();
            boss_time = true;
        }

        if (map_pos.y >= 609.91f)
            map_pos.y = 609.91f;

        Map.transform.position = map_pos;
    } 
*/

/*
    private float map_speed;
    private Camera camera;
    [SerializeField] GameObject Map;
    private AudioSource level_song;
    private AudioSource boss_song;
    private bool boss_time;


    [Header ("Follow Player Camera Behavior")]
    [SerializeField] private Transform player;
    private bool followPlayer = true;

    private Vector3 offset = new Vector3(0, 0, -10);
    public Vector3 minPos = new Vector3(-5, 0, 0);
    public Vector3 maxPos = new Vector3(5, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        map_speed = 1.0f;
        level_song = GameObject.Find("Level 1 Song").GetComponent<AudioSource>();
        boss_song = GameObject.Find("Level 1 Boss Song").GetComponent<AudioSource>();
        level_song.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 map_pos = Map.transform.position;

        map_pos.y -= map_speed * Time.deltaTime;

        if (map_pos.y <= -588.47f && boss_time == false)
        {
            level_song.Stop();
            boss_song.Play();
            boss_time = true;
        }

        if (map_pos.y <= -609.91f)
            map_pos.y = -609.91f;

        Map.transform.position = map_pos;
    }

    void LateUpdate()
    {
        if (transform.position != player.position && followPlayer)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, 0);

            targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);

            transform.position = targetPos + offset;
        }
    } 
*/