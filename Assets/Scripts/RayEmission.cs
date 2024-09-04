using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleRay
{
    public class RayEmission
    {
        public static RaycastHit2D CheckWall(Transform currentPos, Vector2 moveDir)
        {
            int layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Item"));
            RaycastHit2D hitWall = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, layerMask); // ��
            RaycastHit2D hitDoor = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, LayerMask.GetMask("Door")); // ��
            RaycastHit2D hitTrigger = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, LayerMask.GetMask("Trigger")); // �ڽ�

            if (hitTrigger) return hitTrigger;
            if (hitDoor) return hitDoor;
            if (hitWall) return hitWall;
            else Debug.LogError("Ray�� �ش��ϴ� ������Ʈ�� ���������ʽ��ϴ�.(CheckWall)");

            return hitWall;
        }

        public static RaycastHit2D CheckItem(Transform currentPos, Vector2 moveDir)
        {
            int layerMask = (1 << LayerMask.NameToLayer("Item")) + (1 << LayerMask.NameToLayer("Door"));
            RaycastHit2D hitItem = Physics2D.Raycast(currentPos.position, moveDir, 0.3f, layerMask);

            if (hitItem) return hitItem;
            else Debug.LogError("Ray�� �ش��ϴ� ������Ʈ�� ���������ʽ��ϴ�.(CheckItem)");

            return hitItem;
        }
    }
}
