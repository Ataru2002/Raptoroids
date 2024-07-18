using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] bool clockwise;

    [Tooltip("Describes the rotation speed in revolutions per second")]
    [SerializeField] float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        float angleDelta = rotateSpeed * 360 * Time.deltaTime;
        float direction = clockwise ? -angleDelta : angleDelta;
        transform.Rotate(0, 0, direction);
    }

    public void SetClockwise(bool val)
    {
        clockwise = val;
    }
}
