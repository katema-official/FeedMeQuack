using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUpsNamespace
{

    [CreateAssetMenu(fileName = "New PowerUp", menuName = "PowerUpSO")]
    public class PowerUpSO : ScriptableObject
    {
        public Sprite Sprite;
        public string Name;
        public string Description;
        public int Cost;

        //TODO: change with ivan enum
        public List<PowerUpComponent.PlayerSkillAttribute> PowerUpKinds;
        public List<float> amountForPowerUpKind;



    }

}