﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    EventSystem currentEventSystem;

    void Start()
    {
        currentEventSystem = EventSystem.current;
    }

    void FixedUpdate()
    {
        if (currentEventSystem.currentSelectedGameObject == transform.gameObject)
        {
            transform.SetAsLastSibling();
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(800 * 1.25f, 600 * 1.25f);

            if (Input.GetButtonDown("Submit"))
            {
                //LoadScene();
            }

            if (Input.GetAxisRaw("Horizontal") > 0.1 && Time.time > MainMenu.lastInput + 0.25f)
            {
                transform.SetAsFirstSibling();
                MainMenu.lastInput = Time.time;
                currentEventSystem.SetSelectedGameObject(transform.parent.Find("PlayGameButton").gameObject);
            }
            else if (Input.GetAxisRaw("Horizontal") < -0.1 && Time.time > MainMenu.lastInput + 0.25f)
            {
                transform.SetAsFirstSibling();
                MainMenu.lastInput = Time.time;
                currentEventSystem.SetSelectedGameObject(transform.parent.Find("LevelSelectButton").gameObject);
            }
        }
        else
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
        }
    }

}
