using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class CollectBreadScript : MonoBehaviour
{
    public GameObject fatherGameObject;
    [SerializeField] private EnemyFSM _fsmScript;
    
    // Start is called before the first frame update
    void Start(){
        _fsmScript = fatherGameObject.GetComponent<EnemyFSM>();
    }

    private void Awake(){
        _fsmScript = fatherGameObject.GetComponent<EnemyFSM>();
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.GetComponent<EnemyFSM>()!=null || other.GetComponentInParent<EnemyFSM>()!=null) return;
        if(_fsmScript.breadBeingEaten!=null) return;
        if(!_fsmScript.IsEating()) _fsmScript.StartEatingBread(other.gameObject);
    }
}
//chewingRate 0.7 => ogni 0.7 sec mangio un bread point