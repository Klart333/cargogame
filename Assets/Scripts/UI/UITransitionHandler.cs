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

    public void Transition()
    {
        animator.SetTrigger("Transition");
    }
}
