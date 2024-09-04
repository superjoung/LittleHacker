using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util
{
    public static class TouchUtil
    {
        public enum ETouchState { None, Began, Moved, Ended }; //����ϰ� ������ ��� �����ϰ� ��ġ & ���콺 ó��
                                                               // ����� or ������ ���콺 ��ġ��ǥ ó��
        public static void TouchSetUp(ref ETouchState touchState, ref Vector2 touchPosition)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = ETouchState.Began; }
            }
            else if (Input.GetMouseButton(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = ETouchState.Moved; } }
            else if (Input.GetMouseButtonUp(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = ETouchState.Ended; } }
            else touchState = ETouchState.None;
            touchPosition = Input.mousePosition;
#else
                if (Input.touchCount > 0)
                {

                    Touch touch = Input.GetTouch(0);
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true) return;
                    if (touch.phase == TouchPhase.Began) touchState = ETouchState.Began;
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) touchState = ETouchState.Moved;
                    else if (touch.phase == TouchPhase.Ended) if (touchState != ETouchState.None) touchState = ETouchState.Ended;
                    touchPosition = touch.position;
                }
                else touchState = ETouchState.None;
#endif
        }
    }
}
