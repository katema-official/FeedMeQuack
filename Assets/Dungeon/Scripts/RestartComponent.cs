using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Music.UniversalAudio.PlaySound("GameOver", transform);
        //DISTRUGGI IL GIOCATORE E IL LEVEL STAGE MANAGER
        Destroy(GameObject.Find("LevelStageManagerObject"));
        Destroy(GameObject.Find("DuckPlayer"));
        Destroy(GameObject.Find("DuckTypeManager"));
        Destroy(GameObject.Find("HUD"));
        Destroy(GameObject.Find("Minimap"));
        Destroy(GameObject.Find("LevelIntroUI"));
        if (GameObject.Find("Tutorial")) Destroy(GameObject.Find("Tutorial"));

    }
}
