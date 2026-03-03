using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("组件引用")]
    public PhysicsCheck physicsCheck;
    public Animator anim;
    public Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }
    void Update()
    {
        SetAnimation();
    }
    public void SetAnimation()
    {
        // 将角色的水平速度传递给动画状态机
        // Mathf.Abs取绝对值
        anim.SetFloat("Velocity.x", Math.Abs(rb.velocity.x));
        anim.SetFloat("Velocity.y", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        // anim.SetBool("isDeath", )
    }
    public void Hurt()
    {
        anim.SetTrigger("isHurt");
    }
}
