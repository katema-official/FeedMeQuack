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


        public LevelStageNamespace.EnumsDungeon.BreadType Dimension;
        public int BreadPoints;
        public bool IsBeingEaten = false;
        private GameObject _entityEatingThisBread;
        private Transform _positionEntityEatingThisBread;



        //function used to correctly initialize the bread thrown. Must be called as soon as a bread has been generated.
        public void InitializedBread(LevelStageNamespace.EnumsDungeon.BreadType breadDimension)
        {
            Dimension = breadDimension;
            switch (Dimension)
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




        //function used to notify to this piece of bread that someone started to eat it. The argument tells who started to eat it
        public void StartedToBeEaten(GameObject entityEatingThisBread)
        {
            //for the moment I simply make its alpha disappear
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0;
            GetComponent<SpriteRenderer>().color = c;

            _entityEatingThisBread = entityEatingThisBread;
            _positionEntityEatingThisBread = _entityEatingThisBread.transform;

            IsBeingEaten = true;
        }

        //function used to subtract BreadPoints (usually one, could be more if needed) from this piece of bread.
        //returns:
        //-as first value of the tuple, a bool that signal that the bread has been eaten completely (true) or not (false)
        //-the number of bread points subtracted
        public (bool, int) SubtractBreadPoints(int nOfBreadPoints)
        {
            int eaten = 0;
            bool destroyed = false;
            if (BreadPoints >= nOfBreadPoints)
            {
                BreadPoints -= nOfBreadPoints;
                eaten = nOfBreadPoints;
            }
            else
            {
                eaten = BreadPoints;
                BreadPoints = 0;
            }

            if(BreadPoints == 0)
            {
                destroyed = true;
                GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeDescriptionComponent>().NotifyBreadEaten();
                Destroy(this.gameObject);
            }

            return (destroyed, eaten);
        }


        private void Update()
        {
            //I use this code so that the bread always follows the entity eating it. Another thing we could do is to equip every entity that can eat bread with
            //a child object called "Mouth", that has its position always on the mouth of the main entity, so that the bread is always in that position.
            if (IsBeingEaten)
            {
                transform.position = _positionEntityEatingThisBread.position;
            }
        }

    }
}