using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class MakeJsonMapData : MonoBehaviour
{   
    private string csvDirectoryPath;     // CSV 파일들이 있는 폴더 경로
    private string jsonDirectoryPath;    // JSON 파일들을 저장할 폴더 경로

    void Start()
    {
        csvDirectoryPath = "Assets/Resources/MapDatasCSV";
        jsonDirectoryPath = "Assets/Resources/MapDatasJSON";

        InitializeJsonData(); // 존재하는 Json 파일들 지우기 (필요하면 주석 처리)

        // 디렉토리에서 모든 CSV 파일 목록을 가져오기
        string[] csvFiles = Directory.GetFiles(csvDirectoryPath, "*.csv");

        foreach (string csvFile in csvFiles)
        {
            // Debug.Log("csvFile: " + csvFile);

            // 파일의 전체 경로에서 파일 이름만 추출 (확장자 제외)
            string fileName = csvFile.Split('\\')[1].Split('.')[0];

            // Debug.Log("CSV File Name: " + fileName);

            // JSON 파일의 완전한 경로 생성
            string jsonFilePath = jsonDirectoryPath + "/" + fileName + ".json";

            // Debug.Log("JSON File Name: " + jsonFilePath);

            SaveMapDataToJson(fileName, jsonFilePath);
        }
    }

    // JSON 파일들이 저장된 폴더의 모든 파일을 지우기
    void InitializeJsonData()
    {
        
        if (Directory.Exists(jsonDirectoryPath))
        {
            string[] files = Directory.GetFiles(jsonDirectoryPath);
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log("DoneFileDelete : " + file); // 적는게 느려서 확인하는 코드
            }
        }
    }

    void SaveMapDataToJson(string csvFilePath, string jsonFilePath)
    {
        // CSV 파일 읽기
        List<Dictionary<string, object>> csvData = CSVReader.Read(csvFilePath);

        // JSON으로 변환할 객체 생성
        MapData mapData = new MapData();
        mapData.Walls = new List<List<int>>();
        mapData.Numbers = new List<List<string>>();
        mapData.Operators = new List<List<string>>();
        mapData.Boxes = new List<List<int>>();
        mapData.Doors = new List<List<string>>();
        mapData.PlayerPosition = new Vector2();

        int rowIndex = 0;
        foreach (var row in csvData)
        {
            List<int> wallRow = new List<int>();
            List<string> numberRow = new List<string>();
            List<string> operatorRow = new List<string>();
            List<int> boxRow = new List<int>();
            List<string> doorRow = new List<string>();

            int colIndex = 0;
            foreach (var col in row)
            {
                string value = col.Value.ToString();
                if (value == "1")  // 벽
                {
                    wallRow.Add(1);
                    numberRow.Add("");
                    operatorRow.Add("");
                    boxRow.Add(0);
                    doorRow.Add("");
                }
                else if (value == "0")  // 배경
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    boxRow.Add(0);
                    doorRow.Add("");
                }
                else if (value.StartsWith("3_"))  // 숫자
                {
                    wallRow.Add(0);
                    numberRow.Add(value.Split('_')[1]);
                    operatorRow.Add("");
                    boxRow.Add(0);
                    doorRow.Add("");
                }
                else if (value.StartsWith("4_") || value.StartsWith("5_") || value.StartsWith("6_") || value.StartsWith("7_"))  // 연산자
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add(value.Split('_')[1]);
                    boxRow.Add(0);
                    doorRow.Add("");
                }
                else if (value == "2")  // 플레이어 위치
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    boxRow.Add(0);
                    doorRow.Add("");
                    mapData.PlayerPosition = new Vector2(colIndex, rowIndex);
                }
                else if(value.StartsWith("8_"))   // 문 위치
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    boxRow.Add(0);
                    doorRow.Add((value.Split('_')[1]));
                }
                else if(value.StartsWith("9_"))     // 상자의 위치
                {
                    wallRow.Add(0);
                    numberRow.Add("");
                    operatorRow.Add("");
                    boxRow.Add(1);
                    doorRow.Add("");
                }
                colIndex++;
            }

            mapData.Walls.Add(wallRow);
            mapData.Numbers.Add(numberRow);
            mapData.Operators.Add(operatorRow);
            mapData.Boxes.Add(boxRow);
            mapData.Doors.Add(doorRow);

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

        Debug.Log("DoneFileWrite : " + jsonFilePath); // 적는게 느려서 확인하는 코드
    }
}

[System.Serializable]
public class MapData
{
    public List<List<int>> Walls { get; set; }
    public List<List<string>> Numbers { get; set; }
    public List<List<string>> Operators { get; set; }
    public List<List<int>> Boxes { get; set; }
    public Vector2 PlayerPosition;
    public List<List<string>> Doors { get; set; }
}