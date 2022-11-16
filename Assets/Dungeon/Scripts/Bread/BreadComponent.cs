using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{

    public class BreadComponent : MonoBehaviour
    {

        [SerializeField] private BreadSO _smallBreadSO;
        [SerializeField] private BreadSO _mediumBreadSO;
        [SerializeField] private BreadSO _largeBreadSO;


        public LevelStageNamespace.EnumsDungeon.BreadType dimension;
        public int BreadPoints;




        public void InitializedBread(LevelStageNamespace.EnumsDungeon.BreadType breadDimension)
        {
            dimension = breadDimension;
            switch (dimension)
            {
                case LevelStageNamespace.EnumsDungeon.BreadType.Small:
                    BreadPoints = Random.Range(_smallBreadSO.MinAmountOfBreadPoints, _smallBreadSO.MaxAmountOfBreadPoints + 1);
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Medium:
                    BreadPoints = Random.Range(_mediumBreadSO.MinAmountOfBreadPoints, _mediumBreadSO.MaxAmountOfBreadPoints + 1);
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Large:
                    BreadPoints = Random.Range(_largeBreadSO.MinAmountOfBreadPoints, _largeBreadSO.MaxAmountOfBreadPoints + 1);
                    break;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}