using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroSceneActionPrompts : MonoBehaviour
{
    Color[] flashColors = { new Color(197f / 255f, 197f / 255f, 197f / 255f), new Color(197f / 255f, 197f / 255f, 197f / 255f, 0.4f) };
    [SerializeField] TextMeshProUGUI[] textFields;

    const float stepTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlashSequence());
    }

    IEnumerator FlashSequence()
    {
        float timePassed = 0;
        int step = 0;
        while (timePassed < 3.0f)
        {
            foreach (TextMeshProUGUI field in textFields)
            {
                field.color = flashColors[step % 2];
            }

            step++;

            yield return new WaitForSeconds(stepTime);
            timePassed += stepTime;
        }

        gameObject.SetActive(false);
    }
}
