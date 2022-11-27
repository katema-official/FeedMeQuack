using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Player
{
    public class EnemyController : MonoBehaviour
    {
        private LakeController _currentLake = null;
        private BreadController _catchedBread = null;
        private Transform _mouth = null;


        // Start is called before the first frame update
        void Awake()
        {
            _mouth = transform.Find("Mouth");
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var lakeController = collision.gameObject.GetComponentInChildren<LakeController>();
            if (lakeController) {
                _currentLake = lakeController;
                _catchedBread = _currentLake.GenerateNewBread();
                _catchedBread.Move(_mouth.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var lakeController = collision.gameObject.GetComponentInChildren<LakeController>();
            if (lakeController) _currentLake = null;
        }
    }
}
