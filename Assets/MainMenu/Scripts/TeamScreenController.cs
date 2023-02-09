using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeamScreenController : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetKeyDown(KeyCode.Insert))
        {
            SceneManager.LoadScene("PolimiLogo");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
        {
            var s = animator.GetCurrentAnimatorStateInfo(0);
            var time = s.normalizedTime;

            if (time > 1.0f)
            {
                SceneManager.LoadScene("PolimiLogo");
            }
        }


    }
}
