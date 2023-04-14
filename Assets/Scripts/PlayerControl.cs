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
    public int Line = 1;
    private ChankControl ChankNow;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        KeyManager();
        if (isRun)
        {
            Rigidbody.velocity = transform.forward * Speed;
        }
    }
    private void KeyManager()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        Ray Ray = new Ray(transform.position + Vector3.up * 2, Vector3.down);
        if (Physics.Raycast(Ray, out RaycastHit hit, 10f))
        {
            ChankNow = hit.collider.GetComponent<ChankControl>();
            switch (ChankNow.type) {
                case ChankControl.Ttype.Pivot:
                    if (!ChankNow.WeRot)
                    {
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            ChankNow.WeRot = true;
                            transform.Rotate(Vector3.up, -90);
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            ChankNow.WeRot = true;
                            transform.Rotate(Vector3.up, 90);
                        }
                    }
                    break;
                case ChankControl.Ttype.Floor:
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        ChangeLine(ChankNow, true);
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        ChangeLine(ChankNow, false);
                    }
                    break;
                default: break;
            }

        }

    }
    private void ChangeLine(ChankControl chank, bool left)
    {
        if (left) Line--;
        else Line++;
        if (Line < 0) { Line = 0; return; };
        if (Line > 2) { Line = 2; return; };
        if (left)transform.position -= transform.right * 5;
        else transform.position += transform.right * 5;
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
