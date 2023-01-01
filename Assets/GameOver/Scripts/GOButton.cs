using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GOButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{



    private bool _enable = false;
    private Image _image = null;
    private TextMeshProUGUI _text = null;
    private GOButtonManager _manager = null;


    public void OnPointerEnter(PointerEventData eventData)
    {
        _manager.SetEnableButtons(false);
        SetEnable(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //_manager.SetEnableButtons(false);
        //SetEnable(false);
    }
    public void SetEnable(bool enable)
    {
        if (enable)
        {
            _image.gameObject.SetActive(true);
            _text.fontStyle = FontStyles.Underline;
            _text.color = new Color(255.0f / 255.0f, 157.0f / 255.0f, 58.0f / 255.0f);
        }
        else
        {
            _image.gameObject.SetActive(false);
            _text.fontStyle = FontStyles.Normal;
            _text.color = Color.white;
        }
        _enable = enable;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _image = transform.GetComponentInChildren<Image>();
        _text = transform.GetComponentInChildren<TextMeshProUGUI>();
        _manager = transform.parent.GetComponent<GOButtonManager>();
       // SetEnable(false);
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
