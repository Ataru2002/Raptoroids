using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteManager : MonoBehaviour
{
    // Start is called before the first frame update
    static ParasiteManager instance;
    public static ParasiteManager Instance { get { return instance; } }

    bool bossShielded;
     private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
            
        }
    }

    void Start()
    {
        bossShielded = false;
    }

    public bool BossShieldStatus(){
        return bossShielded;
    }

    public void SetShieldStatus(bool status){
        bossShielded = status;
    }
}
