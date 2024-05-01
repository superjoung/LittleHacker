using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class MakeJsonMapData : MonoBehaviour
{
    // CSV 파일
    private string csvFilePath = "SN_1_ST_1"; // 수정필요
    // JSON 파일 저장 경로
    private string jsonFilePath = "Assets/Resources/Map_data1.json"; // 수정필요

    void Start()
    {
        SaveMapDataToJson();
    }

    void SaveMapDataToJson()
    {
        // CSV 파일 읽기
        List<Dictionary<string, object>> csvData = CSVReader.Read(csvFilePath);

        // JSON으로 변환할 객체 생성
        MapData mapData = new MapData();
        mapData.Walls = new List<List<int>>();
        mapData.Numbers = new List<List<string>>();
        mapData.Operators = new List<List<string>>();
        mapData.PlayerPosition = new Vector2();
        mapData.DoorPosition = new Vector2();
        mapData.DoorValue = new int();

        int rowIndex = 0;
        foreach (var row in csvData)
        {
            List<int> wallRow = new List<int>();
            List<string> numberRow = new List<string>();
            List<string> operatorRow = new List<string>();

            int colIndex = 0;
            foreach (var col in row)
            {
                string value = col.Value.ToString();
                if (value == "1")  // 벽
                {
                    wallRow.Add(1);
                    numberRow.Add("");
                    operatorRow.Add("");
                }
                else if (value == "0")  // 배경
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                }
                else if (value.StartsWith("3_"))  // 숫자
                {
                    wallRow.Add(0);
                    numberRow.Add(value.Split('_')[1]);
                    operatorRow.Add("");
                }
                else if (value.StartsWith("4_") || value.StartsWith("5_") || value.StartsWith("6_") || value.StartsWith("7_"))  // 연산자
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add(value.Split('_')[1]);
                }
                else if (value == "2")  // 플레이어 위치
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    mapData.PlayerPosition = new Vector2(rowIndex, colIndex);
                }
                else if(value.StartsWith("8_"))   // 문 위치
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    mapData.DoorPosition = new Vector2(rowIndex, colIndex);
                    mapData.DoorValue = int.Parse(value.Split('_')[1]);
                }
                colIndex++;
            }

            mapData.Walls.Add(wallRow);
            mapData.Numbers.Add(numberRow);
            mapData.Operators.Add(operatorRow);

            rowIndex++;
        }

        // Json에서 Vector2는 재귀참조 문제가 생겨서 루프를 무시해줘야함
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        // JSON 파일로 저장
        string json = JsonConvert.SerializeObject(mapData, Formatting.Indented, settings);

        /* string json = JsonUtility.ToJson(mapData, true); 이거쓰고싶은데 뭔가 직렬화 안된다해서 안쓰는중입니다
         * json파일이 한줄로 쭉 출력되어서 안이쁘긴해요
         */

        File.WriteAllText(jsonFilePath, json);
    }
}

[System.Serializable]
public class MapData
{
    public List<List<int>> Walls { get; set; }
    public List<List<string>> Numbers { get; set; }
    public List<List<string>> Operators { get; set; }
    public Vector2 PlayerPosition;
    public Vector2 DoorPosition;
    public int DoorValue;
}