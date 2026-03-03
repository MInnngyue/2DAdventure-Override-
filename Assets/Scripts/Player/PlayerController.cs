using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputController inputControl;//Unity新输入系统
     
    [Header("组件引用")]
    public Rigidbody2D rb;//物理组件
    public PhysicsCheck physicsCheck;
    public Vector2 inputDirection;//二维向量

    [Header("基础属性")]
    public float speed;
    public float jumpForce;

    private void Awake()
    {
        // 实例化输入控制类（相当于加载输入配置文件）
       inputControl = new PlayerInputController(); 
       // 获取组件,否则无法使用方法
       rb = GetComponent<Rigidbody2D>();
       physicsCheck = GetComponent<PhysicsCheck>();
       //事件注册
       inputControl.Player.Jump.started += Jump;
    }
    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();

    }
    private void FixedUpdate()
    {
        // 在FixedUpdate中处理移动，避免物理计算与渲染帧率不同步
        Move();
    }
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        // 检测移动方向并翻转角色朝向
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;
        // 应用新的缩放值实现角色翻转
        transform.localScale = new Vector3(faceDir, 1, 1);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        // 使用Impulse模式施加瞬时力，实现跳跃效果
        if(physicsCheck.isGround == true)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
