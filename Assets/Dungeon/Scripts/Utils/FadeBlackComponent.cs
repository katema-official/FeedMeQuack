using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlackComponent : MonoBehaviour
{

    private SpriteRenderer _sprite;

    [SerializeField] private float _fadeOutDuration = 0.3f;

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



    public void fadeToBlackAndGoToLake()
    {
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;
        StartCoroutine(fadeToBlackAndGoToLakeCoroutine());
    }

    public void fadeFromBlack()
    {
        var color = _sprite.color;
        color.a = 1f;
        _sprite.color = color;
        StartCoroutine(fadeFromBlackCoroutine());
    }


    public void fadeToBlackAndGoToShop()
    {
        var color = _sprite.color;
        color.a = 0f;
        _sprite.color = color;
        StartCoroutine(fadeToBlackAndGoToShopCoroutine());
    }





    private IEnumerator fadeToBlackAndGoToLakeCoroutine()
    {
        Color c;

        for (float i = 0; i < _fadeOutDuration; i += Time.deltaTime)
        {
            float normalizedTime = i / _fadeOutDuration;
            c = _sprite.color;
            c.a = Mathf.Lerp(0f, 1f, normalizedTime);
            _sprite.color = c;
            yield return null;
        }
        _levelStageManager.EnterLake();

        yield return null;
    }

    private IEnumerator fadeFromBlackCoroutine()
    {
        Color c;

        for (float i = 0; i < _fadeOutDuration; i += Time.deltaTime)
        {
            float normalizedTime = i / _fadeOutDuration;
            c = _sprite.color;
            c.a = Mathf.Lerp(1f, 0f, normalizedTime);
            _sprite.color = c;
            yield return null;
        }
        yield return null;
    }


    private IEnumerator fadeToBlackAndGoToShopCoroutine()
    {
        Color c;

        for (float i = 0; i < _fadeOutDuration; i += Time.deltaTime)
        {
            float normalizedTime = i / _fadeOutDuration;
            c = _sprite.color;
            c.a = Mathf.Lerp(0f, 1f, normalizedTime);
            _sprite.color = c;
            yield return null;
        }
        _levelStageManager.EnterShop();

        yield return null;
    }


}
