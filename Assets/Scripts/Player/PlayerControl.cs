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

    private bool isRotate = false;
    private float? last_mouse_pos = null;
    private float? mouse_up_position = null;

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
            if (ChankNow.type ==ChankControl.Ttype.Floor)
            {
                moveXXX();
            }
            if(ChankNow.type == ChankControl.Ttype.Pivot )
            {
                rotate();
            }
            
        }
        

    }
    /// <summary>
    /// roatate player
    /// </summary>
    void rotate()
    {
        //??? bugs
        if (mouse_up_position < last_mouse_pos)
        {
            ChankNow.WeRot = true;
            transform.Rotate(Vector3.up, -90);
            isRotate = !isRotate;
       
        }
        if (mouse_up_position > last_mouse_pos)
        {
            ChankNow.WeRot = true;
            transform.Rotate(Vector3.up, 90);
            isRotate = !isRotate;
            
        }
        mouse_up_position = null;

    }
    /// <summary>
    /// move player on axis by mouse
    /// </summary>
    void moveXXX()
    {
        float difference;
        Vector3 now_vector;
        difference = (Input.mousePosition.x - last_mouse_pos.Value);
        if (isRotate)
        {
            //TODO:Fix rare inverse
            now_vector = new Vector3(transform.position.x , transform.position.y, transform.position.z + (difference / 188)*-1);

        }
        else
        {

            now_vector = new Vector3(transform.position.x + (difference / 188), transform.position.y, transform.position.z);
        }
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
            Debug.Log(mouse_up_position+"u");
            last_mouse_pos = Input.mousePosition.x;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(last_mouse_pos+"L");
            mouse_up_position = Input.mousePosition.x;
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

