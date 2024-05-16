/*
 * File name: ExitButton.cs
 * Author: Jacob Roman
 * Company: 5B2P Studios / Roman Entertainment Software LLC
 * Date: 10/22/2022
 * Purpose: Exits the application.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Image exit_image;

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
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

    }

    void Awake()
    {
        exit_image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
