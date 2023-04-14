using System.Collections;
using System.Collections.Generic;
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

    private Animator Animator;
    private Rigidbody Rigidbody;
    private ChankControl ChankNow;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out RaycastHit hitC, 3f))
        ChankNow = hitC.collider.GetComponent<ChankControl>();
        KeyManager();
        if (Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out RaycastHit hit, 10f))
        {
            if(isLive)
                isRun = true;
            isGround = true;
            isJump = false;
        } else isGround = false;
        if (isRun && isGround)
        {
            Rigidbody.velocity = transform.forward * Speed;
        }
    }
    private void KeyManager()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        switch (ChankNow.type)
        {
            case ChankControl.Ttype.Pivot:
                CanJump = false;
                if (!ChankNow.WeRot)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        ChankNow.WeRot = true;
                        transform.Rotate(Vector3.up,-90);
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        ChankNow.WeRot = true;
                        transform.Rotate(Vector3.up, 90);
                    }
                }
                break;
            case ChankControl.Ttype.Floor:
                CanJump = true;
                break;
            default: break;
        }

    }
    private void FixedUpdate()
    {
        Animator.SetBool("Run", isRun);
        Animator.SetBool("Die", !isLive);
    }
    private void LateUpdate()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position,transform.position + transform.TransformVector(CameraOffset),
            Time.deltaTime*3f);
        Camera.transform.rotation = Quaternion.LookRotation(CameraTarget.transform.position - Camera.transform.position);
    }
    private void Jump() {
        if (!CanJump) return; 
        if (isJump) return;
        isRun = false;
        isJump = true;
        isGround = false;
        Animator.SetTrigger("Jump");
        Rigidbody.AddForce(100 * JumpForce * Vector3.up, ForceMode.Impulse);
    }
}
