using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    public class BreadInMouthComponent : MonoBehaviour
    {

        private int _breadPoints;
        private bool _isLastPiece;    //true = when this BreadInMouth was generated, it destroyed the BreadInWater, so, once this has been
        //fully eaten, notify the LakeDescriptionComponent

        [SerializeField] private Sprite _breadSmallSprite;
        [SerializeField] private Sprite _breadMediumSprite;
        [SerializeField] private Sprite _breadLargeSprite;

        [SerializeField] private BreadSO _breadSmallSO;
        [SerializeField] private BreadSO _breadMediumSO;
        [SerializeField] private BreadSO _breadLargeSO;

        private LevelStageNamespace.EnumsDungeon.BreadType _dimension;

        private LevelStageNamespace.LakeDescriptionComponent _lakeDescriptionComponent;




        public void Initialize(int breadPoints, bool isLastPiece, LevelStageNamespace.EnumsDungeon.BreadType dimension)
        {
            _breadPoints = breadPoints;
            _isLastPiece = isLastPiece;
            _dimension = dimension;
        }

        //returns the number of bread points eaten from this piece of bread + true when this bread is destroyed, false otherwise
        public (int, bool) SubtractBreadPoints(int bp)
        {
            if(_breadPoints > bp)
            {
                _breadPoints -= bp;
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