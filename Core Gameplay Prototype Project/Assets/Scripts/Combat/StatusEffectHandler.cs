using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    byte statusVector = 0;
    
    public bool HasStatusCondition(StatusEffect status)
    {
        byte checkVector = (byte)status;
        return (statusVector & checkVector) != 0;
    }

    public void SetStatusCondition(StatusEffect status, float duration = 3)
    {
        StartCoroutine(SetStatusTime(status, duration));
    }

    IEnumerator SetStatusTime(StatusEffect status, float duration)
    {
        // For now, assume only one instance of a status can exist
        if (HasStatusCondition(status))
        {
            // yield break breaks a coroutine early. Think returning from a function
            yield break;
        }

        statusVector |= (byte)status;
        yield return new WaitForSeconds(duration);
        // Bitwise XOR flips only the relevant bit from 1 to 0 while keeping all other bits as is
        statusVector ^= (byte)status;
    }
}

public enum StatusEffect
{
    Stun = 0b0000_0001
}
