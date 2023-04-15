using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerControl : MonoBehaviour
{
    public Camera Camera;
    public GameObject Map;
    public Transform CameraTarget;

    public Vector3 CameraOffset;

    public float Speed;
    public float JumpForce;

    public bool CanJump = true;
    public bool isJump = false;
    public bool isRun = false;
    public bool isLive = true;
    public bool isGround = true;

    private bool isRotateR = false;
    private bool isRotateL = false;
    private float? last_mouse_pos = null;
    private float? mouse_up_position = null;
    private float? mouse_down_pos = null;
    private int angle_rotate = 90;

    private Animator Animator;
    private Rigidbody Rigidbody;
    public ChankControl ChankNow;
    //Sets Physycs and anim fields
    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    /// <summary>
    /// update is makes something actions every frame
    /// </summary>
    private void Update()
    {
        if (Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out RaycastHit hitC, 3f))
            ChankNow = hitC.collider.GetComponent<ChankControl>();
        KeyManager();
        if (Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out RaycastHit hit, 10f))
        {
            if (isLive)
                isRun = true;
            isGround = true;
            isJump = false;
        }
        else isGround = false;
        if (isRun && isGround)
        {
            Rigidbody.velocity = transform.forward * Speed;
        }
        if (last_mouse_pos != null)
        {
            if (ChankNow.type == ChankControl.Ttype.Floor)
            {
                moveXXX();
            }
        }
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
    /// <summary>
    /// roatate player
    /// </summary>
    void rotate()
    {
        
        if (mouse_up_position < mouse_down_pos)
            {
            ChankNow.WeRot = true;
            transform.Rotate(Vector3.up, angle_rotate *-1) ;
            if (isRotateL == true & isRotateR == true)
            {
                isRotateR = !isRotateR;
            }
            else
            {
                isRotateL= !isRotateL;
            }


            Debug.Log("L" + isRotateL + "L  R" + isRotateR);
        }
        if (mouse_up_position > mouse_down_pos)
        {
            ChankNow.WeRot = true;
            transform.Rotate(Vector3.up, angle_rotate);
            if(isRotateL == true & isRotateR == true)
            {
                isRotateL = !isRotateL;
            }
            else
            {
                isRotateR = !isRotateR;
            }
            
            Debug.Log("R" + isRotateL + " " + isRotateR);

        }
        Debug.Log(mouse_up_position + "U  L" + mouse_down_pos);
    }
    /// <summary>
    /// move player on axis by mouse
    /// </summary>
    void moveXXX()
    {
        float difference;
        Vector3 now_vector;
        difference = (Input.mousePosition.x - last_mouse_pos.Value);
      
       if (isRotateL== true & isRotateR == false)
        {
          now_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + (difference / 188) );

        }
        else if(isRotateL == false &  isRotateR == true)
        {
                now_vector = new Vector3(transform.position.x, transform.position.y, transform.position.z + (difference / 188)*-1 );
                
        }
        else if (isRotateR == true & isRotateL == true)
        {
            now_vector = new Vector3(transform.position.x + (difference / 188), transform.position.y, transform.position.z);
        }
        else 
        {
            now_vector = new Vector3(transform.position.x + (difference / 188), transform.position.y, transform.position.z);
        }
                //TODO:Fix rare invers 
       transform.position = now_vector;
        last_mouse_pos = Input.mousePosition.x ;
    
    }
    /// <summary>
    /// check buttons events
    /// </summary>
    void KeyManager()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump(); 
        if (Input.GetMouseButtonDown(0))
        {           
            last_mouse_pos = Input.mousePosition.x;         
        }
        else if (Input.GetMouseButtonUp(0))
        {
            last_mouse_pos = null;
        }
    }
    /// <summary>
    /// Update actions something like 50 times per second
    /// </summary>
    private void FixedUpdate()
    {
        Animator.SetBool("Run", isRun);
        Animator.SetBool("Die", !isLive);
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

}

