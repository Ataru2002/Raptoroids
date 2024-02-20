using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeAnimator : MonoBehaviour
{
    const float minScale = 1.0f;
    const float maxScale = 1.3f;
    const float animateTime = 0.8f;

    bool animating = false;
    int deltaSign = 1;
    float animationStep = 0.0f;

    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            float scale = Mathf.Lerp(minScale, maxScale, animationStep);
            rect.localScale = new Vector3(1, 1, 1) * scale;

            animationStep += Time.deltaTime * deltaSign / animateTime;
            
            if (animationStep >= 1.0f)
            {
                animationStep = 1.0f;
                deltaSign = -1;
            }
            else if (animationStep <= 0.0f)
            {
                animationStep = 0.0f;
                deltaSign = 1;
            }
        }
    }

    public void ToggleAnimation(bool enabling)
    {
        animating = enabling;

        if (!enabling)
        {
            if (rect != null)
            {
                rect.localScale = new Vector3(1, 1, 1);
            }

            animationStep = 0.0f;
        }
    }
}
