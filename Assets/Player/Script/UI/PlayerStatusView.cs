using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerStatusView : MonoBehaviour
{
    private Image _icon = null;
    private TextMeshProUGUI _label = null;

    void Awake()
    {
        _icon = transform.Find("StatusIcon").GetComponent<Image>();
        _label = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
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
