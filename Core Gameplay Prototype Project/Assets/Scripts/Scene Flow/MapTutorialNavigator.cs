using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTutorialNavigator : MonoBehaviour
{
    [SerializeField] GameObject nodesCanvas;
    [SerializeField] GameObject progressionCanvas;
    [SerializeField] GameObject mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectSubmenu(string submenu)
    {
        mainCanvas.SetActive(false);

        switch (submenu)
        {
            case "nodes":
                nodesCanvas.SetActive(true);
                break;
            case "progression":
                progressionCanvas.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ReturnToMapMain(string submenu)
    {
        mainCanvas.SetActive(true);

        switch (submenu)
        {
            case "nodes":
                nodesCanvas.SetActive(false);
                break;
            case "progression":
                progressionCanvas.SetActive(false);
                break;
            default:
                break;
        }
    }
}
