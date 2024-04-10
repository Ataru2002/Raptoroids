using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gem Data", menuName = "Gem Data")]
public class GemProjectileData : ScriptableObject
{
    public Sprite gemSprite;
    public int gemValue;
}
