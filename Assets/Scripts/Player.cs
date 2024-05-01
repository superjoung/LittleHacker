using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public enum ETouchState { None, Begin, Move, End };
    public ETouchState playerTouch = ETouchState.None;

    private Vector2 touchPosition = new Vector2(0, 0);
    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 moveDir = new Vector2(0, 0);

    public float playerMoveSpeed;
    private bool moveStart = false;

    public Dictionary<int, ObjectData> formula = new Dictionary<int, ObjectData>();
    public TMP_Text[] formulaUi = new TMP_Text[3];
    public int formulaTotalNum = 0;
    private int formulaCount = 0;

    public void Start()
    {
        int count = 0;
        foreach(Transform formulaInfoUi in GameObject.Find("FormulaBackGround").transform)
        {
            formulaUi[count] = formulaInfoUi.GetComponent<TMP_Text>();
            count++;
        }
        Initialized();
    }

    public void Update()
    {
        TouchSetup();
        if(moveStart == false)
        {
            PlayerMoveDIr();
        }
        PlayerMove();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckFormula();
        }
    }

    public void Initialized()
    {
        formula.Clear();
        formulaUi[0].text = "";
        formulaUi[1].text = "";
        formulaUi[2].text = "";
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
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y)) moveDir.y = 0;
                else moveDir.x = 0;

                moveDir.Normalize();
                moveStart = true;
            }
        }
    }

    // player가 움직일 때 벽에 가로막힘 판정에 대해 계산
    void PlayerMove()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Item"));
        if (moveStart)
        {
            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, moveDir, 0.5f, layerMask);
            RaycastHit2D hitDoor = Physics2D.Raycast(transform.position, moveDir, 0.5f, LayerMask.GetMask("Door"));
            PlayerGetItem();
            transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
            if (hitWall)
            {
                // 벽 처리
                if(hitWall.transform.tag == "Wall" || hitWall.transform.tag == "Oper" && formulaCount / 2 == 0 || hitWall.transform.tag == "Number" && formulaCount / 2 == 1)
                {
                    moveStart = false;
                    transform.position = new Vector2(hitWall.transform.position.x - moveDir.x, hitWall.transform.position.y - moveDir.y);
                }
            }

            if (hitDoor)
            {
                if (hitDoor.transform.GetComponent<ObjectData>().num != formulaTotalNum)
                {
                    moveStart = false;
                    transform.position = new Vector2(hitDoor.transform.position.x - moveDir.x, hitDoor.transform.position.y - moveDir.y);
                }
            }
        }
    }

    // player가 수식들을얻 얻었을 경우 또는 문에 닿았을 경우를 나눠서 계산
    void PlayerGetItem()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Item")) + (1 << LayerMask.NameToLayer("Door"));
        RaycastHit2D hitItem = Physics2D.Raycast(transform.position, moveDir, 0.3f, layerMask);
        if (hitItem)
        {
            ObjectData OD = hitItem.transform.GetComponent<ObjectData>();
            // 먹은 오브젝트가 숫자일 경우
            if (hitItem.transform.tag == "Number" && formulaCount / 2 == 0)
            {
                formula.Add(formulaCount, OD);
                formulaUi[formulaCount % 3].text = "" + OD.num;
                formulaCount++;
                Destroy(hitItem.transform.gameObject);
            }

            // 먹은 오브젝트가 연산자일 경우
            else if (hitItem.transform.tag == "Operator" && formulaCount / 2 == 1)
            {
                formula.Add(formulaCount, OD);
                formulaUi[1].text = OD.oper;
                formulaCount++;
                Destroy(hitItem.transform.gameObject);
            }

            else if(hitItem.transform.tag == "Door")
            {
                if(hitItem.transform.GetComponent<ObjectData>().num == formulaTotalNum)
                {
                    Debug.Log("Clear");
                    Destroy(hitItem.transform.gameObject);
                }
                else
                {
                    return;
                }
            }

            // 수식란에 모든 칸이 다 채워져 있는 경우 갱신
            if(formulaCount % 3 == 0)
            {
                PlayerCalculate();
            }
        }
        else
        {
            return;
        }

        // totalNum안에 값을 넣어서 문으로 통과 가능한지 확인
        if((formulaCount-1) % 3 == 0)
        {
            formulaTotalNum = formula[formulaCount - 1].num;
        }
    }

    // 수식 계산 숫자 + 연산자 + 숫자 순서로 수식이 생겼을 때 계산해주는 함수
    void PlayerCalculate()
    {
        ObjectData OD = new ObjectData();
        switch (formula[formulaCount - 2].oper)
        {
            case "-":
                OD.num = formula[formulaCount - 3].num - formula[formulaCount - 1].num;
                formula.Add(formulaCount, OD);
                break;
            case "+":
                OD.num = formula[formulaCount - 3].num + formula[formulaCount - 1].num;
                formula.Add(formulaCount, OD);
                break;
            case "/":
                OD.num = formula[formulaCount - 3].num / formula[formulaCount - 1].num;
                formula.Add(formulaCount, OD);
                break;
            case "*":
                OD.num = formula[formulaCount - 3].num * formula[formulaCount - 1].num;
                formula.Add(formulaCount, OD);
                break;
            default:
                Debug.LogError("Playe.cs 파일 중 PlayerCalculate 오류 해당 연산자 없음");
                break;
        }
        // 수식 초기화
        formulaUi[0].text = "" + formula[formulaCount].num;
        formulaUi[1].text = "";
        formulaUi[2].text = "";
        formulaCount++;
    }

    // 디버그용 함수
    void CheckFormula()
    {
        for(int count = 0; count < formulaCount; count++)
        {
            if(count / 2 == 0) Debug.Log("iter count " + count + " : " + formula[count].num);
            else if (count / 2 == 0) Debug.Log("iter count " + count + " : " + formula[count].oper);
        }
    }
}