using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class StatusViewController : MonoBehaviour
{
    private SpriteRenderer _icon = null;
    private TextMeshPro _label = null;

    bool _miniStatusActive = false;
    bool _interactionActive = false;

    bool[] _messagesA  = new bool[6];
    string[] _messages = { 
        "<color=\"yellow\">E: <color=\"white\">Eat\n<color=\"yellow\">Q: <color=\"white\">Catch", 
        "<color=\"yellow\">Left Shift: <color=\"white\">Stop Dash", 
        "<color=\"yellow\">Q: <color=\"white\">Spit", 
        "<color=\"yellow\">Space: <color=\"white\">Steal",
        "<color=\"yellow\">E: <color=\"white\">Buy",
        "<color=\"yellow\">Left Shift: <color=\"white\">Dash",
    };
    string[] _messagesXbox = { 
        "<color=\"green\">A: <color=\"white\">Eat\n<color=\"blue\">X: <color=\"white\">Catch", 
        "<color=\"red\">B: <color=\"white\">Stop Dash", 
        "<color=\"blue\">X: <color=\"white\">Spit", 
        "<color=\"yellow\">Y: <color=\"white\">Steal",
        "<color=\"green\">A: <color=\"white\">Buy",
        "<color=\"red\">B: <color=\"white\">Dash"
    };

    public void SetMiniStatusActive(bool active)
    {
        _miniStatusActive = active;
        transform.Find("MiniStatus").gameObject.SetActive(active);
        Active();
    }
    public void SetInteractionActive(bool active, int type)
    {
        _messagesA[type] = active;
        //_interactionActive = active;

        //transform.Find("Interaction/BreadInteraction").gameObject.SetActive(false);
        //transform.Find("Interaction/DashInteraction").gameObject.SetActive(false);
        //transform.Find("Interaction/SpitInteraction").gameObject.SetActive(false);
        //transform.Find("Interaction/EnemyInteraction").gameObject.SetActive(false);

        //if (type == 0) transform.Find("Interaction/BreadInteraction").gameObject.SetActive(_interactionActive);
        //if (type == 1) transform.Find("Interaction/DashInteraction").gameObject.SetActive(_interactionActive);
        //if (type == 2) transform.Find("Interaction/SpitInteraction").gameObject.SetActive(_interactionActive);
        //if (type == 3) transform.Find("Interaction/EnemyInteraction").gameObject.SetActive(_interactionActive);

        Active();
    }






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
        //if (!visible)
        //{
        //    transform.Find("MiniStatus").gameObject.SetActive(false);
        //    transform.Find("Interaction/BreadInteraction").gameObject.SetActive(false);
        //    transform.Find("Interaction/DashInteraction").gameObject.SetActive(false);
        //    transform.Find("Interaction/SpitInteraction").gameObject.SetActive(false);
        //    transform.Find("Interaction/EnemyInteraction").gameObject.SetActive(false);
        //}

        //gameObject.SetActive(visible);
    }


    private void Active()
    {
        //if (_interactionActive)
        //{
        //    transform.Find("MiniStatus").localPosition = new Vector3(0, 2.21f, 0);
        //}
        //else
        //{
        //    transform.Find("MiniStatus").localPosition = new Vector3(0, 0.0f, 0);
        //}

        string text = "";
        int lines = 0;
        for (int i = 0; i < 6; i++)
        {   
            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (_messagesA[i])
                {
                    if (lines >= 1) text += "\n";
                    text += _messagesXbox[i];
                    lines++;
                }
                continue;
            }

            if (_messagesA[i])
            {
                if (lines >= 1) text += "\n";
                text += _messages[i];
                lines++;
            }
        }

        transform.Find("Interaction/Text").GetComponent<TextMeshPro>().text = text;

        if (text.Length > 0)
        {
            transform.Find("MiniStatus").localPosition = new Vector3(0, 2.21f, 0);
        }
        else
        {
            transform.Find("MiniStatus").localPosition = new Vector3(0, 0.0f, 0);
        }
    }


    void Awake()
    {
        _icon = transform.Find("MiniStatus/Sprite").GetComponent<SpriteRenderer>();
        _label = transform.Find("MiniStatus/Name").GetComponent<TextMeshPro>();
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
