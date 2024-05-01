using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class MapCreate : MonoBehaviour
{
    [SerializeField]
    List<GameObject> renderObj = new List<GameObject>();

    private float mapX;
    private float mapY;

    private Vector2 renderPos; // � ��ǥ�� ���� render�ؾ��ϴ°�
    private GameObject mapBox; // ��Ƶ� �ڽ�
    private string stageInfo;


    private void Start()
    {
        stageInfo = "SN_" + GameManager.currentScenario.ToString() + "_ST_" + GameManager.currentStage.ToString();
        Initialize(stageInfo);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.currentStage++;
            stageInfo = "SN_" + GameManager.currentScenario.ToString() + "_ST_" + GameManager.currentStage.ToString();

            Initialize(stageInfo);
        }
    }

    public void Initialize(string stageJsonData)
    {
        TextAsset jsonData = Resources.Load<TextAsset>("MapDatasJSON/" + stageJsonData); // �ó�����&������ �߰� �ؾ��� �ڵ� �����ʿ�

        if (jsonData == null)
        {
            Debug.LogError("Failed to load map data!");
            return;
        }

        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonData.text);
        if (mapData == null)
        {
            Debug.LogError("Failed to parse map data!");
            return;
        }

        RenderMap(mapData);
    }

    private void RenderMap(MapData mapData)
    {
        mapBox = GameObject.Find("Maps");

        foreach (Transform child in mapBox.transform)
        {
            Destroy(child.gameObject);
        }

        mapX = mapData.Walls[0].Count - 1;
        mapY = mapData.Walls.Count - 1;

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

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

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

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
                        case "x":
                            operatorObj = Instantiate(renderObj[5], new Vector3(renderPos.x, renderPos.y, -1), Quaternion.identity, mapBox.transform);
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.Operators[y][x];
                            operatorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = "x";
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

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        Vector2 playerPosition = mapData.PlayerPosition;
        Instantiate(
            renderObj[2],
            new Vector3(renderPos.x + playerPosition.x * GameManager.gridSize, renderPos.y - playerPosition.y * GameManager.gridSize, -2),
            Quaternion.identity, mapBox.transform
        );

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        Vector2 DoorPosition = mapData.DoorPosition;
        GameObject doorObj = Instantiate(
            renderObj[8],
            new Vector3(renderPos.x + DoorPosition.x * GameManager.gridSize, renderPos.y - DoorPosition.y * GameManager.gridSize, -2),
            Quaternion.identity, mapBox.transform
        );
        doorObj.GetComponent<ObjectData>().num = mapData.DoorValue;
        doorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = mapData.DoorValue.ToString();

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
