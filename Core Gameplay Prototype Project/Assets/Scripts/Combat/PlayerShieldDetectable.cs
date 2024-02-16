using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldDetectable : MonoBehaviour, IBulletHittable
{
    PlayerAbility playerAbility;
    // Start is called before the first frame update
    void Start()
    {
        playerAbility = FindFirstObjectByType<PlayerAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit(){
        print("Shield hit");

        playerAbility.deactivateShield();
    }
}
