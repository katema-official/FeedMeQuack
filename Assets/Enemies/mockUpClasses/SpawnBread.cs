using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpawnBread : MonoBehaviour
{
    [SerializeField] private GameObject breadPrefab;

    public int CD, qty;
    // Start is called before the first frame update
    void Start(){
        //StartCoroutine(WaitForSeconds(2));
        StartCoroutine(SpawnBreadCoroutine());
    }

    private void SpawnSingleBread(){
        new WaitForSeconds(2);
        Vector3 pos=new Vector3(100, 50);
        Instantiate(breadPrefab, pos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForSeconds(int sec){
        yield return new WaitForSeconds(sec);
        SpawnSingleBread();
        yield return null;
    }

    IEnumerator SpawnBreadCoroutine(){
        for (int i = 0; i < qty; i++){
            GenerateBread();
            yield return new WaitForSeconds(CD);
        }
    }

    private void GenerateBread(){
        Vector3 pos=new Vector3(Random.Range(0, 100), Random.Range(-100, 100));
        Instantiate(breadPrefab, pos, Quaternion.identity);
    }
}
