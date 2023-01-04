using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DuckFootController : MonoBehaviour
{
    [SerializeField] private GameObject duckFoot;
    //private TextMeshPro _textMeshPro;
    
    // Start is called before the first frame update
    private void Awake()
    {
        //_textMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void EnableDuckFoot()
    {
        duckFoot.SetActive(!duckFoot.activeInHierarchy);
        //_textMeshPro.faceColor = new Color(255, 169, 106);
    }
}
