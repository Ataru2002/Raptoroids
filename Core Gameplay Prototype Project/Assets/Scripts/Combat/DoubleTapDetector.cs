using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoubleTapDetector : MonoBehaviour
{
    private float doubleClickWindow = 0.2f;
    private float lastClickTime = 0f;

    public UnityEvent doubleTap;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float sinceLastClick = Time.unscaledTime - lastClickTime;

            if (sinceLastClick <= doubleClickWindow)
            {
                doubleTap.Invoke();
            }
            lastClickTime = Time.unscaledTime;
        }
    }
}
