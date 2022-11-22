using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class EatingManager : MonoBehaviour
{
    [SerializeField] private EnemyFSM _fsmScript;

    public int BreadPointsInMouth=0;

    private float _chewingRate;

    private Coroutine _eatingCoroutine;

    public Bread BreadInMouth;
    // Start is called before the first frame update
    void Start(){
        _fsmScript = gameObject.GetComponentInParent<EnemyFSM>();
        Species species= _fsmScript.MySpecies;
        _chewingRate = species.chewingRate;
    }

    public void StartEatingBread(int breadPoints){
        BreadPointsInMouth = breadPoints;
        _eatingCoroutine = StartCoroutine(EatingCoroutine());
    }

    public void StartEatingBread(Bread bread){
        BreadInMouth = bread;
        BreadPointsInMouth = bread.BreadPoints;
        _eatingCoroutine = StartCoroutine(EatingCoroutine());
    }

    private IEnumerator EatingCoroutine(){
        while (BreadPointsInMouth>0){
            yield return new WaitForSeconds(_chewingRate);
            BreadPointsInMouth--;
            if (BreadInMouth != null) BreadInMouth.BreadPoints = BreadPointsInMouth;
        }
        FinishEating();
        yield return null;
    }

    private void FinishEating(){
        BreadInMouth = null;
        BreadPointsInMouth = 0;
        _fsmScript.ChangeState(EnemyFSM.ActionState.Chilling);
    }

    public bool IsEating(){
        return BreadPointsInMouth > 0;
    }
}
