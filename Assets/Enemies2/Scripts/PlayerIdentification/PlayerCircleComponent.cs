using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class PlayerCircleComponent : MonoBehaviour
    {

        //this component just signals to the IdentifyPlayerComponent if the player has been identified

        [SerializeField] private int ID = 0;
        IdentifyPlayerComponent _identifyPlayerComponent;


        void Awake()
        {
            _identifyPlayerComponent = transform.parent.GetComponent<IdentifyPlayerComponent>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                _identifyPlayerComponent.NotifyPlayerIdentified(collision.gameObject, ID);
            }
        }
    }
}