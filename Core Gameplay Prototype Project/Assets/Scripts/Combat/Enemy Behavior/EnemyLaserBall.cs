using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBall : MonoBehaviour
{
    [SerializeField] float startSize;
    [SerializeField] float finalSize;
    [SerializeField] float growthTime;
    float timeSinceActivation = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceActivation += Time.deltaTime;
        transform.localScale = Vector3.one * Mathf.Lerp(startSize, finalSize, timeSinceActivation / growthTime);
    }

    public void ResetGrowth()
    {
        timeSinceActivation = 0;
    }
}
