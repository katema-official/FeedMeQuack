using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Enemies
{
    public class CollectBreadScript : MonoBehaviour
    {
        public GameObject fatherGameObject;
        [SerializeField] private EnemyFSM _fsmScript;

        // Start is called before the first frame update
        /*void Start(){
            _fsmScript = fatherGameObject.GetComponent<EnemyFSM>();
        }*/

        private void Awake(){
            _fsmScript = fatherGameObject.GetComponent<EnemyFSM>();
        }

        private void OnTriggerEnter2D(Collider2D other){
            if (other.gameObject.tag != "FoodInWater") return;  //TODO: controllare se è il giocatore
            //if (other.GetComponent<EnemyFSM>() != null || other.GetComponentInParent<EnemyFSM>() != null) return;
            if (_fsmScript.breadBeingEaten != null) return;
            if (!_fsmScript.IsEating()) _fsmScript.StartEatingBread(other.gameObject);
        }

        public void ResetCollider()
        {
            GameObject collider2DGameObject = gameObject.GetComponent<Collider2D>().gameObject;
            collider2DGameObject.SetActive(false);
            collider2DGameObject.SetActive(true);
        }


    }
}