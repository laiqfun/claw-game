using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    public GameManager gameManager;
    public AudioController audioController;
    public float offset = 2f;
    [Tooltip("对齐方向(默认False向左对齐,True为向右对齐)")]
    public bool alignDirection = true;
    public Transform joystickControl;
    public Transform buttonPlunger;
    public Transform coin;
    public bool is_press_button { get; private set; } = false;
    public float horizontalInput { get; private set; } = 0;
    public float verticalInput { get; private set; } = 0;
    private Animation coinAnim;
    private bool is_insertCoin = false;
    void Awake()
    {
        uint i = 1;
        bool found = true;
        do
        {
            Transform Button = transform.Find("Button Item (" + i + ")");
            if (!Button)
            {
                found = false;
                continue;
            }
            Button.localPosition = new Vector3((i - 1) * offset * (alignDirection ? -1 : 1), 0, 0);
            i++;
        } while (found);
    }
    // Start is called before the first frame update
    void Start()
    {
        coinAnim = coin.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        PressButton();
        MoveJoystick();
        InsertCoin();
    }

    void PressButton()
    {
        if (buttonPlunger != null)
        {
            const float depth = 0.09f;
            bool is_pressdown_key = Input.GetKeyDown(KeyCode.Space);
            bool is_pressup_key = Input.GetKeyUp(KeyCode.Space);
            if (is_pressdown_key && is_press_button == false)
            {
                buttonPlunger.Translate(new Vector3(0, -depth, 0));
                is_press_button = true;
                gameManager.action();
                audioController.PlayButtonClip();
            }
            if (is_pressup_key && is_press_button == true)
            {
                buttonPlunger.Translate(new Vector3(0, depth, 0));
                is_press_button = false;
                gameManager.action();
            }
        }
    }

    void MoveJoystick()
    {
        if (joystickControl != null)
        {
            // 获取水平和垂直输入轴的数值
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            // 计算输入向量（用于判断方向和强度）
            Vector2 inputVector = new Vector2(horizontalInput, -verticalInput).normalized;
            float inputStrength = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude);

            if (inputStrength > 0) gameManager.action();

            // 1. 计算Y轴旋转角度（控制倾斜方向）
            // 用Atan2将输入方向转换为角度（0~360度），再根据模型适配调整偏移
            float directionAngle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            // 你的模型向左对应-90度，这里需要根据实际方向微调（可能需要±90度校准）
            directionAngle -= 90f; // 关键校准：让输入"左"对应Y轴-90度

            // 2. 计算X轴倾斜角度（控制倾斜幅度）
            // 初始角度-90，最大倾斜30度（输入越强，倾斜越大）
            float tiltAngle = -90f - (inputStrength * 30f);

            // 3. 应用欧拉角（X:倾斜幅度，Y:倾斜方向，Z:固定0）
            joystickControl.rotation = Quaternion.Euler(tiltAngle, directionAngle, 0f);
        }
    }

    void InsertCoin()
    {
        if (coin != null)
        {
            bool is_pressup_key = Input.GetKeyUp(KeyCode.Q);
            if (!gameManager.isInsertCoin && coinAnim != null && coinAnim.isPlaying == false && !is_insertCoin && is_pressup_key)
            {
                coinAnim.Play();
                is_insertCoin = true;
            }
            if (is_insertCoin && coinAnim.isPlaying == false)
            {
                audioController.PlayCoinClip();
                gameManager.InsertCoin();
                gameManager.action();
                is_insertCoin = false;
            }
        }
    }
}
