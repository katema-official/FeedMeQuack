using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class PlayerUICanvas : MonoBehaviour
    {
        private StatusViewController  _statusView = null;

        public StatusViewController GetStatusView()
        {
            return _statusView;
        }

        void Awake()
        {
            _statusView = transform.Find("PlayerStatusView").GetComponent<StatusViewController>();
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
