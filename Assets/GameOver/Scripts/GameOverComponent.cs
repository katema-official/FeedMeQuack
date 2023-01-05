using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Music.UniversalAudio.PlayMusic("GameOver", true);
        //Music.UniversalAudio.PlaySound("GameEnd", transform);
    }

}
