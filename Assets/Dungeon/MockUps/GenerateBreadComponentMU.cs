using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBreadComponentMU : MonoBehaviour
{

    [SerializeField] private GameObject _bread;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < 1; i++)
            {
                Instantiate(_bread);
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            GameObject b = Instantiate(_bread);
            b.GetComponent<BreadNamespace.BreadThrownComponent>().InitializeBreadThrownFromDuck(null, 0, 0, 100, 100);
        }
    }
}
