using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Duck/DuckDescription")]
public class PlayerDuckDescriptionSO : ScriptableObject
{
    public string Name = "";
    public string Description = "";
    public float  Speed = 0.0f;
    public float  EatingSpeed = 0.0f;
    public float  ChewingRate = 0.0f;
    public int    MouthSize = 0;
}
