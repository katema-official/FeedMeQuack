using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ResizeTilemapSize : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<Tilemap>().CompressBounds();
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
