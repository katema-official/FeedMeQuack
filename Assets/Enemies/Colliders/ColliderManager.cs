using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    //[SerializeField] private GameObject innerCollider, mediumCollider, outerCollider;

    [SerializeField] private Collider2D _collider2D;

    [SerializeField] private MyCollider _myCollider;

    private CollisionManager _collisionManager;


    private void Awake(){
        _collisionManager = GetComponentInParent<CollisionManager>();
        GetComponent<CircleCollider2D>().radius = _myCollider.radius;
    }

    // Start is called before the first frame update
    void Start()
    {
        _collisionManager.AddSelfToColliderManagers(this);
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.GetComponentInParent<EnemyFSM>()!=null)   return;
        if (_collisionManager.IsEating()){
            return;
        }
        if (col.gameObject.GetComponent<PlayerDuck>() != null) _collisionManager.CheckStealingOptions(col.gameObject);
        _collisionManager.BreadDetectedAction(col, _myCollider);
    }

    public void TurnOnCollider(){
        GameObject collider2DGameObject = _collider2D.gameObject;
        collider2DGameObject.SetActive(true);
    }

    public void TurnOffCollider(){
        GameObject collider2DGameObject = _collider2D.gameObject;
        collider2DGameObject.SetActive(false);
    }
}
