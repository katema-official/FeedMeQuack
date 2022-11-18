using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Player
{ 
    [CreateAssetMenu(menuName = "Player/Description/Duck")]
    public class PlayerDuckDescriptionSO : ScriptableObject
    {
        public string Name = "";
        public string Description = "";
        public PlayerSkillDescriptionSO[] Skills;
    }
}
