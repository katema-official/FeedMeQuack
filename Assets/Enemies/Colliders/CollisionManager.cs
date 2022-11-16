using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> mediumColliderBreadAlreadyAnalyzed, outerColliderBreadAlreadyAnalyzed;

    private List<ColliderManager> _myColliderManagers;

    public GameObject BreadTargeted;

    private EnemyFSM _enemyFsm;

    private void Awake(){
        _enemyFsm = GetComponentInParent<EnemyFSM>();
        _myColliderManagers = new List<ColliderManager>();
    }

    public bool IsEating(){
        return _enemyFsm.breadBeingEaten!=null;
    }

    public void CheckStealingOptions(GameObject colGameObject){
        Debug.Log("Checking stealing options!");
    }

    public void BreadDetectedAction(Collider2D col, MyCollider myCollider){
        GameObject breadGameObject = col.gameObject;
        if(HasBreadAlreadyBeenAnalyzed(breadGameObject, myCollider.radiusType)) return;
        AddBreadToAnalyzedOnes(breadGameObject, myCollider.radiusType);
        if (CheckIfInterestedInBread(myCollider) && !AlreadyMovingToCloserBread(breadGameObject)){
                _enemyFsm.TargetBread(breadGameObject);
        }
    }

    private bool HasBreadAlreadyBeenAnalyzed(GameObject breadGameObject, MyCollider.RadiusType radiusType){
        if ((radiusType == MyCollider.RadiusType.Outer && outerColliderBreadAlreadyAnalyzed.Contains(breadGameObject)) ||
            (radiusType == MyCollider.RadiusType.Medium) && mediumColliderBreadAlreadyAnalyzed.Contains(breadGameObject)) return true;
        return false;
    }

    private void AddBreadToAnalyzedOnes(GameObject breadGameObject, MyCollider.RadiusType radiusType){
        if (radiusType == MyCollider.RadiusType.Outer) outerColliderBreadAlreadyAnalyzed.Add(breadGameObject);
        else if(radiusType == MyCollider.RadiusType.Medium) mediumColliderBreadAlreadyAnalyzed.Add(breadGameObject);
    }

    private bool CheckIfInterestedInBread(MyCollider myCollider){
        float rand = Random.value;
        if (rand < myCollider.detectionChance) return true;
        return false;
    }

    private bool AlreadyMovingToCloserBread(GameObject breadGameObject){
        return (BreadTargeted != null && Distance(BreadTargeted) < Distance(breadGameObject));
    }

    private float Distance(GameObject destinationGameObject){
        Vector3 dest = destinationGameObject.transform.position;
        return math.distance(dest, transform.position);
    }

    public void AddSelfToColliderManagers(ColliderManager colliderManager){
        _myColliderManagers.Add(colliderManager);
    }

    public void RestartColliders(){
        foreach (var colliderManager in _myColliderManagers){
            colliderManager.RestartCollider();
        }
    }
}
