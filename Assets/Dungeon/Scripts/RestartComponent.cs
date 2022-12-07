using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DISTRUGGI IL GIOCATORE E IL LEVEL STAGE MANAGER
        Destroy(GameObject.Find("LevelStageManagerObject"));
        Destroy(GameObject.Find("DuckPlayer"));
        Destroy(GameObject.Find("DuckTypeManager"));
        Music.UniversalAudio.PlaySound("GameOver", transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("StartRunLoading");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
