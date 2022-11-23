using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Enemies.Colliders;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class ColliderManager : MonoBehaviour
    {
        //[SerializeField] private GameObject innerCollider, mediumCollider, outerCollider;

        [SerializeField] private Collider2D _collider2D;

        [SerializeField] private EnemyCustomCollider _myCollider;

        [SerializeField] private float detectionChance;

        private CollisionManager _collisionManager;


        private void Awake(){
            _collisionManager = GetComponentInParent<CollisionManager>();
        }

        // Start is called before the first frame update
        void Start(){
            _collisionManager.AddSelfToColliderManagers(this);
        }

        private void OnTriggerEnter2D(Collider2D col){
            if (col.gameObject.GetComponentInParent<EnemyFSM>() != null) return;
            if (_collisionManager.IsEating()){
                return;
            }

            if (col.gameObject.GetComponent<PlayerDuck>() != null){
                _collisionManager.CheckStealingOptions(col.gameObject);
            }
            else{
                _collisionManager.BreadDetectedAction(col, _myCollider);
            }
        }



        public void TurnOnCollider(){
            GameObject collider2DGameObject = _collider2D.gameObject;
            collider2DGameObject.SetActive(true);
        }

        public void TurnOffCollider(){
            GameObject collider2DGameObject = _collider2D.gameObject;
            collider2DGameObject.SetActive(false);
        }

        public void InitializeValuesAndName(EnemyColliderType type, Species species){
            _myCollider = new EnemyCustomCollider(species, type);
            gameObject.name = type.ToString();
            CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
            circleCollider2D.radius = _myCollider.Radius;
            detectionChance = _myCollider.DetectionChance;
            _collider2D.gameObject.SetActive(true);
        }
    }
}
