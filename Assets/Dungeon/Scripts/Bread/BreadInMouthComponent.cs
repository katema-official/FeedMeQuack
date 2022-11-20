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




        public void Initialize(int breadPoints, bool isLastPiece)
        {
            _breadPoints = breadPoints;
            _isLastPiece = isLastPiece;
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
                Destroy(this.gameObject);
                return (_breadPoints, true);
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