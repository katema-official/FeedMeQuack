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
    private const int _tutorialEat1 = 1;
    private const int _tutorialEat2 = 2;
    private const int _tutorialUI1 = 3;
    private const int _tutorialUI2 = 4;
    private const int _tutorialShop = 5;
    private const int _tutorialCommands1 = 6;
    private const int _tutorialAAA = 7;

    private Dictionary<int, string> _tutorialText = new Dictionary<int, string>()
    {
        {_tutorialMovement, "Use WASD/arrow keys or the left analog stick to move. You can move from a lake to another through a " + ColorString("river.", "0045B7") },
        {_tutorialEat1, "Bite the thrown " + ColorString("bread", "CD6E3B") + " with the " + ColorString("E key", "494D42") + "/" + ColorString("Left Mouse Button", "494D42") + " or the " + ColorString("A", "00FD10") + " button when close enough." },
        {_tutorialEat2, "Generally, you " + ColorString("can't eat", "FF0000") + " a " + ColorString("whole bread", "CD6E3B") + ". So, you will " + ColorString("just bite a piece", "FF0000") + " of it, that is, you will start " + ColorString("chewing some", "FF0000") + " of its " + ColorString("BP (Bread Points)", "CD6E3B")},
        {_tutorialUI1, "Your " + ColorString("stats", "00FF00") + " influence how many " + ColorString("BP", "CD6E3B") + " you can take in one bite, and how fast you chew them. Take a look at your stats in the " + ColorString("lower left corner", "D7C36A") + ". Your " + ColorString("BP", "CD6E3B") +", instead, are shown in the " + ColorString("upper left corner", "D7C36A") + ". Get enough of them to go to the next level!"},
        {_tutorialUI2, "Use the minimap in the " + ColorString("upper right corner", "D7C36A") + " to orient yourself. The " + ColorString("yellow square", "FFFF00") + " is the lake you are currently in. The " + ColorString("green square", "00FF00") + " is the lake containing a passage to the next level: just follow the " + ColorString("brown sign", "A18534") + " on its shore..."},
        {_tutorialShop, "In the " + ColorString("shop", "FF4301") + ", your " + ColorString("BP", "CD6E3B") + " in excess are converted in " + ColorString("DBP (Digested Bread Points)", "EAD200") + ", that you can use to purchase " + ColorString("Power Ups", "EAD200") + "."},
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
        StreamReader reader = new StreamReader(Application.dataPath + "/" + "tutorial.txt");
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
                _tutorialIndex = _tutorialEat1;
                break;

            case _tutorialEat1:
                textToShow = _tutorialText[_tutorialEat1];
                _deleteTextCoroutine = StartCoroutine(DeleteText(8f));
                _tutorialIndex = _tutorialEat2;
                break;

            case _tutorialEat2:
                textToShow = _tutorialText[_tutorialEat2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(18f));
                _tutorialIndex = _tutorialUI1;
                break;

            case _tutorialUI1:
                textToShow = _tutorialText[_tutorialUI1];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialUI2;
                break;

            case _tutorialUI2:
                textToShow = _tutorialText[_tutorialUI2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(14f));
                _tutorialIndex = _tutorialShop;
                break;

            case _tutorialShop:
                textToShow = _tutorialText[_tutorialShop];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands1;
                break;

            case _tutorialCommands1:
                textToShow = _tutorialText[_tutorialCommands1];
                StartCoroutine(DeleteText(26f));
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
            case _tutorialEat2:
                ChangeText();
                break;

            case _tutorialUI2:
                ChangeText();
                break;

        }
        yield return null;
    }








    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _sceneCount++;
        if (scene.name == "LakeSmall" && _sceneCount == 3 && !_shopSeen)
        {
            _tutorialIndex = _tutorialEat1;
            StopCoroutine(_deleteTextCoroutine);
            //StartCoroutine(DeleteText(0f));
            ChangeText();
        }
        if(scene.name == "LakeSmall" && _sceneCount == 4 && !_shopSeen)
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
        StreamWriter writer = new StreamWriter(Application.dataPath + "/" + "tutorial.txt", false);
        writer.Write("tutorial, 0\n");
        writer.Close();
    }




}
