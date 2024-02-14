using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    
    [SerializeField]
    private Renderer bgRenderer;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(0, speed*Time.deltaTime);
    }
}
