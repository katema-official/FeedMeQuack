using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextFadeInOut : MonoBehaviour
{
    private TextMeshProUGUI _text = null;
    // can ignore the update, it's just to make the coroutines get called for example

    public void StartAnim(string str)
    {
        GetComponent<TextMeshProUGUI>().text = str;
        StartCoroutine(FadeInOut(1f, GetComponent<TextMeshProUGUI>()));
    }

     private IEnumerator FadeInOut(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }

        yield return new WaitForSeconds(3);

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);

        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
    void Start()
    {
    }


    void Update()
    {
    }

   
}