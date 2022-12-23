using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDNamespace;

public class statChangermockup : MonoBehaviour
{
    public HUDManager HUDManager;
    // Start is called before the first frame update
    void Start()
    {
        HUDManager.ChangeGoalText(30);
        HUDManager.ChangeText(HUDManager.textFields.speed, 15);
        HUDManager.ChangeBreadPointsCollectedText(7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
