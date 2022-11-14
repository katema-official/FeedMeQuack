using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {

    public class ExitPlayerFromRiverComponent : MonoBehaviour
    {

        private LevelStageManagerComponent _levelStageManager;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.name == "DummyPlayer")
            {    //TODO: Change the name when you have the player

                if (transform.parent.name == "North")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.North);
                }
                if (transform.parent.name == "South")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.South);
                }
                if (transform.parent.name == "West")
                {
                    _levelStageManager.ExitLake(EnumsDungeon.CompassDirection.West);
                }
                if (transform.parent.name == "East")
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

        // Update is called once per frame
        void Update()
        {

        }
    }
}