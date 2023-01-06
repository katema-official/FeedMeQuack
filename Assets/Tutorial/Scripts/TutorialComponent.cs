using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialComponent : MonoBehaviour
{
    private int _sceneCount = 0;
    private bool _shopSeen = false;

    private const int _tutorialMovement = 0;
    private const int _tutorialEat = 1;
    private const int _tutorialUI1 = 2;
    private const int _tutorialUI2 = 3;
    private const int _tutorialShop = 4;
    private const int _tutorialCommands1 = 5;
    private const int _tutorialAAA = 6;

    private Dictionary<int, string> _tutorialText = new Dictionary<int, string>()
    {
        {_tutorialMovement, "Use WASD/arrow keys or the left analog stick to move. You can move from a lake to another through a " + ColorString("river.", "0045B7") },
        {_tutorialEat, "Bite the thrown " + ColorString("bread", "CD6E3B") + " with the " + ColorString("E", "494D42") + " key/" + ColorString("Left Mouse Button", "494D42") + " or the " + ColorString("A", "00FD10") + " button when close enough." },
        {_tutorialUI1, "Eating " + ColorString("bread", "CD6E3B") + " gives you " + ColorString("BP (Bread Points)", "CD6E3B") + ", shown in the " + ColorString("upper left corner", "FFFF00") + ". Get enough of them to go to the next level!"},
        {_tutorialUI2, "Use the minimap in the " + ColorString("upper right corner", "FFFF00") + " to orient yourself. The " + ColorString("green square", "00FF00") + " is the lake containing a passage to the next level: just follow the " + ColorString("brown sign", "A18534") + "..."},
        {_tutorialShop, "In the " + ColorString("shop", "FF4301") + ", your " + ColorString("BP", "CD6E3B") + " in excess are converted in " + ColorString("DBP (Digested Bread Points)", "FF4301") + ", that you can use to purchase " + ColorString("Power Ups", "EAD200") + "."},
        {_tutorialCommands1, "Eating is not your only skill.\n Use " + ColorString("Shift", "494D42") + "/" + ColorString("Mouse Wheel Button", "494D42") + " or the " + ColorString("B", "F80000") + " button to dash. Press again to stop.\n"
        + "Use " + ColorString("Space", "494D42") + " or the " + ColorString("Y", "F87700") + " button to steal bread from an enemy. Your victim won't let it go that easily thought...\n" 
        + "Use " + ColorString("Q", "494D42") + "/" + ColorString("Right Mouse Button", "494D42") + " or " + ColorString("X", "0068FF") + " to grab a piece of bread. Then, keep it pressed to charge and spit it!\n"
        + "All your skills have a cooldown. Check them on the statistics menu."}
    };

    private static string ColorString(string s, string color)
    {
        return "<color=#" + color + ">" + s + "</color>";
    }





    private string _tutorialPath = "./Assets/Tutorial/tutorial.txt";
    private GameObject _camera;
    [SerializeField] private TMP_Text _text;

    private int _tutorialIndex = _tutorialMovement;

    private Coroutine _deleteTextCoroutine;


    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        //read if the tutorial must be carried out or not
        StreamReader reader = new StreamReader(_tutorialPath);
        string lineA = reader.ReadLine();
        string[] splitA = lineA.Split(',');
        int tutorial = int.Parse(splitA[1]);
        if(tutorial == 0)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }

        _camera = GameObject.FindGameObjectWithTag("Player").transform.parent.Find("Camera").gameObject;
        ChangeText();

    }


    void Update()
    {
        
    }

    private void ChangeText()
    {
        string textToShow = "";
        switch (_tutorialIndex)
        {
            case _tutorialMovement:
                textToShow = _tutorialText[_tutorialMovement];
                _deleteTextCoroutine = StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialEat;
                break;

            case _tutorialEat:
                textToShow = _tutorialText[_tutorialEat];
                _deleteTextCoroutine = StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialUI1;
                break;

            case _tutorialUI1:
                textToShow = _tutorialText[_tutorialUI1];
                _deleteTextCoroutine = StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialUI2;
                break;

            case _tutorialUI2:
                textToShow = _tutorialText[_tutorialUI2];
                _deleteTextCoroutine = StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialShop;
                break;

            case _tutorialShop:
                textToShow = _tutorialText[_tutorialShop];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands1;
                break;

            case _tutorialCommands1:
                textToShow = _tutorialText[_tutorialCommands1];
                StartCoroutine(DeleteText(25f));
                _tutorialIndex = _tutorialAAA;
                EndTutorial();
                break;

            case _tutorialAAA:

                break;

            default:
                break;


        }
        _text.text = textToShow;
        

        
        
    }


    //private float _deleteTimeText = 8f;
    private IEnumerator DeleteText(float time = 8f)
    {
        yield return new WaitForSeconds(time);
        _text.text = "";

        switch (_tutorialIndex)
        {
            case _tutorialUI2:
                ChangeText();
                break;

        }
        yield return null;
    }








    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _sceneCount++;
        if (scene.name == "LakeSmall" && _sceneCount == 3)
        {
            _tutorialIndex = _tutorialEat;
            StopCoroutine(_deleteTextCoroutine);
            //StartCoroutine(DeleteText(0f));
            ChangeText();
        }
        if(scene.name == "LakeSmall" && _sceneCount == 4)
        {
            _tutorialIndex = _tutorialUI1;
            StopCoroutine(_deleteTextCoroutine);
            ChangeText();
        }
        if(scene.name == "Shop1" && !_shopSeen)
        {
            _tutorialIndex = _tutorialShop;
            //StartCoroutine(DeleteText(0f));
            StopCoroutine(_deleteTextCoroutine);
            _shopSeen = true;
            ChangeText();
            _sceneCount = 0;
        }
        if(scene.name == "LakeSmall" && _shopSeen && _sceneCount == 1)
        {
            _tutorialIndex = _tutorialCommands1;
            StopCoroutine(_deleteTextCoroutine);
            //StartCoroutine(DeleteText(0f));
            ChangeText();
        }

    }

    private void EndTutorial()
    {
        StreamWriter writer = new StreamWriter(_tutorialPath, false);
        writer.Write("tutorial, 0\n");
        writer.Close();
    }




}
