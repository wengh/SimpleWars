﻿using UnityEngine;
using UnityEngine.EventSystems;

public class DragCamera : MonoBehaviour
{
	Vector3 dragOrigin;
    bool dragging = false;
    int button = 1;

    void Awake ()
    {
        button = Input.mousePresent ? 1 : 0;
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(button) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragOrigin = GetComponent<Camera>().ScreenToWorldPoint(dragOrigin);
            dragging = true;
        }

        if (dragging)
        {
            Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            currentPos = GetComponent<Camera>().ScreenToWorldPoint(currentPos);
            Vector3 movePos = dragOrigin - currentPos;
            transform.position = transform.position + movePos;
        }

        if (Input.GetMouseButtonUp(button))
        {
            dragging = false;
        }
    }
}