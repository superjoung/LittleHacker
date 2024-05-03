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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckFormula();
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }
    public void Initialized()
    {
        formula.Clear();
        formulaUi[0].text = "";
        formulaUi[1].text = "";
        formulaUi[2].text = "";
        formulaTotalNum = 0;
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
        bool isKeyPressed = false;

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDir = new Vector2(0, 1);
            isKeyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveDir = new Vector2(-1, 0);
            isKeyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveDir = new Vector2(0, -1);
            isKeyPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveDir = new Vector2(1, 0);
            isKeyPressed = true;
        }
        
        if(isKeyPressed)
        {
            moveStart = true;
            return;
        }

        if (playerTouch == ETouchState.Begin)
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

    // player�� ������ �� ���� ���θ��� ������ ���� ���
    void PlayerMove()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Item"));
        if (moveStart)
        {
            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, moveDir, 0.6f, layerMask);
            RaycastHit2D hitDoor = Physics2D.Raycast(transform.position, moveDir, 0.6f, LayerMask.GetMask("Door"));
            transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
            if (hitWall)
            {
                // �� ó��
                if(hitWall.transform.tag == "Wall" || hitWall.transform.tag == "Operator" && formulaCount % 3 != 1 || hitWall.transform.tag == "Number" && formulaCount % 3 == 1)
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

            PlayerGetItem();
        }
    }

    // player�� ���ĵ����� ����� ��� �Ǵ� ���� ����� ��츦 ������ ���
    void PlayerGetItem()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Item")) + (1 << LayerMask.NameToLayer("Door"));
        RaycastHit2D hitItem = Physics2D.Raycast(transform.position, moveDir, 0.3f, layerMask);
        if (hitItem)
        {
            ObjectData OD = hitItem.transform.GetComponent<ObjectData>();
            // ���� ������Ʈ�� ������ ���
            if (hitItem.transform.tag == "Number" && formulaCount % 3 == 0 || formulaCount % 3 == 2)
            {
                formula.Add(formulaCount, OD);
                formulaUi[formulaCount % 3].text = "" + OD.num;
                if (formulaCount % 3 == 0) formulaTotalNum = formula[formulaCount].num;
                formulaCount++;
                Destroy(hitItem.transform.gameObject);
            }

            // ���� ������Ʈ�� �������� ���
            else if (hitItem.transform.tag == "Operator" && formulaCount % 3 == 1)
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

            // ���Ķ��� ��� ĭ�� �� ä���� �ִ� ��� ����
            if(formulaCount % 3 == 0)
            {
                PlayerCalculate();
            }
        }
        else
        {
            return;
        }
    }

    // ���� ��� ���� + ������ + ���� ������ ������ ������ �� ������ִ� �Լ�
    void PlayerCalculate()
    {
        formula.Add(formulaCount, new ObjectData());
        switch (formula[formulaCount - 2].oper)
        {
            case "-":
                formula[formulaCount].num = formula[formulaCount - 3].num - formula[formulaCount - 1].num;
                break;
            case "+":
                formula[formulaCount].num = formula[formulaCount - 3].num + formula[formulaCount - 1].num;
                break;
            case "/":
                formula[formulaCount].num = formula[formulaCount - 3].num / formula[formulaCount - 1].num;
                break;
            case "*":
                formula[formulaCount].num = formula[formulaCount - 3].num * formula[formulaCount - 1].num;
                break;
            default:
                Debug.LogError("Playe.cs ���� �� PlayerCalculate ���� �ش� ������ ����");
                break;
        }
        // ���� �ʱ�ȭ
        formulaUi[0].text = "" + formula[formulaCount].num;
        formulaUi[1].text = "";
        formulaUi[2].text = "";

        formulaTotalNum = formula[formulaCount].num;
        formulaCount++;
    }

    // ����׿� �Լ�
    void CheckFormula()
    {
        for(int count = 0; count < formulaCount; count++)
        {
            if(count % 2 == 0) Debug.Log("iter count " + count + " : " + formula[count].num);
            else if (count % 2 == 1) Debug.Log("iter count " + count + " : " + formula[count].oper);
        }
    }
}