using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlackComponent : MonoBehaviour
{

    private SpriteRenderer _sprite;
    [SerializeField] private float _timeBetweenAlphas;
    [SerializeField] private float _alphaAmountPerUnitOfTime;

    private LevelStageNamespace.LevelStageManagerComponent _levelStageManager;

    // Start is called before the first frame update
    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;

        _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageNamespace.LevelStageManagerComponent>();
    }



    public void fadeToBlack()
    {
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;
        StartCoroutine(fadeToBlackCoroutine());
    }

    public void fadeFromBlack()
    {
        var color = _sprite.color;
        color.a = 1f;
        _sprite.color = color;
        StartCoroutine(fadeFromBlackCoroutine());
    }



    private IEnumerator fadeToBlackCoroutine()
    {
        Color c;
        while(_sprite.color.a < 1)
        {
            c = _sprite.color;
            c.a += _alphaAmountPerUnitOfTime;
            _sprite.color = c;
            yield return new WaitForSeconds(_timeBetweenAlphas);
        }

        _levelStageManager.EnterLake();

        yield return null;
    }

    private IEnumerator fadeFromBlackCoroutine()
    {
        Color c;
        while (_sprite.color.a > 0)
        {
            c = _sprite.color;
            c.a -= _alphaAmountPerUnitOfTime;
            _sprite.color = c;
            yield return new WaitForSeconds(_timeBetweenAlphas);
        }

        yield return null;
    }




    // Update is called once per frame
    void Update()
    {
        
    }


}
