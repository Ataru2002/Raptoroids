using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MapScreenGemTextControl : MonoBehaviour
{
    LocalizeStringEvent textEvent;

    private void Awake()
    {
        textEvent = GetComponent<LocalizeStringEvent>();
        textEvent.StringReference.Add("collectedGems", new IntVariable { Value = GameManager.Instance.GetCurrentGems() });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
