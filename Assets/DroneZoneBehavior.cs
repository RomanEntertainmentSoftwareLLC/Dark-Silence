using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneZoneBehavior : MonoBehaviour
{
    [SerializeField] private GameObject drone;
    [SerializeField] private int numDrones;
    [SerializeField] private bool start = true;

    [SerializeField] private Transform level;
    [SerializeField] private AudioSource warp_sound;

    private Player player;

    private BossEyeBehavior bosseyebehavior;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        bosseyebehavior = GameObject.Find("Left Eye Look").GetComponent<BossEyeBehavior>();
    }

    private void Update()
    {
        if (level.position.y <=  -629.25f)
        {
            if (bosseyebehavior.dead == false)
            {
                if (start)
                {
                    StartCoroutine(SpawnDrones());
                    StartCoroutine(SpawnAsteroids());
                }
            }
        }
    }

    IEnumerator SpawnAsteroids()
    {
        if (bosseyebehavior.dead == false)
        {
            yield return new WaitForSeconds(30f);
            Instantiate(Resources.Load("Graphics\\Game\\Sprites\\Flight Groups\\asteroid field"));
            StartCoroutine(SpawnAsteroids());
        }
    }

    IEnumerator SpawnDrones()
    {
        if (bosseyebehavior.dead == false)
        {
            if (start)
            {
                start = false;
                yield return new WaitForSeconds(5f);
            }

            if (numDrones < 10 && !start)
            {
                yield return new WaitForSeconds(1f);

                //Debug.Log("I FIRED!");

                var obj = Instantiate(drone, transform.position, Quaternion.identity);
                obj.GetComponent<Rigidbody2D>().velocity += new Vector2(Random.Range(-5, 5), Random.Range(-1, -5));
                numDrones++;

                if (warp_sound != null)
                    warp_sound.Play();
            }

            if (numDrones != 10)
            {
                StartCoroutine(SpawnDrones());
            }
            else
            {
                numDrones = 0;
                start = true;
                StartCoroutine(SpawnDrones());
            }
        }
    }
}
/*
IEnumerator AgravateDrones()
    {
        var posTarget = transform.position - new Vector3 (2, 2, 0);
        var pos = transform.position;

        yield return new WaitForSeconds(13f);

        transform.position = Vector2.MoveTowards(transform.position, posTarget, 10 * Time.deltaTime);

        yield return new WaitForSeconds(2f);

        transform.position = Vector2.MoveTowards(transform.position, pos, 10 * Time.deltaTime);
    } 

void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == 6)
            numDrones++;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
            numDrones--;
    }
*/
/*
[SerializeField] private GameObject drone;
    [SerializeField] private int numDrones;
    [SerializeField] private bool start = true;

    [SerializeField] private Transform level;

    public GameObject player;

    private void Update()
    {
        if (level.position.y == -633f)
        {
            if (start)
            {
                StartCoroutine(SpawnDrones());
            }
        }
    }

    IEnumerator SpawnDrones()
    {
        print("SpawnDrones");// 
        if (start)
        {
            start = false;
            yield return new WaitForSeconds(5f);
        }

        if (numDrones < 10 && !start)
        {
            yield return new WaitForSeconds(1f);

            var obj = Instantiate(drone, transform.position, Quaternion.identity);
            obj.GetComponent<EnemyBehavior>().player = GameObject.Find("Player");
            obj.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(Random.Range(-20, 20), Random.Range(-20, 20));
            numDrones++;
        }

        //StartCoroutine(SpawnDrones());
    }
*/