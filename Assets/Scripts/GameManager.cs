using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    // 单例实例
    public static GameManager Instance;

    public DanmakuController danmakuController;
    public AudioController audioController;
    // 是否暂停
    public bool isPaused { get; private set; } = false;

    public bool isStrongGrab { get; private set; } = false;
    public bool isInsertCoin { get; private set; } = false;
    public void InsertCoin()
    {
        isInsertCoin = true;
    }
    public void OverGrab()
    {
        isInsertCoin = false;
        if (disconnectToyCount >= 4)
        {
            disconnectToyCount = 0;
            isStrongGrab = true;
            danmakuController.FireDanmaku("太好了，是强力爪！我们有救了");
        }
    }

    // 娃娃预制体数组，可以在Inspector中赋值
    public GameObject dollPrefab;
    public Material[] emoteMaterial;

    // 娃娃生成的父物体，用于整理层级
    public Transform spawnParent;

    // 生成范围
    public Vector3[] fillRange;

    // 当前场景中的娃娃数量
    public int currentDollCount { get; private set; } = 0;

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

        // 在范围内随机生成娃娃
        for (int i = 0; i < maxDollCount; i++)
        {
            int randomIndex = Random.Range(0, fillRange.Length / 2) * 2;
            Vector3 randomPosition = new Vector3(
                Random.Range(fillRange[randomIndex].x, fillRange[randomIndex + 1].x),
                Random.Range(fillRange[randomIndex].y, fillRange[randomIndex + 1].y),
                Random.Range(fillRange[randomIndex].z, fillRange[randomIndex + 1].z)
            );
            Debug.Log("Spawning at " + randomPosition);
            SpawnDoll(randomPosition);
        }
    }

    // 生成一个娃娃
    public void SpawnDoll(Vector3 position)
    {
        // if (dollPrefabs.Length == 0)
        // {
        //     Debug.LogError("请在Inspector中添加娃娃预制体！");
        //     return;
        // }

        // // 随机选择一个娃娃预制体
        // int randomIndex = Random.Range(0, dollPrefabs.Length);
        // GameObject selectedDollPrefab = dollPrefabs[randomIndex];

        // 生成随机旋转
        Quaternion randomRotation = Quaternion.Euler(
            0, Random.Range(0, 50), 0
        );

        // 实例化娃娃
        GameObject newDoll = Instantiate(
            dollPrefab,
            position,
            randomRotation,
            spawnParent
        );
        int randomIndex = Random.Range(0, emoteMaterial.Length);
        Material borderMaterial = newDoll.transform.Find("Face").GetComponent<Renderer>().materials[0];
        Material[] materials = new Material[2];
        materials[0] = borderMaterial;
        materials[1] = emoteMaterial[randomIndex];
        newDoll.transform.Find("Face").GetComponent<Renderer>().materials = materials;
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
            Destroy(other.gameObject);

            // 娃娃被抓走
            OnDollCaught();
        }
    }

    // 当娃娃被抓走时调用
    public void OnDollCaught()
    {
        currentDollCount--;
        isStrongGrab = false;

        List<string> texts = new List<string>
            {
                "可以可以",
                "这也行？",
                "GREAT!",
                "不错不错",
            };
        danmakuController.FireDanmaku(texts[Random.Range(0, texts.Count)]);
        audioController.PlayGreatClip();
        if (Time.time < 5)
        {
            danmakuController.FireDanmaku("这么快就进" + (maxDollCount - currentDollCount) + "个了？");
        }
        if (currentDollCount < 5 && currentDollCount > 0)
        {
            danmakuController.FireDanmaku("还差" + currentDollCount + "个了！");
        }
        if (currentDollCount == 0)
        {
            audioController.PlaySuccessClip();
            danmakuController.FireDanmaku("woc!厉害厉害，给你全都抓完了！老实说，开发者都没想过会有人抓完~");
        }
    }
    void Update()
    {
        DanmakuUpdate();
    }
    private float actionTime;
    private int startExplain = 0;
    void DanmakuUpdate()
    {
        if (Time.time - actionTime > 30)
        {
            List<string> texts = new List<string>
            {
                "喂喂喂？",
                "人呢？",
                "睡着了吗？qwq",
                "Hi?",
                "qwq"
            };
            danmakuController.FireDanmaku(texts[Random.Range(0, texts.Count)]);
            actionTime += 10;
        }

        if (Time.time > startExplain * 1 && startExplain != -1)
        {
            List<string> texts = new List<string>
            {
                "按Q投币",
                "WASD移动摇杆~",
                "空格键开抓！",
            };
            danmakuController.FireDanmaku(texts[startExplain++]);
            if (startExplain >= texts.Count) startExplain = -1;
        }
    }
    public void action()
    {
        if (!isInsertCoin && Time.time - actionTime > 1)
        {
            List<string> texts = new List<string>
            {
                "请先投币~",
                "投币 投币 投币",
                "先按Q投币啊qwq",
            };
            danmakuController.FireDanmaku(texts[Random.Range(0, texts.Count)]);
        }
        actionTime = Time.time;
    }
    private int disconnectToyCount = 0;
    public void DisconnectToy()
    {
        disconnectToyCount++;
        List<string> texts = new List<string>
            {
                "可惜可惜",
                "就差一点点啊"
            };
        danmakuController.FireDanmaku(texts[Random.Range(0, texts.Count)]);
    }
}
