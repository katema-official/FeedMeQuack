using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    public class BreadInMouthComponent : MonoBehaviour
    {
        //we serialize this two just to see them in the editor
        [SerializeField] private int _breadPoints;
        [SerializeField] private bool _isLastPiece;    //true = when this BreadInMouth was generated, it destroyed the BreadInWater, so, once this has been
        //fully eaten, notify the LakeDescriptionComponent

        [SerializeField] private Sprite _breadSmallSprite;
        [SerializeField] private Sprite _breadMediumSprite;
        [SerializeField] private Sprite _breadLargeSprite;

        [SerializeField] private BreadSO _breadSmallSO;
        [SerializeField] private BreadSO _breadMediumSO;
        [SerializeField] private BreadSO _breadLargeSO;

        private LevelStageNamespace.EnumsDungeon.BreadType _dimension;

        private LevelStageNamespace.LakeDescriptionComponent _lakeDescriptionComponent;




        public void Initialize(int breadPoints, bool isLastPiece)
        {
            _breadPoints = breadPoints;
            _isLastPiece = isLastPiece;
            SetBreadDimension();
        }

        private void SetBreadDimension()
        {
            if (_breadPoints >= _breadSmallSO.MinBreadPoints && _breadPoints <= _breadSmallSO.MaxBreadPoints)
            {
                _dimension = LevelStageNamespace.EnumsDungeon.BreadType.Small;
                //GetComponent<SpriteRenderer>().sprite = _breadSmallSprite;
            }
            if (_breadPoints >= _breadMediumSO.MinBreadPoints && _breadPoints <= _breadMediumSO.MaxBreadPoints)
            {
                _dimension = LevelStageNamespace.EnumsDungeon.BreadType.Medium;
                //GetComponent<SpriteRenderer>().sprite = _breadMediumSprite;
            }
            if (_breadPoints >= _breadLargeSO.MinBreadPoints && _breadPoints <= _breadLargeSO.MaxBreadPoints)
            {
                _dimension = LevelStageNamespace.EnumsDungeon.BreadType.Large;
                //GetComponent<SpriteRenderer>().sprite = _breadLargeSprite;
            }
        }

        //returns the number of bread points eaten from this piece of bread + true when this bread is destroyed, false otherwise
        public (int, bool) SubtractBreadPoints(int bp)
        {
            if(_breadPoints > bp)
            {
                _breadPoints -= bp;
                SetBreadDimension();
                return (bp, false);
            }
            else
            {
                if (_isLastPiece)
                {
                    _lakeDescriptionComponent.NotifyBreadEaten();
                }
                Destroy(this.gameObject);
                return (_breadPoints, true);
            }
        }





        public void Move(Vector3 point)
        {
            transform.position = point;
        }





        public int GetBreadPoints()
        {
            return _breadPoints;
        }

        public bool GetIsLastPiece()
        {
            return _isLastPiece;
        }

        public LevelStageNamespace.EnumsDungeon.BreadType GetDimension()
        {
            return _dimension;
        }



        // Start is called before the first frame update
        void Start()
        {
            _lakeDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeDescriptionComponent>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}