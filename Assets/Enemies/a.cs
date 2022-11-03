using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class a : MonoBehaviour
{
    public int Numero;

    public int Numero1
    {
        get => Numero;
        set => Numero = value;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("a");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
