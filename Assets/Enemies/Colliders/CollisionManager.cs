using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Enemies.Colliders;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> mediumColliderBreadAlreadyAnalyzed, outerColliderBreadAlreadyAnalyzed;

    [SerializeField] private List<ColliderManager> _EnemyCustomColliderManagers;

    public GameObject BreadTargeted;

    [SerializeField] private EnemyFSM _enemyFsm;

    private void Awake(){
        _enemyFsm = GetComponentInParent<EnemyFSM>();
        _EnemyCustomColliderManagers = new List<ColliderManager>();
    }

    public void InitializeColliders(Species species){
        //assegno il tipo di collider ai vari gameObject, e così facendo setto i loro parametri
        _EnemyCustomColliderManagers[0].InitializeValuesAndName(EnemyColliderType.Inner, species);
        _EnemyCustomColliderManagers[1].InitializeValuesAndName(EnemyColliderType.Medium, species);
        _EnemyCustomColliderManagers[2].InitializeValuesAndName(EnemyColliderType.Outer, species);
    }

    public bool IsEating(){
        return _enemyFsm.breadBeingEaten!=null;
    }

    public void CheckStealingOptions(GameObject colGameObject){
        Debug.Log("Checking stealing options!");
    }

    public void BreadDetectedAction(Collider2D col, EnemyCustomCollider enemyCustomCollider){
        EnemyColliderType type = enemyCustomCollider.ColliderType;
        GameObject breadGameObject = col.gameObject;
        if(HasBreadAlreadyBeenAnalyzed(breadGameObject, type)) return;
        AddBreadToAnalyzedOnes(breadGameObject, type);
        if (CheckIfInterestedInBread(enemyCustomCollider) && !AlreadyMovingToCloserBread(breadGameObject)){
                _enemyFsm.TargetBread(breadGameObject);
        }
    }

    private bool HasBreadAlreadyBeenAnalyzed(GameObject breadGameObject, EnemyColliderType radiusType){
        if ((radiusType == EnemyColliderType.Outer && outerColliderBreadAlreadyAnalyzed.Contains(breadGameObject)) ||
            (radiusType == EnemyColliderType.Medium) && mediumColliderBreadAlreadyAnalyzed.Contains(breadGameObject)) return true;
        return false;
    }

    private void AddBreadToAnalyzedOnes(GameObject breadGameObject, EnemyColliderType radiusType){
        if (radiusType == EnemyColliderType.Outer) outerColliderBreadAlreadyAnalyzed.Add(breadGameObject);
        else if(radiusType == EnemyColliderType.Medium) mediumColliderBreadAlreadyAnalyzed.Add(breadGameObject);
    }

    private bool CheckIfInterestedInBread(EnemyCustomCollider enemyCustomCollider){
        float rand = Random.value;
        if (rand < enemyCustomCollider.DetectionChance) return true;
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
        if (_EnemyCustomColliderManagers == null) _EnemyCustomColliderManagers=new List<ColliderManager>();
        _EnemyCustomColliderManagers.Add(colliderManager);
        if (_EnemyCustomColliderManagers.Count == 3){
            InitializeColliders(_enemyFsm.MySpecies);
        }
    }

    public void TurnOnColliders(){
        foreach (var colliderManager in _EnemyCustomColliderManagers){
            colliderManager.TurnOnCollider();
        }
        mediumColliderBreadAlreadyAnalyzed = new List<GameObject>();
        outerColliderBreadAlreadyAnalyzed = new List<GameObject>();
    }

    public void TurnOffColliders(){
        foreach (var colliderManager in _EnemyCustomColliderManagers){
            colliderManager.TurnOffCollider();
        }
    }
}
