using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Player
{
    public class SpitProgressBar : MonoBehaviour
    {
        private GameObject _bar = null;

        public void SetProgress(float value)
        {
            var pos = _bar.transform.localPosition;
            pos.Set(-1.6892f * (1.0f-value), pos.y, pos.z);
            _bar.transform.localPosition = pos; //0.007148883
            _bar.transform.localScale = new Vector3(1.232566f * value, 1.232566f, 1);
        }

        void Awake()
        {
            _bar = transform.Find("Bar").gameObject;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
