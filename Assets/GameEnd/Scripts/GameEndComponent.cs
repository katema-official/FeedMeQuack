using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Music.UniversalAudio.PlaySound("GameEnd", transform);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = Vector3.zero;
    }

}
