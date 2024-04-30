using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class MapCreate : MonoBehaviour
{
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
        TextAsset jsonData = Resources.Load<TextAsset>("Map_data1"); // 시나리오&레벨은 추가 해야함 코드 수정필요
        if (jsonData == null)
        {
            Debug.LogError("Failed to load map data!");
            return;
        }

        //MapData mapData = JsonUtility.FromJson<MapData>(jsonData.text); /* JsonUtility는 이차원배열을 다루지 못함 */

        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonData.text); // 맵 정보로 가져오기
        if (mapData == null)
        {
            Debug.LogError("Failed to parse map data!");
            return;
        }

        RenderMap(mapData); // 맵 데이터를 가지고 렌더링
    }

    private void RenderMap(MapData mapData)
    {
        mapBox = GameObject.Find("Maps");

        // 맵 초기화
        foreach (Transform child in mapBox.transform)
        {
            Destroy(child.gameObject);
        }

        // X, Y 최대크기 산정
        mapX = mapData.Walls[0].Count - 1;
        mapY = mapData.Walls.Count - 1;

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        // 벽을 먼저 렌더링 (레이어 0)
        for (int y = 0; y < mapData.Walls.Count; y++)
        {
            for (int x = 0; x < mapData.Walls[y].Count; x++)
            {
                GameObject prefab = mapData.Walls[y][x] == 1 ? renderObj[1] : renderObj[0];
                Instantiate(prefab, new Vector3(renderPos.x, renderPos.y, 0), Quaternion.identity, mapBox.transform);
                renderPos.x += GameManager.gridSize;
            }
            renderPos.x = -GameManager.gridSize * (mapX / 2);
            renderPos.y -= GameManager.gridSize;
        }

        // 렌더링 위치 초기화
        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        // 숫자와 연산자 렌더링 (레이어 -1)
        for (int y = 0; y < mapData.Numbers.Count; y++)
        {
            for (int x = 0; x < mapData.Numbers[y].Count; x++)
            {
                if (!string.IsNullOrEmpty(mapData.Numbers[y][x]))
                {
                    GameObject numberObj = Instantiate(renderObj[3], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                    numberObj.GetComponent<ObjectData>().num = int.Parse(mapData.Numbers[y][x]);
                    numberObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Numbers[y][x];
                }
                if (!string.IsNullOrEmpty(mapData.Operators[y][x]))
                {
                    GameObject operatorObj;
                    switch (mapData.Operators[y][x]) {
                        case "+":
                            operatorObj = Instantiate(renderObj[4], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Operators[y][x];
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = "+";
                            break;
                        case "-":
                            operatorObj = Instantiate(renderObj[5], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Operators[y][x];
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = "-";
                            break;
                        case "*":
                            operatorObj = Instantiate(renderObj[5], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Operators[y][x];
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = "*";
                            break;
                        case "/":
                            operatorObj = Instantiate(renderObj[5], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Operators[y][x];
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = "/";
                            break;
                    }
                        
                    
                }
                renderPos.x += GameManager.gridSize;
            }
            renderPos.x = -GameManager.gridSize * (mapX / 2);
            renderPos.y -= GameManager.gridSize;
        }

        // 렌더링 위치 초기화
        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        // 플레이어 렌더링 (레이어 -2)
        Vector2 playerPosition = mapData.PlayerPosition;
        Instantiate(
            renderObj[2],
            new Vector3(renderPos.x + playerPosition.x * GameManager.gridSize, renderPos.y - playerPosition.y * GameManager.gridSize, -2),
            Quaternion.identity, mapBox.transform
        );

        // 렌더링 위치 초기화
        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        // 문 렌더링 (레이어 -2)
        Vector2 DoorPosition = mapData.DoorPosition;
        GameObject doorObj = Instantiate(
            renderObj[8],
            new Vector3(renderPos.x + DoorPosition.x * GameManager.gridSize, renderPos.y - DoorPosition.y * GameManager.gridSize, -2),
            Quaternion.identity, mapBox.transform
        );
        doorObj.GetComponent<ObjectData>().num = mapData.DoorValue;
        doorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.DoorValue.ToString();

    }
}
