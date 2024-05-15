using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ���� �����ϰ� �ִ� �������� �� �ó����� ����
    static public int currentScenario = 1;
    static public int currentStage = 1;
    static public int playerTurn = 0;
    static public float gridSize = 1;

    public GameObject Canvas;

    public Player player;
    public MapCreate mapCreate;

    private void Awake()
    {
        SpawnInitialized();
        currentScenario = 1;
        currentStage = 1;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        mapCreate = transform.GetComponent<MapCreate>();
    }

    void SpawnInitialized()
    {
        Instantiate(Canvas);
    }

    // ���� ��ũ��Ʈ Initialized �Լ��� ��Ƽ� �����ų����
    public void StageClear()
    {
        currentStage++;

        mapCreate.Initialize("SN_" + currentScenario.ToString() + "_ST_" + currentStage.ToString());
        player.Initialized();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
}
