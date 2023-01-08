using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TutorialComponent : MonoBehaviour
{
    private int _sceneCount = 0;
    private bool _shopSeen = false;

    private const int _tutorialMovement = 0;
    private const int _tutorialEat1 = 1;
    private const int _tutorialEat2 = 2;
    private const int _tutorialEat3 = 3;
    private const int _tutorialEat4 = 4;
    private const int _tutorialEat5 = 5;
    private const int _tutorialUI1 = 6;
    private const int _tutorialUI2 = 7;
    private const int _tutorialUI3 = 8;
    private const int _tutorialShop = 9;
    private const int _tutorialShop2 = 10;
    private const int _tutorialShop3 = 11;
    private const int _tutorialCommands1 = 12;
    private const int _tutorialAAA = 13;
    private const int _tutorialCommands2 =14;
    private const int _tutorialCommands3 = 15;
    private const int _tutorialCommands4 = 16;


    private Dictionary<int, string> _tutorialText = new Dictionary<int, string>()
    {
        //intro/movement tutorial
        {_tutorialMovement,
            "Welcome to " + ColorString("Feed Me, Quack!", "ffa96a") + "\n"+
            "Use " + ColorString("WASD/Arrow", "FFFF00") + " keys or the left analog stick to move.\n"+
            "You can move from a lake to another through a " + ColorString("river.", "0045B7")  +"\n"+
            "Go to the right to enter in the next lake."},
      
        
        //eating tutorial
        {_tutorialEat1, "Bite the thrown " + ColorString("bread", "CD6E3B") + " with the " + ColorString("E key", "494D42") + "/" + ColorString("Left Mouse Button", "494D42") + " or the " + ColorString("A", "00FD10") + " button when close enough."},
        
        
        {_tutorialEat2,
            "Each piece of bread has a certain amount of Bread Points (BP).\n"+
            "When you start to eat a piece of bread, its BPs are shown at the top of the duck player, with a spoon/knife icon on its left.\n" +
            "Since you eat BPs, you'll see the number placed above your duck shrink more and more.\n"},
        {_tutorialEat3,
            "Generally, you " + ColorString("can't eat", "FF0000") + " a " + ColorString("whole bread", "CD6E3B") + ".\n"+
            "So, you will " + ColorString("just bite a piece", "FF0000") + " of it.\n"+
            "The rest of the bread will stay on the lake surface to be eaten at a later time."},

          {_tutorialEat4, 
            "Distant pieces of breads, outside your view, will be highlighted through directional arrows.\n" +
            "This useful markers will help you to locate and catch the various pieces of breads before the other ducks!\n"},


          {_tutorialEat5,
            "Your goal is to eat enough pieces of breads (and so their Bread Points) to win the run!\n"},




        //{_tutorialUI1, "Your " + ColorString("stats", "00FF00") + " influence how many " + ColorString("BP", "CD6E3B") + " you can take in one bite, and how fast you chew them.\nTake a look at your stats in the " + ColorString("lower left corner", "D7C36A") + ".\nYour " + ColorString("BP", "CD6E3B") +", instead, are shown in the " + ColorString("upper left corner", "D7C36A") + ".\nGet enough of them to go to the next level!"},
       
        
        //{_tutorialUI2, "Use the minimap in the " + ColorString("upper right corner", "D7C36A") + " to orient yourself.\nThe " + ColorString("yellow square", "FFFF00") + " is the lake you are currently in.\nThe " + ColorString("green square", "00FF00") + " is the lake containing a passage to the next level: just follow the " + ColorString("brown sign", "A18534") + " on its shore..."},
        
        
        
        {_tutorialUI1,
            "Check your statistics through the User Interface!\n"+
            "The eaten " + ColorString("BPs", "CD6E3B") +" are shown in the " + ColorString("upper left corner panel.", "D7C36A") +"\n"+
            "Beside the eaten BPs, you can find the goal of the current stage in terms of BPs, colored in orange.\n"},


        {_tutorialUI2, 
            "Check the status of your skills in the" + ColorString("lower left corner panel.", "D7C36A")+"\n"+
            "The skills are listed in this order from the top: Dash - Steal - Catch/Spit.\n"+
            "Press Tab or the View Button to open the Statistics View."},

          {_tutorialUI3,
            "Use the minimap in the " + ColorString("upper right corner", "D7C36A") + " to orient yourself.\n"+
            "The " + ColorString("yellow square", "FFFF00") + " is the lake you are currently in.\n"+
            "The " + ColorString("green square", "00FF00") + " is the lake containing a passage to the shop: just follow the " + ColorString("End Brown Sign ", "A18534") + " on its shore..."
            },


     //  {_tutorialShop, "In the " + ColorString("shop", "FF4301") + ", your " + ColorString("BP", "CD6E3B") + " in excess are converted in " + ColorString("DBP (Digested Bread Points)", "EAD200") + ", that you can use to purchase " + ColorString("Power Ups", "EAD200") + "."},
      
         {_tutorialShop, 
            "Welcome to the Shop!\n"+
                "In the " + ColorString("shop", "FF4301") + ", your " + ColorString("BPs", "CD6E3B") + " in excess are converted in " +
                ColorString("DBPs (Digested Bread Points)", "EAD200") + " shown in yellow in the upper left corner of the screen.\n" +
                "You can use these DBPs to purchase " + ColorString("Power Ups", "EAD200") + " displaced around this lake."
                },

          {_tutorialShop2,
                 "Press the E key or the A button to buy a power up.\n"+
                 "Check carefully its description and, most of all, its cost."
                },

         {_tutorialShop3,
                 "Once you have finished to buy power-ups,\n"+
                 "You can go to the next level moving towards the river on the right."
                },


        {_tutorialCommands1, "Eating is not your only skill.\nUse " + ColorString("Shift", "494D42") + "/" + ColorString("Mouse Wheel Button", "494D42") + " or the " + ColorString("B", "F80000") + " button to dash.\nPress again to stop.\n"},
        {_tutorialCommands2,"Use " + ColorString("Space", "494D42") + " or the " + ColorString("Y", "F87700") + " button to steal bread from an enemy.\nYour victim won't let it go that easily thought...\n" },
        {_tutorialCommands3,"Use " + ColorString("Q", "494D42") + "/" + ColorString("Right Mouse Button", "494D42") + " or " + ColorString("X", "0068FF") + " to grab a piece of bread.\nThen, keep it pressed to charge and spit it!\n"},
        {_tutorialCommands4,
            "All your skills have a cooldown.\n"+
            "Check them on the Statistics View (the one in the lower left corner)."
        }
    };

    private static string ColorString(string s, string color)
    {
        return "<color=#" + color + ">" + s + "</color>";
    }





    private string _tutorialPath = "./Assets/Tutorial/tutorial.txt";
    private GameObject _camera;
    [SerializeField] private GameObject _tutorialRootCanvas;
    [SerializeField] private GameObject _pauseMenuCanvas;

    [SerializeField] private TMP_Text _text;

    private int _tutorialIndex = _tutorialMovement;

    private Coroutine _deleteTextCoroutine;



    public bool IsActive()
    {
        return _tutorialRootCanvas.active;
    }

    void Awake()
    {
        if (PlayerPrefs.HasKey("Tutorial"))
        {
            var tutorialEnabled = PlayerPrefs.GetInt("Tutorial");
            if (tutorialEnabled == 1) SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetInt("Tutorial", 1);
        //PlayerPrefs.Save();
        //read if the tutorial must be carried out or not
        if (PlayerPrefs.HasKey("Tutorial"))
        {
            var tutorialEnabled = PlayerPrefs.GetInt("Tutorial");
            if (tutorialEnabled == 0) return;
        }
        else
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            PlayerPrefs.Save();
        }


        //StreamReader reader = new StreamReader(Application.dataPath + "/" + "tutorial.txt");
        //string lineA = reader.ReadLine();
        //string[] splitA = lineA.Split(',');
        //int tutorial = int.Parse(splitA[1]);
        //if(tutorial == 0)
        //{
        //   // SceneManager.sceneLoaded -= OnSceneLoaded;
        //    //Destroy(gameObject);
        //    return;
        //}

        // _camera = GameObject.FindGameObjectWithTag("Player").transform.parent.Find("Camera").gameObject;
        if (PlayerPrefs.HasKey("Tutorial"))
        {
            var tutorialEnabled = PlayerPrefs.GetInt("Tutorial");
            if(tutorialEnabled == 1) StartCoroutine(StartTutorial());
        }
    }
   public void Enable()
    {
        PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
    }

    void Update()
    {
        var gamepad = Gamepad.current;

        if (Time.timeScale == 0 && (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame)))
        {
            if (FindObjectOfType<Music.PauseManager>().IsActive()) return;
            if (_deleteTextCoroutine != null) StopCoroutine(_deleteTextCoroutine);
            OtherTutorials();
        }
    }


    void OnDestroy()
    {
        Time.timeScale = 1;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private IEnumerator StartTutorial(float time = 3f)
    { 
        yield return new WaitForSecondsRealtime(time);
        yield return new WaitUntil(() => (!FindObjectOfType<Music.PauseManager>().IsActive()));

        ChangeText();
    }


    private void ChangeText()
    {
        string textToShow = "";
        switch (_tutorialIndex)
        {
            case _tutorialMovement:
                textToShow = _tutorialText[_tutorialMovement];
                _deleteTextCoroutine = StartCoroutine(DeleteText(18f));
                _tutorialIndex = _tutorialEat1;
                break;




            case _tutorialEat1:
                textToShow = _tutorialText[_tutorialEat1];
                _deleteTextCoroutine = StartCoroutine(DeleteText(6f));
                _tutorialIndex = _tutorialEat2;
                break;
            case _tutorialEat2:
                textToShow = _tutorialText[_tutorialEat2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(22f));
                _tutorialIndex = _tutorialEat3;
                break;
            case _tutorialEat3:
                textToShow = _tutorialText[_tutorialEat3];
                _deleteTextCoroutine = StartCoroutine(DeleteText(16f));
                _tutorialIndex = _tutorialEat4;
                break;
            case _tutorialEat4:
                textToShow = _tutorialText[_tutorialEat4];
                _deleteTextCoroutine = StartCoroutine(DeleteText(16f));
                _tutorialIndex = _tutorialEat5;
                break;
            case _tutorialEat5:
                textToShow = _tutorialText[_tutorialEat5];
                _deleteTextCoroutine = StartCoroutine(DeleteText(8f));
                _tutorialIndex = _tutorialUI1;
                break;




            case _tutorialUI1:
                textToShow = _tutorialText[_tutorialUI1];
                _deleteTextCoroutine = StartCoroutine(DeleteText(17f));
                _tutorialIndex = _tutorialUI2;
                break;

            case _tutorialUI2:
                textToShow = _tutorialText[_tutorialUI2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(18f));
                _tutorialIndex = _tutorialUI3;
                break;

            case _tutorialUI3:
                textToShow = _tutorialText[_tutorialUI3];
                _deleteTextCoroutine = StartCoroutine(DeleteText(18f));
                _tutorialIndex = _tutorialShop;
                break;





            case _tutorialShop:
                textToShow = _tutorialText[_tutorialShop];
                _deleteTextCoroutine = StartCoroutine(DeleteText(18f));
                _tutorialIndex = _tutorialShop2;
                break;
            case _tutorialShop2:
                textToShow = _tutorialText[_tutorialShop2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialShop3;
                break;
            case _tutorialShop3:
                textToShow = _tutorialText[_tutorialShop3];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands1;
                break;



            case _tutorialCommands1:
                textToShow = _tutorialText[_tutorialCommands1];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands2;
                break;
            case _tutorialCommands2:
                textToShow = _tutorialText[_tutorialCommands2];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands3;
                break;
            case _tutorialCommands3:
                textToShow = _tutorialText[_tutorialCommands3];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialCommands4;
                break;
            case _tutorialCommands4:
                textToShow = _tutorialText[_tutorialCommands4];
                _deleteTextCoroutine = StartCoroutine(DeleteText(12f));
                _tutorialIndex = _tutorialAAA;
                EndTutorial();
                break;

            case _tutorialAAA:

                break;

            default:
                break;


        }
        _text.text = textToShow;
         _tutorialRootCanvas.SetActive(true);
        Time.timeScale = 0;
    }


    //private float _deleteTimeText = 8f;
    private IEnumerator DeleteText(float time = 8f)
    {
        
        yield return new WaitForSecondsRealtime(time);
        yield return new WaitUntil(() => (!FindObjectOfType<Music.PauseManager>().IsActive()));

        OtherTutorials();
        yield return null;
    }



    private void OtherTutorials()
    {
        _text.text = "";
        _tutorialRootCanvas.SetActive(false);
        Time.timeScale = 1;

        switch (_tutorialIndex)
        {
            case _tutorialEat2:
                StartCoroutine(StartTutorial(5f));
                //  ChangeText();
                break;
            case _tutorialEat3:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;
            case _tutorialEat4:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;
            case _tutorialEat5:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;





            case _tutorialUI2:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;
            case _tutorialUI3:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;


            case _tutorialShop2:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;
            case _tutorialShop3:
                StartCoroutine(StartTutorial());
                //  ChangeText();
                break;




            case _tutorialCommands2:
                StartCoroutine(StartTutorial(6));
                break;

            case _tutorialCommands3:
                StartCoroutine(StartTutorial(6));
                break;

            case _tutorialCommands4:
                StartCoroutine(StartTutorial(6));
                break;
        }
    }




    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _sceneCount++;
        if (scene.name == "LakeSmall" && _sceneCount == 3 && !_shopSeen)
        {
            _tutorialIndex = _tutorialEat1;
            if (_deleteTextCoroutine != null) StopCoroutine(_deleteTextCoroutine);
            //StartCoroutine(DeleteText(0f));
            StartCoroutine(StartTutorial());
           // ChangeText();
        }
        if(scene.name == "LakeSmall" && _sceneCount == 4 && !_shopSeen)
        {
            _tutorialIndex = _tutorialUI1;
            if (_deleteTextCoroutine != null)  StopCoroutine(_deleteTextCoroutine);
            StartCoroutine(StartTutorial());
        }
        if(scene.name == "Shop1" && !_shopSeen)
        {
            _tutorialIndex = _tutorialShop;
            //StartCoroutine(DeleteText(0f));
            if (_deleteTextCoroutine != null) StopCoroutine(_deleteTextCoroutine);
            _shopSeen = true;
            StartCoroutine(StartTutorial());
          //  ChangeText();
            _sceneCount = 0;
        }
        if(_shopSeen && _sceneCount == 2)
        {
            //_tutorialIndex = _tutorialCommands1;
            StopCoroutine(_deleteTextCoroutine);
            //StartCoroutine(DeleteText(0f));
            StartCoroutine(StartTutorial());
          //  ChangeText();
        }

    }

    private void EndTutorial()
    {
        //StreamWriter writer = new StreamWriter(Application.dataPath + "/" + "tutorial.txt", false);
        //writer.Write("tutorial, 0\n");
        //writer.Close();
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.Save();
    }




}
