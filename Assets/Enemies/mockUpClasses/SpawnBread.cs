using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnBread : MonoBehaviour
{
    [SerializeField] private GameObject breadPrefab;

    public float CD;
    public int qty;
    // Start is called before the first frame update
    void Start(){
        //StartCoroutine(WaitForSeconds(2));
        StartCoroutine(SpawnBreadCoroutine());
    }

    private void SpawnSingleBread(){
        new WaitForSeconds(2);
        Vector3 pos=new Vector3(100, 50);
    }

    IEnumerator WaitForSeconds(int sec){
        yield return new WaitForSeconds(sec);
        SpawnSingleBread();
        yield return null;
    }

    IEnumerator SpawnBreadCoroutine(){
        for (int i = 0; i < qty; i++){
            GenerateBread(i);
            yield return new WaitForSeconds(CD);
        }
    }

    private void GenerateBread(int i){
        Vector3 pos=new Vector3(Random.Range(-150, 150), Random.Range(-150, 150));
        GameObject newBread= Instantiate(breadPrefab, pos, Quaternion.identity);
        String name = "Bread" + i;
        newBread.name = name;
    }
}
