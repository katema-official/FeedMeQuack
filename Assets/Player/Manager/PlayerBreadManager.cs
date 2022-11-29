using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class PlayerBreadManager : MonoBehaviour
    {
        [SerializeField] private GameObject BreadToThrow;

        public void ThrowBread(int breadPoints, Vector3 startPos, Vector3 endPos)
        {
            GameObject newBread = Instantiate(BreadToThrow);
            newBread.GetComponent<BreadNamespace.BreadThrownComponent>().InitializeBreadThrownFromDuck(
                LevelStageNamespace.EnumsDungeon.BreadType.Medium,
                startPos.x,startPos.y,
                 endPos.x, endPos.y,10,30);
        }
    }
}
