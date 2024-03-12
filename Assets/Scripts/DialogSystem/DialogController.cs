using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    private Animator animator;

    public List<AnimationClip> animations;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetFace(int index)
    {
        if (animator != null)
        {
            if (animations.Count > index)
            {
                animator.Play(animations[index].name);
            }
        }
    }
}
