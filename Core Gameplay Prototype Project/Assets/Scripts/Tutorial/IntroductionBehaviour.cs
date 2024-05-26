using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action onIntroduction;
    [SerializeField] float moveTimeRequired = 2f;
    float moveTime = 0;

    private void Update()
    {
        if (Input.mousePositionDelta != Vector3.zero)
        {
            moveTime += Time.deltaTime;
        }

        if (moveTime >= moveTimeRequired)
        {
            onIntroduction?.Invoke();
            this.enabled = false;
        }
    }
}
