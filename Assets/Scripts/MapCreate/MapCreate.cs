using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreate : MonoBehaviour
{
    List<Dictionary<string, object>> data_Stage = new List<Dictionary<string, object>>();
    [SerializeField]
    List<GameObject> renderObj = new List<GameObject>();

    private float mapX;
    private float mapY;
    private Vector2 renderPos;
    private GameObject mapBox;

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
        // 맵 초기화
        foreach(Transform child in mapBox.transform)
        {
            Destroy(child.gameObject);
        }
        // X, Y 최대 구역 산정
        mapX = data_Stage.Count - 1;
        mapY = data_Stage[0].Count - 1;

        renderPos = new Vector2(-GameManager.gridSize * (mapX / 2), GameManager.gridSize * (mapY / 2));

        for(int countX = 0; countX < data_Stage.Count; countX++)
        {
            foreach(KeyValuePair<string, object> child in data_Stage[countX])
            {
                Instantiate(renderObj[int.Parse(child.Value.ToString())], renderPos, Quaternion.identity, mapBox.transform);
                renderPos.y -= GameManager.gridSize;
            }
            renderPos.y = GameManager.gridSize * (mapY / 2);
            renderPos.x += GameManager.gridSize;
        }
    }
}
