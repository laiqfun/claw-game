using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DanmakuController : MonoBehaviour
{
    public GameObject danmakuPrefab;
    public float speed = 1f;
    private List<GameObject> danmakus;
    // Start is called before the first frame update
    RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // 初始化弹幕列表
        danmakus = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        DanmakuUpdate();
    }

    float getWidth()
    {
        if (rectTransform)
            return rectTransform.rect.width;
        else
            return 0;
    }
    float getHeight()
    {
        if (rectTransform)
            return rectTransform.rect.height;
        else
            return 0;
    }

    // 发射弹幕
    public void FireDanmaku(string text)
    {
        GameObject Danmaku = Instantiate(
            danmakuPrefab,
            transform
        );
        Danmaku.GetComponent<TextMeshProUGUI>().text = text;
        float range = getHeight() / 2 - 100;
        Danmaku.GetComponent<RectTransform>().anchoredPosition = new Vector2(500, Random.Range(-range, range));

        danmakus.Add(Danmaku);
    }

    void DanmakuUpdate()
    {
        for (int i = 0; i < danmakus.Count; i++)
        {
            GameObject Danmaku = danmakus[i];
            RectTransform DanmakuRect = Danmaku.GetComponent<RectTransform>();
            DanmakuRect.anchoredPosition += new Vector2(-1 * Time.deltaTime * speed, 0);
            if (DanmakuRect.anchoredPosition.x < -getWidth())
            {
                danmakus.RemoveAt(i);
                Destroy(Danmaku);
            }
        }
    }
}