using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class PlayerCircleComponent : MonoBehaviour
    {

        //this component just signals to the IdentifyPlayerComponent if the player has been identified

        [SerializeField] private int ID = 0;
        private IdentifyPlayerComponent _identifyPlayerComponent;
        private float _radius;

        void Awake()
        {
            _identifyPlayerComponent = transform.parent.GetComponent<IdentifyPlayerComponent>();
        }

        void Start()
        {
            _radius = GetComponent<CircleCollider2D>().radius;
        }

        /*private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                _identifyPlayerComponent.NotifyPlayerIdentified(collision.gameObject, ID);
            }
        }*/


        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckPlayer(collision);
        }

        private void CheckPlayer(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _identifyPlayerComponent.NotifyPlayerIdentified(collision.gameObject, ID);
            }
        }
        public void NotifyPlayerNear(Collider2D collision)
        {
            if (Vector2.Distance(transform.position, collision.gameObject.transform.position) > _radius + 0.5f) return;
            CheckPlayer(collision);
        }


    }
}