using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Player player;
    private bool followPlayer = true;

    private Vector3 offset = new Vector3(0, 0, -10);
    public Vector3 minPos = new Vector3(-2.9f, 0, 0);
    public Vector3 maxPos = new Vector3(2.9f, 0, 0);

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void LateUpdate()
    {
        if (transform != null)
        {
            if (transform.position != player.transform.position && followPlayer)
            {
                Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);

                targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);

                transform.position = targetPos + offset;
            }
        }

    }
}
