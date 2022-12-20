using System.Collections;
using System.Collections.Generic;
using HUD;
using UnityEngine;

public class Mokcup : MonoBehaviour
{
    public int Nord, Sud, Est, Ovest;
    public MapManager MapManager;
    public MapManager.CardinalDirection dir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TestMove(){
        MapManager.UpdateMinimapAfterRiver(dir, Nord, Sud, Est, Ovest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
