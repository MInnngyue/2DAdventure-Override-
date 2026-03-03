using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public Vector2 bottomOffSet;
    public Vector2 leftOffSet;
    public Vector2 rightOffSet;
    public float checkRaduis;
    public LayerMask groundLayer;
    [Header("状态参数")]
    public bool isGround;
    public bool touchiLeftWall;
    public bool touchiRightWall;
    public void Update()
    {
        check();
    }
    public void check()
    {
        // 在指定位置进行圆形区域检测，结果存储在isGround中
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffSet, checkRaduis, groundLayer);
        touchiLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffSet, checkRaduis, groundLayer);
        touchiRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffSet, checkRaduis, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        // 设置绘制颜色为红色，使辅助图形更醒目
        Gizmos.color = Color.red;

        // 在检测位置绘制线框球体，直观显示检测范围
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffSet, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffSet, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffSet, checkRaduis);
    }
}
