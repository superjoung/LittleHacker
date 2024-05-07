using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    public int num;
    public int doorNum;
    public string oper;

    public bool boxTrigger = false;
    public bool boxStop = false;
    public Vector2 boxMoveDir = new Vector2(0, 0);
    private float boxMoveSpeed = 5;

    public void Update()
    {
        if (boxTrigger)
        {
            boxMove();
        }
    }

    public void boxMove()
    {
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, boxMoveDir, 0.6f, LayerMask.GetMask("Wall"));

        if (hitWall)
        {
            // 박스가 벽과 부딪쳤을 경우
            boxStop = true;
            boxTrigger = false;
            transform.position = new Vector2(hitWall.transform.position.x - boxMoveDir.x, hitWall.transform.position.y - boxMoveDir.y);
        }

        else
        {
            transform.Translate(boxMoveDir * boxMoveSpeed * Time.deltaTime);
        }
    }
}
