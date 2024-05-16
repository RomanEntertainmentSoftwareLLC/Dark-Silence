/*
 * File name: StartButton.cs
 * Author: Jacob Roman
 * Company: 5B2P Studios / Roman Entertainment Software LLC
 * Date: 10/22/2022
 * Purpose: TODO
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool game_enabled;
    private Canvas title_canvas;
    [SerializeField] private CanvasGroup title_ui_group;
    private AudioSource title_song;
    private bool fade_out;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.35f, 0.35f, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (title_ui_group.alpha == 1.0f)
            {
                if (!game_enabled)
                {
                    Debug.Log("Click");
                    game_enabled = true;
                    fade_out = true;
                }
            }
        }
    }

    void Awake()
    {
        title_canvas = GameObject.Find("Title Canvas").GetComponent<Canvas>();
        title_ui_group = GameObject.Find("Title Canvas").GetComponent<CanvasGroup>();
        title_song = GameObject.Find("Title Song").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game_enabled)
            if (fade_out)
                if (title_ui_group.alpha > 0.0f)
                    title_ui_group.alpha -= Time.deltaTime;
                else
                {
                    fade_out = false;
                    title_song.Stop();
                    title_canvas.enabled = false;
                    SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                }
    }
}
