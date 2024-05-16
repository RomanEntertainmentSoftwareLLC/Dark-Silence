/*
 * File name: OptionsButton.cs
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

public class OptionsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image options_image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.35f, 0.35f, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
    }

    void Awake()
    {
        options_image = GetComponent<Image>();
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
