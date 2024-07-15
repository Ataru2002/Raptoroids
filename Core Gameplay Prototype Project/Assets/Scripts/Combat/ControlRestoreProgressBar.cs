using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ControlRestoreProgressBar : MonoBehaviour
{
    Image target;

    float freezeTime;
    float timePassed;

    void Awake()
    {
        target = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        target.fillAmount = timePassed / freezeTime;
        target.material.SetFloat("_TimeElapsed", timePassed);
    }

    public void ResetDuration(float duration)
    {
        freezeTime = duration;
        target.material.SetFloat("_Duration", duration);

        timePassed = 0;
        target.material.SetFloat("_TimeElapsed", 0);
    }
}
