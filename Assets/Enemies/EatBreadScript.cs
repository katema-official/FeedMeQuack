using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class EatBreadScript : MonoBehaviour
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
        _fsmScript.StartEatingBread(other.gameObject);
    }
}
