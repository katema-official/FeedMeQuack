using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{
    public class ShopDescriptionComponent : LakeShopDescriptionComponent
    {
        // Start is called before the first frame update
        void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = new Vector3(0, 0, 0);
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}