using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "New Species", menuName = "Species")]
    public class Species : ScriptableObject
    {
        public int mouthSize;

        public float maxSpeed,
            chewingRate,
            idleTime,
            movementDuration,
            accelerationTimeSeconds,
            chillingTime,
            steeringValue;


        public float innerRadiusCollider,
            mediumRadiusCollider,
            outerRadiusCollider,
            innerColliderChance,
            mediumColliderChance,
            outerColliderChance;

        public float stealingCd, stealingPerc;
    }
}
