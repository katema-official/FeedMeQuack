using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlackComponent : MonoBehaviour
{

    private SpriteRenderer _sprite;
    [SerializeField] private float _timeBetweenAlphas;
    [SerializeField] private float _alphaAmountPerUnitOfTime;

    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;
        Debug.Log("AA " + _sprite.color.a);
    }



    public void fadeToBlack()
    {
        _sprite = GetComponent<SpriteRenderer>();
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;
        Debug.Log("fadeToBlack called");
        StartCoroutine(fadeToBlackCoroutine());
    }

    public void fadeFromBlack()
    {
        var color = _sprite.color;
        color.a = 1f;
        _sprite.color = color;
    }



    private IEnumerator fadeToBlackCoroutine()
    {
        Color c;
        while(_sprite.color.a < 1)
        {
            c = _sprite.color;
            c.a += _alphaAmountPerUnitOfTime;
            _sprite.color = c;
            Debug.Log("alpha = " + c.a);
            yield return new WaitForSeconds(_timeBetweenAlphas);
        }
        Debug.Log("Done");

        yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }


}
