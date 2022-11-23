using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Enemies
{
    public class EatingManager : MonoBehaviour
    {
        [SerializeField] private EnemyFSM _fsmScript;

        //public int BreadPointsInMouth = 0;

        private float _chewingRate;

        private Coroutine _eatingCoroutine;

        public BreadNamespace.BreadInMouthComponent BreadInMouth;

        // Start is called before the first frame update
        void Start(){
            _fsmScript = gameObject.GetComponentInParent<EnemyFSM>();
            Species species = _fsmScript.MySpecies;
            _chewingRate = species.chewingRate;
        }

        public void StartEatingBread(BreadNamespace.BreadInMouthComponent breadInMouth)
        {
            BreadInMouth = breadInMouth;
            _eatingCoroutine = StartCoroutine(EatingCoroutine());
        }

        /*public void StartEatingBread(int breadPoints){
            BreadPointsInMouth = breadPoints;
            _eatingCoroutine = StartCoroutine(EatingCoroutine());
        }

        public void StartEatingBread(Bread bread){
            BreadInMouth = bread;
            BreadPointsInMouth = bread.BreadPoints;
            _eatingCoroutine = StartCoroutine(EatingCoroutine());
        }*/

        private IEnumerator EatingCoroutine(){
            int eatenAmount;
            bool eatenCompletely = false;
            while (eatenCompletely == false && BreadInMouth!=null){
                yield return new WaitForSeconds(_chewingRate);
                (eatenAmount, eatenCompletely) = BreadInMouth.SubtractBreadPoints(1);
                //TODO: caso in cui il giocatore ruba il mio pane? Stoppare la coroutine e passare nello stato stealingPassive
            }

            FinishEating();
            yield return null;
        }

        private void FinishEating(){
            BreadInMouth = null;
            //BreadPointsInMouth = 0;
            _fsmScript.ChangeState(EnemyFSM.ActionState.Chilling);
        }

        public bool IsEating(){
            return BreadInMouth != null;
        }
    }
}
