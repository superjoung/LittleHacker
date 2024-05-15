using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using UnityEditor.SceneManagement;

public class revertObject
{
    public enum ERevertType {formula, box};
    public List<GameObject> objects = new List<GameObject>();
    public List<Vector2> transforms = new List<Vector2>();
    public List<ERevertType> revertTypes = new List<ERevertType>();
    public Vector2 playerPos;

    public bool formulaCalcule = false;

    public revertObject(List<GameObject> objects, List<Vector2> transforms, List<ERevertType> revertTypes, Vector2 playerPos, bool formulaCalcule)
    {
        this.objects = objects;
        this.transforms = transforms;
        this.revertTypes = revertTypes;
        this.playerPos = playerPos;
        this.formulaCalcule = formulaCalcule;
    }

    public revertObject()
    {
        return;
    }
}


public class Player : MonoBehaviour
{
    // ȭ�� ��ġ �Լ�
    public enum ETouchState { None, Begin, Move, End };
    public ETouchState playerTouch = ETouchState.None;
    private Vector2 touchPosition = new Vector2(0, 0);
    private Vector2 startPos = new Vector2(0, 0);
    public Vector2 moveDir = new Vector2(0, 0);
    private Vector2 calculMoveDir = new Vector2(0, 0);
    public List<Vector2> moveDirs = new List<Vector2>();

    // �ϴ� �ΰ��� Player ����
    public float playerMoveSpeed;
    private bool moveStart = false;

    // key = ������ ��, value Ŭ����
    public Dictionary<int, revertObject> backUpRevert = new Dictionary<int, revertObject>();
    public revertObject revertObjects = new revertObject();

    // ���� ����
    public Dictionary<int, ObjectData> formula = new Dictionary<int, ObjectData>();
    public TMP_Text[] formulaUi = new TMP_Text[3];
    public int formulaTotalNum = 0;
    private int formulaCount = 0;
    private bool formulaCalculate = false;

    public GameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        MoveKeyBind();
        PlayerMoveDIr();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckFormula();
        }
    }

    private void FixedUpdate()
    {
        if(moveDirs.Count != 0) PlayerMove();
    }

    public void Initialized()
    {
        formula.Clear();
        formulaUi[0].text = "";
        formulaUi[1].text = "";
        formulaUi[2].text = "";
        formulaTotalNum = 0;
        backUpRevert.Clear();

        GameObject.Find("BackStartButton").GetComponent<Button>().onClick.AddListener(() => BackStartButtonClick());
        GameObject.Find("ReStartButton").GetComponent<Button>().onClick.AddListener(() => ReStartButtonClick());
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

    void MoveKeyBind()
    {
        if (playerTouch == ETouchState.None)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                moveStart = true;
                moveDirs.Add(new Vector2(0, 1));
                formulaCalculate = false;
                revertObjects.playerPos = transform.position;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                moveStart = true;
                moveDirs.Add(new Vector2(-1, 0));
                formulaCalculate = false;
                revertObjects.playerPos = transform.position;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                moveStart = true;
                moveDirs.Add(new Vector2(0, -1));
                formulaCalculate = false;
                revertObjects.playerPos = transform.position;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                moveStart = true;
                moveDirs.Add(new Vector2(1, 0));
                formulaCalculate = false;
                revertObjects.playerPos = transform.position;
            }
        }
    }

    void PlayerMoveDIr()
    {
        if (playerTouch == ETouchState.Begin)
        {
            startPos = touchPosition;
        }

        else if (playerTouch == ETouchState.Move)
        {
            calculMoveDir = touchPosition - startPos;
            calculMoveDir = new Vector2(Mathf.Floor(calculMoveDir.x), Mathf.Floor(calculMoveDir.y));
        }

        else if (playerTouch == ETouchState.End)
        {
            if (new Vector2(Mathf.Floor(calculMoveDir.x), Mathf.Floor(calculMoveDir.y)) == new Vector2(0, 0)) return;

            else
            {
                if (Mathf.Abs(calculMoveDir.x) > Mathf.Abs(calculMoveDir.y)) calculMoveDir.y = 0;
                else calculMoveDir.x = 0;

                calculMoveDir.Normalize();
                moveDirs.Add(calculMoveDir);
                moveStart = true;
                formulaCalculate = false;
                revertObjects.playerPos = transform.position;
            }
        }
    }

    // player�� ������ �� ���� ���θ��� ������ ���� ���
    void PlayerMove()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Item"));
        moveDir = moveDirs[0];
        PlayerGetItem();
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, moveDir, 0.6f, layerMask);
        RaycastHit2D hitDoor = Physics2D.Raycast(transform.position, moveDir, 0.6f, LayerMask.GetMask("Door"));
        RaycastHit2D hitTrigger = Physics2D.Raycast(transform.position, moveDir, 0.6f, LayerMask.GetMask("Trigger"));

        transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
        if (hitWall)
        {
            // �� ó��
            if (hitWall.transform.tag == "Wall" || hitWall.transform.tag == "Operator" && formulaCount % 3 != 1 || hitWall.transform.tag == "Number" && formulaCount % 3 == 1)
            {
                moveStart = false;
                transform.position = new Vector2(hitWall.transform.position.x - moveDir.x, hitWall.transform.position.y - moveDir.y);
            }
        }

        // ���������� �������� �� ������ �������� �ʾ��� ���
        if (hitDoor)
        {
            if (hitDoor.transform.GetComponent<ObjectData>().num != formulaTotalNum || formulaCount % 3 != 1)
            {
                moveStart = false;
                transform.position = new Vector2(hitDoor.transform.position.x - moveDir.x, hitDoor.transform.position.y - moveDir.y);
            }
        }

        // box�� �ε����� �� ����� ��ũ��Ʈ
        if (hitTrigger)
        {
            if (hitTrigger.transform.tag == "Box" && moveStart == true)
            {
                ObjectData box = hitTrigger.transform.GetComponent<ObjectData>();

                if (box.boxStop == true)
                {
                    moveStart = false;
                    transform.position = new Vector2(hitTrigger.transform.position.x - moveDir.x, hitTrigger.transform.position.y - moveDir.y);
                    box.boxStop = false;
                }
                else
                {
                    if (box.boxTrigger == false)
                    {
                        InputRevertObject(hitTrigger.transform.gameObject);
                        box.boxMoveDir = moveDir;
                        box.boxTrigger = true;
                    }
                }
            }
        }

        // ���� �������� ��ġ�ؾ��ϴ� �Լ� ���� ������ ���� player move Initializer��� �����ϸ� ��
        if (moveStart == false)
        {
            backUpRevert.Add(GameManager.playerTurn, new revertObject(revertObjects.objects.ToList(), revertObjects.transforms.ToList(), revertObjects.revertTypes.ToList(), revertObjects.playerPos, formulaCalculate));
            GameManager.playerTurn++;
            formulaCalculate = false;
            moveStart = true;
            moveDirs.RemoveAt(0);
            revertObjects.objects.Clear();
            revertObjects.transforms.Clear();
            revertObjects.revertTypes.Clear();
            revertObjects.playerPos = transform.position;
        }

    }

    // player�� ���ĵ��� ����� ��� �Ǵ� ���� ����� ��츦 ������ ��� ���ĵ鸸 �־�δ� ������
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
                InputRevertObject(hitItem.transform.gameObject);
                formula.Add(formulaCount, OD);
                formulaUi[formulaCount % 3].text = "" + OD.num;
                if (formulaCount % 3 == 0) formulaTotalNum = formula[formulaCount].num;
                formulaCount++;
                hitItem.transform.gameObject.SetActive(false);
            }

            // ���� ������Ʈ�� �������� ���
            else if (hitItem.transform.tag == "Operator" && formulaCount % 3 == 1)
            {
                InputRevertObject(hitItem.transform.gameObject);
                formula.Add(formulaCount, OD);
                formulaUi[1].text = OD.oper;
                formulaCount++;
                hitItem.transform.gameObject.SetActive(false);
            }

            else if(hitItem.transform.tag == "Door" && formulaCount % 3 == 1)
            {
                if(hitItem.transform.GetComponent<ObjectData>().num == formulaTotalNum)
                {
                    gameManager.StageClear();
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
                formulaCalculate = true;
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
        formula.Add(formulaCount, gameObject.AddComponent<ObjectData>());
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

    // �� �ٽ� ���ε� Initialize �����ؾ���
    void ReStartButtonClick()
    {
        GameObject.Find("GameManager").GetComponent<MapCreate>().Initialize("SN_" + GameManager.currentScenario.ToString() + "_ST_" + GameManager.currentStage.ToString());
        Initialized();
    }

    // �ǵ����� ��ư�� ������ �� class enum�� �°� �Լ� ���� �ǰ� ��������
    void BackStartButtonClick()
    {
        if(backUpRevert.Count != 0)
        {
            revertObject revertObj = backUpRevert[GameManager.playerTurn - 1];
            transform.position = revertObj.playerPos;

            for (int count = 0; count < revertObj.revertTypes.Count; count++)
            {
                Debug.Log("iter : " + count);
                switch (revertObj.revertTypes[count])
                {
                    // ���� �Լ��� ���� active ���¸� ����
                    case revertObject.ERevertType.formula:
                        revertObj.objects[count].SetActive(true);
                        formulaCount--;
                        formula.Remove(formulaCount);
                        formulaUi[formulaCount % 3].text = "";
                        Debug.Log("formula BackUp");
                        break;

                    // �ڽ��� transform�� ������
                    case revertObject.ERevertType.box:
                        revertObj.objects[count].transform.position = revertObj.transforms[count];
                        Debug.Log("box BackUp");
                        break;
                }
            }

            // ���� ����� ���� ��� ����ó��
            if (revertObj.formulaCalcule)
            {
                formulaCount--;
                formula.Remove(formulaCount);
            }

            int currentCount = formulaCount - 1;

            // ���� Ui ����
            for(int count = 0; count <= currentCount % 3; count++)
            {
                if(currentCount % 3 - count == 1)
                {
                    formulaUi[currentCount % 3 - count].text = formula[currentCount - count].oper;
                }
                else
                {
                    formulaUi[currentCount % 3 - count].text = "" + formula[currentCount - count].num;
                }
            }

            backUpRevert.Clear();
        }
    }

    // revertObject ����Ÿ ��ǲ �Լ�
    void InputRevertObject(GameObject hit)
    {
        revertObjects.objects.Add(hit);
        revertObjects.transforms.Add(hit.transform.position);

        switch (hit.transform.tag)
        {
            case "Number":
                revertObjects.revertTypes.Add(revertObject.ERevertType.formula);
                break;
            case "Operator":
                revertObjects.revertTypes.Add(revertObject.ERevertType.formula);
                break;
            case "Box":
                revertObjects.revertTypes.Add(revertObject.ERevertType.box);
                break;
        }
    }
}