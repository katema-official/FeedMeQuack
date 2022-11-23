using System;

namespace Enemies.Colliders
{
    public class EnemyCustomCollider
    {
        public float Radius, DetectionChance;
        public EnemyColliderType ColliderType;

        public EnemyCustomCollider(Species species, EnemyColliderType colliderType){
            ColliderType = colliderType;

            switch (colliderType){
                case EnemyColliderType.Inner:
                    Radius = species.innerRadiusCollider;
                    DetectionChance = species.innerColliderChance;
                    break;
                case EnemyColliderType.Medium:
                    Radius = species.mediumRadiusCollider;
                    DetectionChance = species.mediumColliderChance;
                    break;
                case EnemyColliderType.Outer:
                    Radius = species.outerRadiusCollider;
                    DetectionChance = species.outerColliderChance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colliderType), colliderType, null);
            }
        }
    }
}