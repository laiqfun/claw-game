using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    // 单例实例
    public static GameManager Instance;

    // 娃娃预制体数组，可以在Inspector中赋值
    public GameObject[] dollPrefabs;

    // 娃娃生成的父物体，用于整理层级
    public Transform spawnParent;

    // 生成点
    public Vector3 fillPosition;

    // 当前场景中的娃娃数量
    private int currentDollCount = 0;

    // 最大娃娃数量
    public int maxDollCount = 20;

    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 清除静态引用，让 GC 能回收相关资源
        Instance = null;

        // 如果有订阅事件，在这里取消订阅，比如：
        // EventSystem.Instance.CustomEvent -= OnCustomEventCallback;
    }

    // 游戏开始时调用
    private void Start()
    {
        // 初始化生成娃娃
        SpawnInitialDolls();
    }

    // 初始化生成娃娃
    public void SpawnInitialDolls()
    {
        // 清空已有娃娃
        ClearAllDolls();

        // 生成指定数量的娃娃
        for (int i = 0; i < maxDollCount; i++)
        {
            SpawnRandomDoll();
        }
    }

    // 生成一个随机娃娃
    public void SpawnRandomDoll()
    {
        if (dollPrefabs.Length == 0)
        {
            Debug.LogError("请在Inspector中添加娃娃预制体！");
            return;
        }

        // 随机选择一个娃娃预制体
        int randomIndex = Random.Range(0, dollPrefabs.Length);
        GameObject selectedDollPrefab = dollPrefabs[randomIndex];

        // 生成位置在一个填充点自然下落
        Vector3 randomPosition = fillPosition;

        // 生成随机旋转
        Quaternion randomRotation = Quaternion.Euler(
            0, Random.Range(0,50), 0
        );

        // 实例化娃娃
        GameObject newDoll = Instantiate(
            selectedDollPrefab,
            randomPosition,
            randomRotation,
            spawnParent
        );

        currentDollCount++;
    }

    // 清除所有娃娃
    public void ClearAllDolls()
    {
        // 遍历所有子物体并销毁
        foreach (Transform child in spawnParent)
        {
            Destroy(child.gameObject);
        }

        currentDollCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toy"))
        {
            // 抓娃娃
            Destroy(other.gameObject);

            // 娃娃被抓走
            OnDollCaught();
        }
    }

    // 当娃娃被抓走时调用
    public void OnDollCaught()
    {
        currentDollCount--;
        Debug.Log("娃娃被抓走了 ！剩余: " + currentDollCount);
    }
}
