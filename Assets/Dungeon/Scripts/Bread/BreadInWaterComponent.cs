using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    public class BreadInWaterComponent : MonoBehaviour
    {

        [SerializeField] private Sprite _breadSmallSprite;
        [SerializeField] private Sprite _breadMediumSprite;
        [SerializeField] private Sprite _breadLargeSprite;

        [SerializeField] private BreadSO _breadSmallSO;
        [SerializeField] private BreadSO _breadMediumSO;
        [SerializeField] private BreadSO _breadLargeSO;

        [SerializeField] private GameObject BreadInMouthPrefab;

        private int _breadPoints;

        private LevelStageNamespace.LakeDescriptionComponent _lakeDescriptionComponent;



        private LevelStageNamespace.EnumsDungeon.BreadType _dimension;






        public void InitializeBread(LevelStageNamespace.EnumsDungeon.BreadType dimension, int breadPoints = 0)
        {
            _dimension = dimension;
            switch (_dimension)
            {
                case LevelStageNamespace.EnumsDungeon.BreadType.Small:
                    GetComponent<SpriteRenderer>().sprite = _breadSmallSprite;
                    _breadPoints = (breadPoints == 0) ? Random.Range(_breadSmallSO.MinBreadPointsSpawn, _breadSmallSO.MaxBreadPointsSpawn) : breadPoints;
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Medium:
                    GetComponent<SpriteRenderer>().sprite = _breadMediumSprite;
                    _breadPoints = (breadPoints == 0) ? Random.Range(_breadMediumSO.MinBreadPointsSpawn, _breadMediumSO.MaxBreadPointsSpawn) : breadPoints;
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Large:
                    GetComponent<SpriteRenderer>().sprite = _breadLargeSprite;
                    _breadPoints = (breadPoints == 0) ? Random.Range(_breadLargeSO.MinBreadPointsSpawn, _breadLargeSO.MaxBreadPointsSpawn) : breadPoints;
                    break;
                default:
                    break;
            }

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Vector2 spriteSize = sprite.rect.size;                      //to remove
            float xPixelSprite = 100f;                                  //to remove
            float yPixelSprite = 100f;                                  //to remove
            float amountToDivideX = spriteSize.x / xPixelSprite;        //to remove
            float amountToDivideY = spriteSize.y / yPixelSprite;        //to remove
            transform.localScale = new Vector3(transform.localScale.x / amountToDivideX, transform.localScale.y / amountToDivideY, 0);      //to remove

            //if the piece of bread was spawned outside the lake, it must be destroyed
            if(_lakeDescriptionComponent.Contains(transform.position) == false)
            {
                _lakeDescriptionComponent.NotifyBreadEaten();
                StartCoroutine(FadeOutOutsideLake());
            }

        }


        //function used to pick a piece of this braed in water.
        //The effect is:
        //-The BreadPoints of this bread in water are decreased by the amount passed as argument
        //-The sprite is changed according to the new value of BreadPoints
        //-A new BreadInMouth is created and returned to whoever called this function
        //-If necessary, the BreadInMouth returned has the variable IsLastBread = true, if getting that BreadInMouth resulted indestroying this BreadInLake
        public GameObject GenerateNewBreadInMouth(int breadPointsToTake)
        {
            GameObject newBreadInMouth = Instantiate(BreadInMouthPrefab);
            BreadInMouthComponent breadInMouthComponent = newBreadInMouth.GetComponent<BreadInMouthComponent>();
            if(_breadPoints > breadPointsToTake)
            {
                Debug.Log("IS NOT LAST PIECE!");
                _breadPoints -= breadPointsToTake;
                SetBreadSprite();
                breadInMouthComponent.Initialize(breadPointsToTake, false);
            }
            else
            {
                Debug.Log("IS LAST PIECE!");
                breadInMouthComponent.Initialize(_breadPoints, true);
                _breadPoints = 0;
                Destroy(this.gameObject);   //Should be delayed after newBreadInMouth is returned, but it should be checked
            }


            return newBreadInMouth;
        }


        //method used to set the correct sprite depending on the current breadPoints
        private void SetBreadSprite()
        {
            if(_breadPoints >= _breadSmallSO.MinBreadPoints && _breadPoints <= _breadSmallSO.MaxBreadPoints)
            {
                GetComponent<SpriteRenderer>().sprite = _breadSmallSprite;
            }
            if (_breadPoints >= _breadMediumSO.MinBreadPoints && _breadPoints <= _breadMediumSO.MaxBreadPoints)
            {
                GetComponent<SpriteRenderer>().sprite = _breadMediumSprite;
            }
            if (_breadPoints >= _breadLargeSO.MinBreadPoints && _breadPoints <= _breadLargeSO.MaxBreadPoints)
            {
                GetComponent<SpriteRenderer>().sprite = _breadLargeSprite;
            }
        }




        // Start is called before the first frame update
        void Awake()
        {
            _lakeDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeDescriptionComponent>();
        }

        // Update is called once per frame
        void Update()
        {

        }




        public Sprite GetBreadSmallSprite()
        {
            return _breadSmallSprite;
        }
        public Sprite GetBreadMediumSprite()
        {
            return _breadMediumSprite;
        }
        public Sprite GetBreadLargeSprite()
        {
            return _breadLargeSprite;
        }





        //################################################################################################################################################################
        //#################################################### FADE OUT FOR DESTROYING THE BREAD WHEN OUTSIDE THE LAKE ###################################################
        //################################################################################################################################################################

        IEnumerator FadeOutOutsideLake()
        {
            float duration = 0.5f;
            float normalizedTime;
            
            for(float t = 0f; t < duration; t += Time.deltaTime)
            {
                normalizedTime = t / duration;
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = Mathf.Lerp(1f, 0f, normalizedTime);
                GetComponent<SpriteRenderer>().color = c;
                yield return null;
            }
            Destroy(this.gameObject);
            yield return null;
        }



    }
}