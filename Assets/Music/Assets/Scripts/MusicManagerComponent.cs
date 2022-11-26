using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Music.Assets.Scripts
{

    public class MusicManagerComponent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            UniversalAudio.InitAllCoroutine(this.gameObject);
            StartCoroutine(UniversalAudio.UpdateTime());
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}