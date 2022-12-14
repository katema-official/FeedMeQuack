using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopNamespace
{

    public class ExitPlayerFromShopComponent : MonoBehaviour
    {

        private LevelStageNamespace.LevelStageManagerComponent _levelStageManager;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                _levelStageManager.ExitShop();

                Music.UniversalAudio.PlayMusic("Swimming", true);
            }
        }





        // Start is called before the first frame update
        void Start()
        {
            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageNamespace.LevelStageManagerComponent>();
        }








        // Update is called once per frame
        void Update()
        {

        }
    }
}