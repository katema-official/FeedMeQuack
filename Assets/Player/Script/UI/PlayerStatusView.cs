using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerStatusView : MonoBehaviour
{
    private Image _icon = null;
    private TextMeshProUGUI _label = null;

    public void SetIcon(Sprite icon)
    {
        _icon.sprite = icon;
    }
    public void SetText(string text)
    {
        _label.text = text;
    }
    public void SetPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

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
