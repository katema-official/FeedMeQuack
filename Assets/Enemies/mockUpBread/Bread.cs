using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : MonoBehaviour
{
    public int BreadPoints=5;

    public bool IsInWater=true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D colliderDuck){
        GameObject collGameObject = colliderDuck.gameObject;
        if (collGameObject.GetComponent<EatBreadScript>() == null){
        }
        else{
            EnemyFSM enemyDuck = collGameObject.GetComponentInParent<EnemyFSM>();
            enemyDuck.StartEatingBread(this.gameObject);
        }
    }
}
