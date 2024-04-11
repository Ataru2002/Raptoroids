using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetBoss(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBoss(int bossIndex)
    {
        GameManager.Instance.BossID = bossIndex;
    }

    public void GoToBoss()
    {
        GameManager.Instance.SetMapTier(4);
        SceneManager.LoadScene("CombatStagePrototype");
    }
}
