using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GOButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _enable = false;
    private Image _image = null;
    private TextMeshProUGUI _text = null;
    private GOButtonManager _manager = null;
    [SerializeField] private FMQButtonType _type;
    [SerializeField] private int _index;

    public void OnPointerClick(PointerEventData eventData)
    {
        _manager.OnButtonClick(_type);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _manager.SetCurrentButtonIndex(_index);
        //SetEnable(true);
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
            //_image.gameObject.SetActive(true);
            _text.fontStyle = FontStyles.Underline | FontStyles.Bold;
            _text.color = new Color(255.0f / 255.0f, 157.0f / 255.0f, 58.0f / 255.0f);
        }
        else
        {
            //_image.gameObject.SetActive(false);
            _text.fontStyle = FontStyles.Bold;
            _text.color = Color.white;
        }
        _enable = enable;
    }
    public int GetIndex()
    {
        return _index;
    }
    public FMQButtonType GetButtonType()
    {
        return _type;
    }
    // Start is called before the first frame update
    void Awake()
    {
        _image = transform.GetComponentInChildren<Image>(true);
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
