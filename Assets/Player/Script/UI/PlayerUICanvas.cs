using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUICanvas : MonoBehaviour
{
    private PlayerStatusView _statusView = null;
    void Awake()
    {
        _statusView = transform.Find("PlayerStatusView").GetComponent<PlayerStatusView>();
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
