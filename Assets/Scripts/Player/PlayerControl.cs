using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    [Header("Controls")]
    public Vector3 CameraOffset;
    [Range(10, 100)]
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

    [Space(10)]
    [Header("Inversing")]
    public bool isRotateR = false;
    public bool isRotateL = false;
    [Space(10)]
    [Header("Clamp")]
    public float ClampOffset = 7f;
    [Space(10)]
    [Header("Effects")]
    public GameObject Boom;

    [Space(10)]
    [Header("System")]

    public Camera Camera;
    public GameObject Map;
    public Transform CameraTarget;
    public Generate MapGenerator;



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
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemControl>().Get(this);
        }
    }
    /// <summary>
    /// update is makes something actions every frame
    /// </summary>
    private void Update()
    {
        KeyManager();
        SetPlayerParameters();
        Run();
        SideMove();
        OnRotate();
        Autorunning();
        Clamp();
    }
    private void Die()
    {
        if(isLive)
        {
            if (RaycastConfigure(transform.position + Vector3.up * 2 + transform.forward, 3f, out RaycastHit ht, transform.forward) && ht.collider.CompareTag("Coin") != true)
            {
                Instantiate(Boom, transform.position + Vector3.up * 4,Quaternion.identity);
                Debug.Log("Die");
                isLive = false;
                isRun = false;

            }
        }
    }
    //TODO: Function back life in next chank
    private void Clamp()
    {
        if (ChankNow != null)
        {
            Vector3 vector = transform.position;
            Vector3 chankPoint = ChankNow.transform.position;
            if (isRotateL == isRotateR)
                vector.x = chankPoint.x + ClampValue(vector.x - chankPoint.x);
            else vector.z = chankPoint.z + ClampValue(vector.z - chankPoint.z);
            transform.position = vector;
        }
    }
    private void Autorunning()
    {
        if (ChankNow != null && ChankNow.type == ChankControl.Ttype.Pivot)
        {
            Transform chankPoint = ChankNow.transform;
            if (godMod && !ChankNow.WeRot
                && Vector3.Distance(transform.position, chankPoint.position) <= 2)
                Rotate(chankPoint.rotation.y <= 0 & chankPoint.rotation.y > -91);
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
    private void OnRotate()
    {
        if (ChankNow != null && ChankNow.type == ChankControl.Ttype.Pivot && !ChankNow.WeRot)
        {
            if (Input.GetMouseButtonDown(0))
                mouse_down_pos = Input.mousePosition.x;
            if (Input.GetMouseButtonUp(0))
                Rotate(Input.mousePosition.x > mouse_down_pos);
        }
    }
    private void SideMove()
    {
        if (isRun)
        {
            if (last_mouse_pos != null)
            {
                if (ChankNow.type == ChankControl.Ttype.Floor)
                {
                    float difference = (Input.mousePosition.x - last_mouse_pos.Value)/30;
                    Vector3 new_vector = transform.position;
                    if (isRotateL == isRotateR) new_vector.x += difference;
                    else
                    {
                        if (isRotateL & !isRotateR) new_vector.z += difference;
                        else new_vector.z += -difference;
                    }
                    transform.position = new_vector;
                    last_mouse_pos = Input.mousePosition.x;
                }
            }
        }
       
    }
    private void Run()
    {
        if (isRun && isGround)
        {
            Rigidbody.velocity = transform.forward * Speed;
        }
    }
    /// <summary>
    /// roatate player
    /// </summary>
    private void Rotate(bool left)
    {
        transform.Rotate(Vector3.up, angle_rotate * (!left?-1:1));
        if (isRotateL == true & isRotateR == true)
             if(!left) isRotateR = !isRotateR;
             else      isRotateL = !isRotateL;
        else
             if (left) isRotateR = !isRotateR;
             else      isRotateL = !isRotateL;
        ChankNow.WeRot = true;
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
        Die();
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, transform.position + transform.TransformVector(CameraOffset),
            Time.deltaTime * 3f);
        Camera.transform.rotation = Quaternion.LookRotation(CameraTarget.transform.position - Camera.transform.position);
    }

    private float GetAverageVelosity()
        => Mathf.Abs(Rigidbody.velocity.x) + Mathf.Abs(Rigidbody.velocity.z);
    private bool RaycastConfigure(float duration, out RaycastHit hit)
    {
       return Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out hit, duration);
    }
    private bool RaycastConfigure(Vector3 start,float duration, out RaycastHit hit, Vector3 direction)
    {
       return Physics.Raycast(new Ray(start, direction), out hit, duration);
    }

    private float ClampValue(float value)
        => Mathf.Clamp(value, -ClampOffset, ClampOffset);
}

