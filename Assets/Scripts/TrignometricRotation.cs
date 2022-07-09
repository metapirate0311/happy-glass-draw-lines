using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Happy Glass 
/// Credit: Satyam Parkhi
/// Email: satyamparkhi@gmail.com
/// Facebook : https://www.facebook.com/satyamparkhi
/// Instagram : https://www.instagram.com/satyamparkhi/
/// Whatsapp : +91 7050225661
/// </summary>
public class TrignometricRotation : MonoBehaviour
{

    public Vector3 AngleLimit;
    public Vector3 RotationFrequency;
    Vector3 FinalRotation;
    Vector3 StartRotation;
    void Start()
    {
        StartRotation = transform.localEulerAngles;
    }
    void Update()
    {
        if (AngleLimit != Vector3.zero && RotationFrequency != Vector3.zero)
        {
            FinalRotation.x = StartRotation.x + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.x) * AngleLimit.x;
            FinalRotation.y = StartRotation.y + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.y) * AngleLimit.y;
            FinalRotation.z = StartRotation.z + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.z) * AngleLimit.z;
            transform.localEulerAngles = new Vector3(FinalRotation.x, FinalRotation.y, FinalRotation.z);
        }     
    }
}
