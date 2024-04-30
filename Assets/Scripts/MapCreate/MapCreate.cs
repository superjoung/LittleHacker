using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapCreate : MonoBehaviour
{
    // CSV 파일 파싱 받을 변수 공간 data_Stage[행][열]
    List<Dictionary<string, object>> data_Stage = new List<Dictionary<string, object>>();
    [SerializeField]
    List<GameObject> renderObj = new List<GameObject>(); // 값에 따라 render해줄 오브젝트들

    private float mapX;
    private float mapY;
    private Vector2 renderPos; // 어떤 좌표에 지금 render해야하는가
    private GameObject mapBox; // 담아둘 박스

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.currentStage++;
            Initialize();
        }
    }

    public void Initialize()
    {
        data_Stage = CSVReader.Read("SN_" + GameManager.currentScenario.ToString() + "_ST_" + GameManager.currentStage.ToString());
        MapRender();
    }

    private void MapRender()
    {
        mapBox = GameObject.Find("Maps");
        GameObject tmpObj;
        // 맵 초기화
        foreach (Transform child in mapBox.transform)
        {
            Destroy(child.gameObject);
        }
        // X, Y 최대 구역 산정
        mapX = data_Stage[0].Count - 1;
        mapY = data_Stage.Count - 1;

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        for(int countX = 0; countX < data_Stage.Count; countX++) // 열에 있는 모든 행의 값 부터 다 출력 후 열 1칸 이동
        {
            foreach(KeyValuePair<string, object> child in data_Stage[countX]) // 열에 해당하는 행 출력
            {
                // 아무것도 안들어가 있는 경우 패스
                if(child.Value == null)
                {
                    renderPos.x += GameManager.gridSize;
                    continue;
                }
                // 일단 _ 부호 기준으로 문자열 스플릿
                string[] splitText = child.Value.ToString().Split('_');
                MapSuvCreate(splitText);
                // 숫자 오브젝트가 생성되어야 할때
                Instantiate(renderObj[0], renderPos, Quaternion.identity, mapBox.transform);

                renderPos.x += GameManager.gridSize;
            }
            renderPos.x = -GameManager.gridSize * (mapX / 2);
            renderPos.y -= GameManager.gridSize;
        }
    }

    void MapSuvCreate(string[] splitText)
    {
        GameObject tmpObj;
        string[] suvTmpText = new string[3];
        switch (int.Parse(splitText[0].ToString()))
        {
            case 2:
                Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity);
                break;
            case 3:
                tmpObj = Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                tmpObj.GetComponent<ObjectData>().num = int.Parse(splitText[1].ToString());
                tmpObj.transform.GetChild(0).GetComponent<TMP_Text>().text = splitText[1];
                break;

            case 8:
                tmpObj = Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                tmpObj.GetComponent<ObjectData>().doorNum = int.Parse(splitText[1].ToString());
                tmpObj.transform.GetChild(0).GetComponent<TMP_Text>().text = splitText[1];
                break;

            case 9:
                tmpObj = Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                if (splitText.Length > 1)
                {
                    suvTmpText.CopyTo(splitText, 1);
                    MapSuvCreate(suvTmpText);
                }
                break;

            default:
                Instantiate(renderObj[int.Parse(splitText[0].ToString())], renderPos, Quaternion.identity, mapBox.transform);
                break;
        }
    }
}
