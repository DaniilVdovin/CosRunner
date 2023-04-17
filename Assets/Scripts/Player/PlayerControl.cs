using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerControl : MonoBehaviour
{
    [Header("Controls")]
    public Camera Camera;
    public GameObject Map;
    public Transform CameraTarget;

    public Vector3 CameraOffset;

    [Range(10,100,order = 5)]
    public float Speed;

    [Range(1, 100)]
    public float JumpForce;
    [Space(10)]
    [Header("Points")]
    public int Coins = 0;
    public float Score = 0;
    [Space(10)]
    [Header("Statys")]
    public bool CanJump = true;
    public bool isJump = false;
    public bool isRun = false;
    public bool isLive = true;
    public bool isGround = true;
    [Tooltip("goodmod")]
    public bool godMod = true;

    private bool isRotateR = false;
    private bool isRotateL = false;
    private float? last_mouse_pos = null;
    private float? mouse_up_position = null;
    private float? mouse_down_pos = null;
    private int angle_rotate = 90;
    private Vector3 data;

    private Animator Animator;
    private Rigidbody Rigidbody;
    [HideInInspector]
    public ChankControl ChankNow;
    //Sets Physycs and anim fields
    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Coins++;
            Destroy(other.gameObject);
        }
    }
    /// <summary>
    /// update is makes something actions every frame
    /// </summary>
    private void Update()
    {
        KeyManager();
        SetPlayerParameters();
        run();
        SideMove();
        RotateOn();
        Autorunning();
    }


    private void Autorunning()
    {


        if (godMod is true)
        {
            if (ChankNow.WeRot == false)
            {
                if (ChankNow.type == ChankControl.Ttype.Pivot)
                {
                    
                    var dis = Vector3.Distance(transform.position, ChankNow.transform.position);
                    if (Vector3.Distance(transform.position, ChankNow.transform.position) <= 2)
                    {
                        if (ChankNow.transform.rotation.y <= 0 & ChankNow.transform.rotation.y > -91)
                        {
                            ChankNow.WeRot = true;
                            transform.Rotate(Vector3.up, angle_rotate);
                            if (isRotateL == true & isRotateR == true)
                            {
                                isRotateR = !isRotateR;
                            }
                            else
                            {
                                isRotateL = !isRotateL;
                            }
                            Debug.Log($"L:{isRotateL} | R:{isRotateR}");
                        }
                        else
                        {
                            ChankNow.WeRot = true;
                            transform.Rotate(Vector3.up, angle_rotate * -1);
                            if (isRotateL == true & isRotateR == true)
                            {
                                isRotateR = !isRotateR;
                            }
                            else
                            {
                                isRotateL = !isRotateL;
                            }
                            Debug.Log($"L:{isRotateL} | R:{isRotateR}");
                        }

                    }
                }
            }

        }
    }

    private void SetPlayerParameters()
    {
        if (RaycastConfigure(3f, out RaycastHit ht))
            ChankNow = ht.collider.GetComponent<ChankControl>();

        if (RaycastConfigure(3f, out RaycastHit _))
        {
            isGround = true;
            isJump = false;
        }
        else isGround = false;
    }
    private void RotateOn()
    {

        if (ChankNow.type == ChankControl.Ttype.Pivot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouse_down_pos = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouse_up_position = Input.mousePosition.x;
                rotate();
                mouse_down_pos = 0;
                mouse_up_position = 0;
            }
        }
    }
    private void SideMove()
    {
        if (last_mouse_pos != null)
        {
            if (ChankNow.type == ChankControl.Ttype.Floor)
            {
                moveXXX();
            }
        }
    }
    private bool RaycastConfigure(float duration, out RaycastHit hit) => Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out hit, duration);
    private void run()
    {
        if (isRun && isGround)
        {
            Rigidbody.velocity = transform.forward * Speed;
        }
    }
    /// <summary>
    /// roatate player
    /// </summary>
    private void rotate()
    {
        if (ChankNow.WeRot == false)
        {
            if (mouse_up_position < mouse_down_pos)
            {
                ChankNow.WeRot = true;
                transform.Rotate(Vector3.up, angle_rotate * -1);
                if (isRotateL == true & isRotateR == true)
                {
                    isRotateR = !isRotateR;
                }
                else
                {
                    isRotateL = !isRotateL;
                }
                Debug.Log($"L:{isRotateL} | R:{isRotateR}");
            }
            if (mouse_up_position > mouse_down_pos)
            {
                ChankNow.WeRot = true;
                transform.Rotate(Vector3.up, angle_rotate);
                if (isRotateL == true & isRotateR == true)
                {
                    isRotateL = !isRotateL;
                }
                else
                {
                    isRotateR = !isRotateR;
                }

                Debug.Log($"R:{isRotateR} | L:{isRotateL} ");

            }
            Debug.Log($"{mouse_up_position} U | L {mouse_down_pos} ");
        }
    }
    /// <summary>
    /// move player on axis by mouse
    /// </summary>
    private void moveXXX()
    {
        float difference;
        Vector3 now_vector;
        difference = (Input.mousePosition.x - last_mouse_pos.Value);
//      var mp = Camera.ScreenToWorldPoint(data); // new way
        Vector3 test = transform.position;
        if (isRotateL == true & isRotateR == false)
        {

            now_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + Mathf.Clamp((difference / 30), -20, 20));

        }
        else if (isRotateL == false & isRotateR == true)
        {
            now_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + Mathf.Clamp((difference/30 ), -20, 20) * -1);

        }
        else  
        {
            now_vector = new Vector3(transform.position.x + Mathf.Clamp((difference / 150), -20, 20), transform.position.y, transform.position.z);
           
        }


        //test.z = (Mathf.Lerp(transform.localPosition.z, mp.x, Time.deltaTime * 1));
        //transform.position = test;
        //data = Input.mousePosition;
        transform.position = now_vector;
       
        last_mouse_pos = Input.mousePosition.x;

    }
    /// <summary>
    /// check buttons events
    /// </summary>
    private void KeyManager()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetMouseButtonDown(0))
        {
            data = Input.mousePosition;
            last_mouse_pos = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            
            last_mouse_pos = null;
        }
    }
    /// <summary>
    /// jump func make body up
    /// </summary>
    private void Jump()
    {
        if (!CanJump) return;
        if (isJump) return;
        isRun = false;
        isJump = true;
        isGround = false;
        Animator.SetTrigger("Jump");
        Rigidbody.AddForce(100 * JumpForce * Vector3.up, ForceMode.Impulse);
    }
    /// <summary>
    /// Update actions something like 50 times per second
    /// </summary>
    private void FixedUpdate()
    {
        Animator.SetBool("Run", isRun);
        Animator.SetBool("Die", !isLive);
        if (isRun && GetAverageVelosity()>1)
        {
            Score += 0.01f;
        }
    }
    /// <summary>
    /// last update func called when other update is make their deal
    /// </summary>
    private void LateUpdate()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, transform.position + transform.TransformVector(CameraOffset),
            Time.deltaTime * 3f);
        Camera.transform.rotation = Quaternion.LookRotation(CameraTarget.transform.position - Camera.transform.position);
    }

    private float GetAverageVelosity() => Mathf.Abs(Rigidbody.velocity.x) + Mathf.Abs(Rigidbody.velocity.z);

}

