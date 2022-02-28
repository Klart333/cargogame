using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionHandler : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SmolTransition()
    {
        animator.SetTrigger("Transition");
    }

    public void SceneTransition()
    {
        animator.SetTrigger("SceneTransition");
    }

    public void InSceneTransition()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.SetTrigger("InScene");
    }
}
