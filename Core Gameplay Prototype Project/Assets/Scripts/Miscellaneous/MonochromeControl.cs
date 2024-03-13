using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MonochromeControl : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Grayscale"))
        {
            PlayerPrefs.SetInt("Grayscale", 0);
        }

        Refresh();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        volume.enabled = (PlayerPrefs.GetInt("Grayscale") != 0);
    }
}
