using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialComponent : MonoBehaviour
{
    private int _sceneCount = 0;

    private const int _tutorialMovement = 0;
    private const int _tutorialEat = 1;
    private const int _tutorialUI1 = 2;
    private const int _tutorialUI2 = 3;
    private const int _tutorialA = 4;

    private Dictionary<int, string> _tutorialText = new Dictionary<int, string>()
    {
        {_tutorialMovement, "Use WASD or the left analog stick to move. You can move from a lake to another through a river." },
        {_tutorialEat, "In a new lake, humans will throw bread. Bite it with the E key or A button when close enough." },
        {_tutorialUI1, "When you eat some bread you acquire Bread Points (BP), shown in the upper left corner of the screen" },
        {_tutorialUI2, "The minimum number of BP required to access the next stage are shown in the bottom right corner" },
        {_tutorialA, "Amogus" }
    };


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
            Destroy(this);
            Destroy(this.gameObject);
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
                StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialUI1;
                break;

            case _tutorialUI1:
                textToShow = _tutorialText[_tutorialUI1];
                StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialUI2;
                break;

            case _tutorialUI2:
                textToShow = _tutorialText[_tutorialUI2];
                StartCoroutine(DeleteText());
                _tutorialIndex = _tutorialA;
                break;
            default:
                break;


        }
        _text.text = textToShow;
        

        
        
    }


    private float _deleteTimeText = 6f;
    private IEnumerator DeleteText()
    {
        yield return new WaitForSeconds(_deleteTimeText);
        _text.text = "";

        switch (_tutorialIndex)
        {
            case _tutorialUI1:
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
        Debug.Log("SCENE COUNT = " + _sceneCount);
        if (scene.name == "LakeSmall" && _sceneCount == 3)
        {
            StopCoroutine(_deleteTextCoroutine);
            ChangeText();
        }
    }

    private void EndTutorial()
    {
        StreamWriter writer = new StreamWriter(_tutorialPath, false);
        writer.Write("tutorial, 0");
    }




}
