using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Species", menuName = "Species")]
public class Species : ScriptableObject
{
    public float maxSpeed, mouthSize, eatingSpeed=1, idleTime, idleCD, accelerationTimeSeconds;
    
    
    public float innerRadiusCollider, mediumRadiusCollider, outerRadiusCollider, innerColliderChance, mediumColliderChance, outerColliderChance;
    public float stealingCd, stealingPerc;
}
