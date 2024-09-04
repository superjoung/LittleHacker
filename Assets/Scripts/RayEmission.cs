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
            RaycastHit2D hitWall = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, layerMask); // 벽
            RaycastHit2D hitDoor = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, LayerMask.GetMask("Door")); // 문
            RaycastHit2D hitTrigger = Physics2D.Raycast(currentPos.position, moveDir, 0.6f, LayerMask.GetMask("Trigger")); // 박스

            if (hitTrigger) return hitTrigger;
            if (hitDoor) return hitDoor;
            if (hitWall) return hitWall;
            else Debug.LogError("Ray에 해당하는 오브젝트가 존재하지않습니다.(CheckWall)");

            return hitWall;
        }

        public static RaycastHit2D CheckItem(Transform currentPos, Vector2 moveDir)
        {
            int layerMask = (1 << LayerMask.NameToLayer("Item")) + (1 << LayerMask.NameToLayer("Door"));
            RaycastHit2D hitItem = Physics2D.Raycast(currentPos.position, moveDir, 0.3f, layerMask);

            if (hitItem) return hitItem;
            else Debug.LogError("Ray에 해당하는 오브젝트가 존재하지않습니다.(CheckItem)");

            return hitItem;
        }
    }
}
