using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum ETouchState { None, Begin, Move, End };
    public ETouchState playerTouch = ETouchState.None;

    private Vector2 touchPosition = new Vector2(0, 0);
    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 moveDir = new Vector2(0, 0);

    public float playerMoveSpeed;
    private bool moveStart = false;

    public void Start()
    {
        Debug.Log(playerTouch);
    }

    public void Update()
    {
        TouchSetup();
        if(moveStart == false)
        {
            PlayerMoveDIr();
        }
        PlayerMove();
    }


    void TouchSetup()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.Begin; } }
        else if (Input.GetMouseButton(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.Move; } }
        else if (Input.GetMouseButtonUp(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.End; } }
        else playerTouch = ETouchState.None;
        touchPosition = Input.mousePosition;
        //Debug.Log(playerTouch);
#else
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true) return;
            if (touch.phase == TouchPhase.Began) playerTouch = ETouchState.Begin;
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) playerTouch = ETouchState.Move;
            else if (touch.phase == TouchPhase.Ended) if (playerTouch != ETouchState.None) playerTouch = ETouchState.End;
            touchPosition = touch.position;
        }
        else playerTouch = ETouchState.None;
#endif
    }

    void PlayerMoveDIr()
    {
        if(playerTouch == ETouchState.Begin)
        {
            startPos = touchPosition;
        }

        else if(playerTouch == ETouchState.Move)
        {
            moveDir = touchPosition - startPos;
            moveDir = new Vector2(Mathf.Floor(moveDir.x), Mathf.Floor(moveDir.y));
        }

        else if(playerTouch == ETouchState.End)
        {
            if (new Vector2(Mathf.Floor(moveDir.x), Mathf.Floor(moveDir.y)) == new Vector2(0, 0)) return; 

            else
            {
                if (moveDir.x > moveDir.y) moveDir.y = 0;
                else moveDir.x = 0;

                moveDir.Normalize();
                moveStart = true;
            }
        }
    }

    void PlayerMove()
    {
        if (moveStart)
        {
            transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
        }
    }
}