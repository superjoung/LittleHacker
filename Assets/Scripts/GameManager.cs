using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 지금 진행하고 있는 스테이지 및 시나리오 갱신
    static public int currentScenario = 1;
    static public int currentStage = 1;
    static public int playerTurn = 0;
    static public float gridSize = 1;

    public GameObject Canvas;

    private void Awake()
    {
        SpawnInitialized();
        currentScenario = 1;
        currentStage = 1;
    }

    void SpawnInitialized()
    {
        Instantiate(Canvas);
    }
}
