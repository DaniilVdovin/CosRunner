using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerControl : MonoBehaviour
{
    public Camera Camera;
    public Transform CameraTarget;
    public Vector3 CameraOffset;
    public float Speed;
    public bool CanJump = true;
    public bool isJump = false;
    public bool isRun = false;
    public bool isLive = true;

    private Animator Animator;
    private Rigidbody Rigidbody;
    private int Line = 1;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        KeyManager();
        if (isRun)
            Rigidbody.velocity = transform.forward * Speed;


    }
    private void KeyManager()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        Ray Ray = new Ray(transform.position + Vector3.up * 2, Vector3.down);
        if (Physics.Raycast(Ray, out RaycastHit hit, 100f))
        {
            var chank = hit.collider.GetComponent<ChankControl>();
            switch (chank.type) {
                case ChankControl.Ttype.Pivot:
                    if (!chank.WeRot)
                    {
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            chank.WeRot = true;
                            transform.Rotate(Vector3.up, -90);
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            chank.WeRot = true;
                            transform.Rotate(Vector3.up, 90);
                        }
                    }
                    break;
                case ChankControl.Ttype.Floor:
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        ChangeLine(chank, Line--);
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        ChangeLine(chank, Line++);
                    break;
                default:break;
            }
            
        }
       
    }
    private void ChangeLine(ChankControl chank, int Line)
    {
        if (Line < 0) Line = 0;
        if (Line > 2) Line = 2;
        Vector3 temp = chank.Lines[Line].localPosition;
        temp.z = transform.position.z;
        transform.position = temp;
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
        Animator.SetTrigger("Jump");
    }
}
