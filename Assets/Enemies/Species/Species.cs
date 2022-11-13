using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Species", menuName = "Species")]
public class Species : ScriptableObject
{
    public float maxSpeed, mouthSize, eatingSpeed=1, steeringSpeed, idleTime, idleCD, accelerationTimeSeconds;
    
    
    
    public float steeringAngleMax=45;
    public float steeringCd=5f;
    public float innerRadiusCollider, mediumRadiusCollider, outerRadiusCollider;
    public float stealingCd, stealingPerc;
    public int percInnerRadius, percMediumRadius, percOuterRadius;
}
