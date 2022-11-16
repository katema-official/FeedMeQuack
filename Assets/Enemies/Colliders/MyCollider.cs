using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptCollider", menuName = "MyCollider")]
public class MyCollider : ScriptableObject
{
    public float radius, detectionChance;
    public RadiusType radiusType;
    
    
    public enum RadiusType
    {
        Inner, 
        Medium, 
        Outer
    }
}
