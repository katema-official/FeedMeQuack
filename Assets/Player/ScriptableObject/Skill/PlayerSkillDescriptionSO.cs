using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Player
{
    public class PlayerSkillDescriptionSO : ScriptableObject
    {
        public PlayerSkillTypeSO Type;
        public bool EnabledByDefault = false; 
    }
}
