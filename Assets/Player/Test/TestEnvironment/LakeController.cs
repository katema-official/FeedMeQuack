using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class LakeController : MonoBehaviour
    {
        private Bounds _dimension;
        public Bounds getBounds()
        {
            return _dimension;
        }

        private void Awake()
        {
            _dimension = transform.Find("Ground").GetComponent<SpriteRenderer>().bounds;
        }
    }
}