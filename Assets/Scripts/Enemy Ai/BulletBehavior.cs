using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public void startTimedDestroy()
    {
        StartCoroutine(TimedDestroy());
    }

    IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    public void Update()
    {
        if (rb.transform.position.x <= -14.7f || rb.transform.position.x >= 14.7f)
            Destroy(gameObject);

        if (rb.transform.position.y <= -5.3f || rb.transform.position.y >= 5.3f)
            Destroy(gameObject);
    }
}
