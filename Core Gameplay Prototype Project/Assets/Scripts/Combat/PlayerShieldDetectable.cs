using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldDetectable : MonoBehaviour, IBulletHittable
{
    WhiteheadAbility playerAbility;
    // Start is called before the first frame update
    void Start()
    {
        playerAbility = FindFirstObjectByType<WhiteheadAbility>();
    }

    public void OnBulletHit(int damage = 1){
        CombatSFXManager.PlaySoundAtLocation("ShieldHit", transform.position);
    }
}
