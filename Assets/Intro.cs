/*
 * File name: Intro.cs
 * Author: Jacob Roman
 * Company: 5B2P Studios / Roman Entertainment Software LLC
 * Date: 10/22/2022
 * Purpose: Runs the company logos and transitions to the title screen.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    private const int MAIN = 0;
    private const int GAME = 1;
    private const int NUM_LOGOS = 2;

    private Canvas intro_canvas;
    private Canvas title_canvas;
    
    private VideoPlayer[] logo_player = new VideoPlayer[NUM_LOGOS];
    private VideoPlayer title_intro_player;
    private VideoPlayer vortex_player;
    
    private AudioSource title_song;
    
    private bool[] logo_playing = new bool[NUM_LOGOS];
    private bool[] logo_enabled = new bool[NUM_LOGOS];
    private bool title_intro_playing;
    private bool title_intro_enabled;
    private bool title_enabled;
    private bool fade_in;
    private bool fade_out;
    private bool is_paused;
    private bool alt_tab;
    //private bool held_down;
    //private bool ok_to_skip_5b2p;
    //private bool ok_to_skip_roman_entertainment;
    private bool ok_to_skip_title_sequence;
    private bool keydown;

    [SerializeField] private CanvasGroup intro_ui_group;
    [SerializeField] private CanvasGroup title_ui_group;

    private int bit;

    private void Awake()
    {
        intro_canvas = GameObject.Find("Intro Canvas").GetComponent<Canvas>();
        title_canvas = GameObject.Find("Title Canvas").GetComponent<Canvas>();
        logo_player[0] = GameObject.Find("Logo Video Player").GetComponent<VideoPlayer>();
        logo_player[1] = GameObject.Find("Logo 2 Video Player").GetComponent<VideoPlayer>();
        title_intro_player = GameObject.Find("Title Intro Video Player").GetComponent<VideoPlayer>();
        vortex_player = GameObject.Find("Vortex Video Player").GetComponent<VideoPlayer>();
        title_song = GameObject.Find("Title Song").GetComponent<AudioSource>();
        intro_ui_group = GameObject.Find("Intro Canvas").GetComponent<CanvasGroup>();
        title_ui_group = GameObject.Find("Title Canvas").GetComponent<CanvasGroup>();

        logo_player[0].frame = 0;
        logo_player[1].frame = 0;
        logo_enabled[0] = true;
        title_canvas.enabled = false;
        title_ui_group.alpha = 0.0f;
        logo_player[0].Play();
        keydown = false;
    }

    void OnApplicationFocus(bool has_focus)
    {
        is_paused = !has_focus;
        alt_tab = !has_focus;
    }

    void OnApplicationPause(bool pause_status)
    {
        is_paused = pause_status;
        alt_tab = pause_status;
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftAlt) ^ Input.GetKey(KeyCode.RightAlt)))
        {
            if ((bit & (1 << 0)) == 0)
                bit |= (1 << 0);
        }
        else
        {
            if ((bit & (1 << 0)) == 1)
                bit &= ~(1 << 0);
        }

        if ((bit & (1 << 0)) == 1)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                if ((bit & (1 << 1)) == 0)
                    bit |= (1 << 1);
            }
            else
            {
                if ((bit & (1 << 1)) == 1)
                    bit &= ~(1 << 1);
            }
        }

        if ((bit & (1 << 0)) == 1 && (bit & (1 << 1)) == 1)
        {
            alt_tab = true;
        }

        // 5B2P Studios Logo
        if (logo_enabled[0])
        {
            if (logo_player[0] != null)
            {
                if (logo_player[0].isPlaying)
                {
                    if (Input.anyKey)
                    {
                        if (!keydown)
                        {
                            keydown = true;

                            logo_enabled[0] = false;
                            logo_playing[0] = false;
                            logo_player[0].Stop();

                            logo_enabled[1] = true;
                            logo_player[1].Play(); // Play second logo: Roman Entertainment Software
                        }
                    }
                    else
                    {
                        // Reset the keyPressed flag when no keys are being pressed.
                        keydown = false;
                    }
                }

                // Flip the first video playing to true when the video is playing.
                if (logo_playing[0] == false)
                    if (logo_player[0].isPlaying)
                        logo_playing[0] = true;

                // Check if the video stopped playing
                if (logo_playing[0])
                    if (logo_player[0].isPlaying == false)
                    {
                        logo_enabled[0] = false;
                        logo_playing[0] = false;
                        logo_player[0].Stop();

                        logo_enabled[1] = true;
                        logo_player[1].Play(); // Play second logo: Roman Entertainment Software
                    }
                Debug.Log("5B2B Logo Enabled: " + logo_enabled[0]);
            }
        }

        // Roman Entertainment Software Logo
        if (logo_enabled[1])
        {
            if (logo_player[1] != null)
            {
                if (logo_player[1].isPlaying)
                {
                    if (Input.anyKey)
                    {
                        if (!keydown)
                        {
                            keydown = true;

                            logo_enabled[1] = false;
                            logo_playing[1] = false;
                            logo_player[1].Stop();

                            title_intro_enabled = true;
                            title_intro_player.Play(); // Play the title screen video
                            title_song.Play();
                            ok_to_skip_title_sequence = true;
                        }
                    }
                    else
                    {
                        // Reset the keyPressed flag when no keys are being pressed.
                        keydown = false;
                    }
                }

                // Flip the second video playing to true when the video is playing.
                if (logo_playing[1] == false)
                    if (logo_player[1].isPlaying)
                        logo_playing[1] = true;

                // Check if the video stopped playing
                if (logo_playing[1])
                    if (logo_player[1].isPlaying == false)
                    {
                        logo_enabled[1] = false;
                        logo_playing[1] = false;
                        logo_player[1].Stop();

                        title_intro_enabled = true;
                        title_intro_player.Play(); // Play the title screen video
                        title_song.Play();
                        ok_to_skip_title_sequence = true;
                    }
                Debug.Log("RES Logo Enabled: " + logo_enabled[1]);
            }
        }

        // Title Screen Video
        if (title_intro_enabled)
        {
            if (title_intro_player != null)
            {
                if (title_intro_player.isPlaying)
                {
                    if (Input.anyKey)
                    {
                        if (!keydown)
                        {
                            keydown = true;

                            if (fade_out == false)
                                fade_out = true;
                        }
                    }
                    else
                    {
                        keydown = false;
                    }
                }

                // Flip the second video playing to true when the video is playing.
                if (title_intro_playing == false)
                        if (title_intro_player.isPlaying)
                            title_intro_playing = true;

                // Check if the video ended, then jump to the title screen.
                if (title_intro_playing)
                    if (title_intro_player.isPlaying == false)
                    {
                        fade_out = false;
                        title_intro_enabled = false;
                        title_intro_player.Stop();
                        title_enabled = true;
                        title_intro_playing = false;
                        fade_in = true;
                        vortex_player.Play();
                        intro_canvas.enabled = false;
                        title_canvas.enabled = true;
                    }

                // A key was pressed during the video, so jump to the title screen.
                if (fade_out)
                    if (intro_ui_group.alpha > 0.0f)
                        intro_ui_group.alpha -= Time.deltaTime;
                    else
                    {
                        fade_out = false;
                        title_intro_enabled = false;
                        title_intro_player.Stop();
                        title_enabled = true;
                        title_intro_playing = false;
                        fade_in = true;
                        vortex_player.Play();
                        intro_canvas.enabled = false;
                        title_canvas.enabled = true;
                    }
                Debug.Log("Title Intro Enabled: " + title_intro_enabled);
            }
        }

        if (title_enabled)
        {
            if (vortex_player != null)
            {
                if (fade_in)
                    if (title_ui_group.alpha < 1.0f)
                        title_ui_group.alpha += Time.deltaTime;
                    else
                        fade_in = false;

                // Probably can use this within another script
                if (fade_out)
                    if (title_ui_group.alpha > 0.0f)
                        title_ui_group.alpha -= Time.deltaTime;
                    else
                    {
                        fade_out = false;
                    }
                Debug.Log("Title Enabled: " + title_enabled);
            }
        }
    }
}
