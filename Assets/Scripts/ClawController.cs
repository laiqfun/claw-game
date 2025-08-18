using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClawController : MonoBehaviour
{
    public ButtonsController buttonsController;
    public float speed = 10;
    public float boundX = 5;
    public float boundZ = 5;
    public Transform leftClaw;
    public Transform rightClaw;
    public Vector3 initialPosition;
    private float grabAngle = 20f;
    private bool isGrab = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = initialPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
        grab();
    }

    void move()
    {
        float horizontalInput = buttonsController.horizontalInput;
        float verticalInput = buttonsController.verticalInput;

        Vector3 translate = new Vector3(horizontalInput, 0, verticalInput);

        transform.position = transform.position + translate * Time.deltaTime * speed;

        if (translate.normalized != Vector3.zero)
        {
            //检查是否越界
            checkOverBound();
        }
    }

    void checkOverBound()
    {
        //检查 X 坐标是否越界
        if (transform.position.x > boundX)
        {
            transform.position = new Vector3(boundX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -boundX)
        {
            transform.position = new Vector3(-boundX, transform.position.y, transform.position.z);
        }
        //检查 Z 坐标是否越界
        if (transform.position.z > boundZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, boundZ);
        }
        else if (transform.position.z < -boundZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -boundZ);
        }
    }

    void grab()
    {
        if (buttonsController && buttonsController.is_press_button && !isGrab)
        {
            // isGrab = true;
            
        }
    }
}
