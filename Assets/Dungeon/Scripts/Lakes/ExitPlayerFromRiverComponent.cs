using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {

    public class ExitPlayerFromRiverComponent : MonoBehaviour
    {

        private LevelStageManagerComponent _levelStageManager;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {    //TODO: Change the name when you have the player

                if (transform.parent.name == "RiverNorth")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.North);
                }
                if (transform.parent.name == "RiverSouth")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.South);
                }
                if (transform.parent.name == "RiverWest")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.West);
                }
                if (transform.parent.name == "RiverEast")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.East);
                }

            }
        }





        // Start is called before the first frame update
        void Start()
        {
            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
        }

        
    }
}