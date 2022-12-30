using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class StatusViewController : MonoBehaviour
{
    private SpriteRenderer _icon = null;
    private TextMeshPro _label = null;

    public void SetIcon(Sprite icon)
    {
        _icon.sprite = icon;
    }
    public void SetText(string text)
    {
        if (text.Length == 0)
        {
            _icon.gameObject.transform.localPosition = new Vector3(0, 0, _icon.gameObject.transform.localPosition.z);
            _label.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, _label.gameObject.GetComponent<RectTransform>().localPosition.z);
        }
        else
        {
            _icon.gameObject.transform.localPosition = new Vector3(-0.9f, 0, _icon.gameObject.transform.localPosition.z);
            _label.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0.9f, 0, _label.gameObject.GetComponent<RectTransform>().localPosition.z);
        }



        _label.text = text;
    }
    public void SetPosition(Vector2 pos)
    {
        GetComponent<Transform>().position = new Vector3(pos.x,pos.y, 3);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    void Awake()
    {
        _icon = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _label = transform.Find("Name").GetComponent<TextMeshPro>();
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
