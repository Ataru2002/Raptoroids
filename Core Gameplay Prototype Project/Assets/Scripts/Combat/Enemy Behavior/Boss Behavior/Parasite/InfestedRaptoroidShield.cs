using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedRaptoroidShield : MonoBehaviour, IBulletHittable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Intentionally left empty just so that the shield guards against player fire
    // and do nothing else
    public void OnBulletHit(int damage = 1)
    {
        CombatSFXManager.PlaySoundAtLocation("ShieldHit", transform.position);
    }
}
