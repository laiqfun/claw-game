using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    public ButtonsController buttonsController;
    public float speed = 10;
    public float grabRotationSpeed = 30;
    public float boundX = 5;
    public float boundZ = 5;
    public Transform leftClaw;
    public Transform rightClaw;
    public Vector3 initialPosition;
    public float grabYPos;
    private float grabAngle = 20f;
    private bool isGrab = false;
    // 动画步骤列表
    private List<IGrabAnimationStep> animationSteps;
    // 当前步骤索引
    private int currentStepIndex = -1;

    void Start()
    {
        transform.position = initialPosition;
        // 初始化动画步骤序列
        InitializeAnimationSteps();
    }

    void FixedUpdate()
    {
        if(!isGrab) move();
        grab();
    }

    void move()
    {
        float horizontalInput = buttonsController.horizontalInput;
        float verticalInput = buttonsController.verticalInput;

        Vector3 translate = new Vector3(horizontalInput, 0, verticalInput);
        transform.position += translate * Time.deltaTime * speed;

        if (translate.normalized != Vector3.zero)
        {
            checkOverBound();
        }
    }

    void checkOverBound()
    {
        if (transform.position.x > boundX)
            transform.position = new Vector3(boundX, transform.position.y, transform.position.z);
        else if (transform.position.x < -boundX)
            transform.position = new Vector3(-boundX, transform.position.y, transform.position.z);

        if (transform.position.z > boundZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, boundZ);
        else if (transform.position.z < -boundZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, -boundZ);
    }

    void grab()
    {
        if (buttonsController && buttonsController.is_press_button && !isGrab)
        {
            isGrab = true;
            // 重置动画步骤
            currentStepIndex = 0;
        }

        if (isGrab && animationSteps != null && currentStepIndex < animationSteps.Count)
        {
            ExecuteCurrentStep();
        }
    }

    // 执行当前步骤
    void ExecuteCurrentStep()
    {
        var currentStep = animationSteps[currentStepIndex];
        currentStep.Execute();

        if (currentStep.IsComplete())
        {
            currentStepIndex++;
            // 所有步骤执行完毕
            if (currentStepIndex >= animationSteps.Count)
            {
                isGrab = false;
                currentStepIndex = -1;
            }
        }
    }

    // 初始化动画步骤（在这里增删步骤实现灵活调整）
    void InitializeAnimationSteps()
    {
        animationSteps = new List<IGrabAnimationStep>
        {
            new DescendStep(this, grabYPos, speed),       // 下降步骤
            new CloseClawStep(this, grabAngle, grabRotationSpeed),    // 闭合爪子步骤
            new AscendStep(this, initialPosition.y, speed),// 上升步骤
            new ResetZStep(this, initialPosition.z, speed),// 重置Z轴步骤
            new ResetXStep(this, initialPosition.x, speed),// 重置X轴步骤
            new OpenClawStep(this, grabRotationSpeed)                 // 打开爪子步骤
        };
    }
}

// 动画步骤接口
public interface IGrabAnimationStep
{
    void Execute();
    bool IsComplete();
}

// 下降步骤
public class DescendStep : IGrabAnimationStep
{
    private ClawController claw;
    private float targetY;
    private float speed;

    public DescendStep(ClawController claw, float targetY, float speed)
    {
        this.claw = claw;
        this.targetY = targetY;
        this.speed = speed;
    }

    public void Execute()
    {
        claw.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        // 限制最低位置
        if (claw.transform.position.y <= targetY)
        {
            claw.transform.position = new Vector3(
                claw.transform.position.x, 
                targetY, 
                claw.transform.position.z
            );
        }
    }

    public bool IsComplete()
    {
        return claw.transform.position.y <= targetY;
    }
}

// 闭合爪子步骤
public class CloseClawStep : IGrabAnimationStep
{
    private ClawController claw;
    private float targetAngle;
    private float speed;

    public CloseClawStep(ClawController claw, float targetAngle, float speed)
    {
        this.claw = claw;
        this.targetAngle = targetAngle;
        this.speed = speed;
    }

    public void Execute()
    {
        float leftAngle = claw.leftClaw.localRotation.eulerAngles.x + speed * Time.deltaTime;
        float rightAngle = claw.rightClaw.localRotation.eulerAngles.x + speed * Time.deltaTime;

        // 限制最大角度
        if (leftAngle >= targetAngle)
        {
            leftAngle = targetAngle;
            rightAngle = targetAngle;
        }

        claw.leftClaw.localRotation = Quaternion.Euler(leftAngle, 0, 0);
        claw.rightClaw.localRotation = Quaternion.Euler(rightAngle, 0, 0);
    }

    public bool IsComplete()
    {
        return claw.leftClaw.localRotation.eulerAngles.x >= targetAngle;
    }
}

// 上升步骤
public class AscendStep : IGrabAnimationStep
{
    private ClawController claw;
    private float targetY;
    private float speed;

    public AscendStep(ClawController claw, float targetY, float speed)
    {
        this.claw = claw;
        this.targetY = targetY;
        this.speed = speed;
    }

    public void Execute()
    {
        claw.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        // 限制最高位置
        if (claw.transform.position.y >= targetY)
        {
            claw.transform.position = new Vector3(
                claw.transform.position.x, 
                targetY, 
                claw.transform.position.z
            );
        }
    }

    public bool IsComplete()
    {
        return claw.transform.position.y >= targetY;
    }
}

// 重置Z轴步骤
public class ResetZStep : IGrabAnimationStep
{
    private ClawController claw;
    private float targetZ;
    private float speed;

    public ResetZStep(ClawController claw, float targetZ, float speed)
    {
        this.claw = claw;
        this.targetZ = targetZ;
        this.speed = speed;
    }

    public void Execute()
    {
        float zd = claw.transform.position.z - targetZ;
        float moveZ = (zd > 0 ? -1 : 1) * speed * Time.deltaTime;
        claw.transform.position += new Vector3(0, 0, moveZ);
    }

    public bool IsComplete()
    {
        return Math.Abs(claw.transform.position.z - targetZ) < 0.1f;
    }
}

// 重置X轴步骤
public class ResetXStep : IGrabAnimationStep
{
    private ClawController claw;
    private float targetX;
    private float speed;

    public ResetXStep(ClawController claw, float targetX, float speed)
    {
        this.claw = claw;
        this.targetX = targetX;
        this.speed = speed;
    }

    public void Execute()
    {
        float xd = claw.transform.position.x - targetX;
        float moveX = (xd > 0 ? -1 : 1) * speed * Time.deltaTime;
        claw.transform.position += new Vector3(moveX, 0, 0);
    }

    public bool IsComplete()
    {
        return Math.Abs(claw.transform.position.x - targetX) < 0.1f;
    }
}

// 打开爪子步骤
public class OpenClawStep : IGrabAnimationStep
{
    private ClawController claw;
    private float speed;

    public OpenClawStep(ClawController claw, float speed)
    {
        this.claw = claw;
        this.speed = speed;
    }

    public void Execute()
    {
        float leftAngle = claw.leftClaw.localRotation.eulerAngles.x - speed * Time.deltaTime;
        float rightAngle = claw.rightClaw.localRotation.eulerAngles.x - speed * Time.deltaTime;

        // 限制最小角度
        if (leftAngle <= 0)
        {
            leftAngle = 0;
            rightAngle = 0;
        }

        claw.leftClaw.localRotation = Quaternion.Euler(leftAngle, 0, 0);
        claw.rightClaw.localRotation = Quaternion.Euler(rightAngle, 0, 0);
    }

    public bool IsComplete()
    {
        return claw.leftClaw.localRotation.eulerAngles.x <= 0;
    }
}