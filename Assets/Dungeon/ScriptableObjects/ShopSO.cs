using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUpsNamespace;

namespace LevelStageNamespace
{
    [CreateAssetMenu(fileName = "New Shop Description", menuName = "ShopSO")]
    public class ShopSO : ScriptableObject
    {
        [Header("List of PowerUps (SO) that\nthis shop can offer")]
        public List<PowerUpSO> ListOfPowerUps;

        public int MinAmountOfPowerUpsToSell;
        public int MaxAmountOfPowerUpsToSell;
    }
}