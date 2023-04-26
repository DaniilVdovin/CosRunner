using System.Runtime.CompilerServices;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerControl : MonoBehaviour
{
    [Header("Controls")]
    public Vector3 CameraOffset;
    [Range(10, 100)]
    public float Speed;
    [Range(1, 100)]
    public float JumpForce = 2;
    [Space(10)]
    [Header("Points")]
    public int Coins = 0;
    public float Score = 0;
    public float Oxygen = 100f;
    [Space(10)]
    [Header("Statys")]
    public bool CanJump = true;
    public bool isRun = false;
    public bool isLive = false;
    public bool isGround = true;
    public bool CameraFlow = false;
    public bool isShield,isMagnit;

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
    public GameUI GameUI;
    public ExtraItem ShieldMenu;

    public float ShieldCounddown = 0f;
    private float? last_mouse_pos = null;
    private Vector3 mouse_down_pos;
    private int angle_rotate = 90;
    private bool isBreath = false;
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
    public void StartGame()
    {
        isLive = true;
        CameraFlow = true;

        GameUI.StartGame();
    }
    /// <summary>
    /// update is makes something actions every frame
    /// </summary>
    private void Update()
    {
        if (isLive)
        {
            KeyManager();
            SetPlayerParameters();
            if (isBreath == true)
                Breath();
            OnRotate();
            Autorunning();
            Clamp();
            if (isRun)
            {
                Run();
                UIUpdate();
                SideMove();
                Shieldet();
                Jump();
                SpeedUp();
            }


            if (isShield)
            {

                if (CheckRaycastHit(out RaycastHit Right, out RaycastHit Left))
                    if (Right.collider.CompareTag("Danger") || Left.collider.CompareTag("Danger"))
                    {
                        DestroyObject(Left.collider.gameObject);
                        if (ShieldMenu != null)
                        {
                            ShieldMenu.Close();
                            isShield = false;
                        }
                    }
                    else if (Right.collider.CompareTag("Map_rot") || Left.collider.CompareTag("Danger"))
                    {
                        transform.position = Vector3.back * 3f;
                        isShield = false;
                        ShieldMenu.Close();
                    }

            }
            else
            {

            }
        }

    }
    /// <summary>
    /// Update actions something like 50 times per second
    /// </summary>
    private void FixedUpdate()
    {
        Animator.SetBool("Run", isRun);
        Animator.SetBool("Die", !isLive);
        if (isRun && GetAverageVelosity() > 1)
        {
            Score += 0.01f * Speed;
            Oxygen -= 0.01f;
        }
        if (isLive)
        {
            if (CheckRaycastHit(out RaycastHit ht, out RaycastHit hs)
                && !ht.collider.CompareTag("Item")
                && !hs.collider.CompareTag("Item")
                || Oxygen <= 0)
                Die();
        }
    }
    /// <summary>
    /// last update func called when other update is make their deal
    /// </summary>
    private void LateUpdate()
    {
        if (CameraFlow)
        {
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, transform.position + transform.TransformVector(CameraOffset),
                Time.deltaTime * 3f);
        }
        Camera.transform.rotation = Quaternion.LookRotation(CameraTarget.transform.position - Camera.transform.position);
    }
    private GameObject getPlayerShield()
    {
        return this.transform.Find("PlayerShield").gameObject;
    }
    private GameObject getPlayerMagnit()
    {
        return this.transform.Find("PlayerMagnit").gameObject;
    }
    private bool CheckRaycastHit(out RaycastHit hiR, out RaycastHit hiL)
    {
        bool door = RaycastConfigure(transform.position + Vector3.up * 2 + transform.forward, 3f, out RaycastHit hitName, transform.forward);
        bool boy = RaycastConfigure(transform.position + Vector3.up * 2 + transform.forward, 3f, out RaycastHit hasName, transform.forward);

        if (door && boy)
        {
            hiR = hitName;
            hiL = hasName;
            return true;
        }
        hiR = hitName;
        hiL = hasName;
        return door && boy;
    }
    private void SpeedUp()
    {
        Speed += Time.deltaTime / 100;
        if (Speed == 100)
            Speed = 100;
    }
    private void DestroyObject(GameObject DestructibilityObject)
    {
        Destroy(DestructibilityObject);
    }
    private void Breath()
    {
        Oxygen -= Time.deltaTime * 1.6f;
        if (Oxygen <= 0)
        {
            Die();
        }

    }


    public void Shieldet()
    {
        getPlayerShield().SetActive(isShield);
        getPlayerMagnit().SetActive(isMagnit);
        
    }

    private void UIUpdate()
    {
        GameUI.SetCoinsAndScore(this,Coins, Score);
        GameUI.SetOxygen(Oxygen);
    }
    private void Die()
    {

        if (isShield == false)
        {

            {
                Destroy(Instantiate(Boom, transform.position + Vector3.up * 4, Quaternion.identity), 4f);
                Rigidbody.velocity = Vector3.zero;
                isLive = false;
                isRun = false;
                GameUI.Die();
            }
        }

    }

    private ChankControl takenextChunk(GameObject chanknow_gameobject, out ChankControl chank_now)
    {
        int now = MapGenerator.Map.LastIndexOf(chanknow_gameobject);
        ChankControl chank_now = MapGenerator.Map[now].GetComponent<ChankControl>();
        ChankControl chank_next = MapGenerator.Map[now + 1].GetComponent<ChankControl>();
        return chank_next;
    }


    public void PreRessurect()
    {
        ChankControl nowChank = takenextChunk(ChankNow.gameObject);

        if (nowChank.type == ChankControl.Ttype.Pivot) chank_next = MapGenerator.Map[now + 2].GetComponent<ChankControl>(); ;
        chank_next.Clear();
        Transform chankPoint = chank_next.transform;
        while (chankPoint.rotation.y != transform.rotation.y)
            Rotate(chank_now.transform.rotation.y <= 0 & chank_now.transform.rotation.y > -91);
        transform.position = chankPoint.position;
        isLive = true;
    }

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
    //TODO: FIX GODMOD
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
        {
            ChankNow = ht.collider.GetComponent<ChankControl>();
            isGround = true;
        }
        
        else isGround = false;
    }
    private void OnRotate()
    {
        if (ChankNow != null && ChankNow.type == ChankControl.Ttype.Pivot && !ChankNow.WeRot)
        {
            if (Input.GetMouseButtonDown(0))
                mouse_down_pos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
                Rotate(Input.mousePosition.x > mouse_down_pos.x);
        }
    }
    private void SideMove()
    {
        if (ChankNow != null && isRun && last_mouse_pos is not null&& ChankNow.type != ChankControl.Ttype.Pivot)
        {
            float difference = (Input.mousePosition.x - last_mouse_pos.Value) / 30;
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
    private void Run()
    {
        isBreath = true;
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
        transform.Rotate(Vector3.up, angle_rotate * (!left ? -1 : 1));
        if (isRotateL == true & isRotateR == true)
            if (!left) isRotateR = !isRotateR;
            else isRotateL = !isRotateL;
        else
             if (left) isRotateR = !isRotateR;
        else isRotateL = !isRotateL;
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
        //fix flying
        if (CanJump == false && isRun == true) return;
        int offset = 30;
        if (isGround == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouse_down_pos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if ((Input.mousePosition.y > mouse_down_pos.y) && ((Input.mousePosition.x - mouse_down_pos.x) <= offset) && (mouse_down_pos.x - Input.mousePosition.x <= offset))
                {


                    // Vector3 force = 120 * JumpForce * Vector3.up;
                    // Rigidbody.AddForce(force, ForceMode.Impulse);
                    Animator.SetTrigger("Jump");

                    isGround = false;

                }
            }


               

            

        }
        return;
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemControl>().Get(this);
        }
        
    }

    private float GetAverageVelosity()
        => Mathf.Abs(Rigidbody.velocity.x) + Mathf.Abs(Rigidbody.velocity.z);
    private bool RaycastConfigure(float duration, out RaycastHit hit)
        => Physics.Raycast(new Ray(transform.position + Vector3.up * 2, Vector3.down), out hit, duration);
    private bool RaycastConfigure(Vector3 start, float duration, out RaycastHit hit, Vector3 direction)
        => Physics.Raycast(new Ray(start, direction), out hit, duration);
    private float ClampValue(float value)
        => Mathf.Clamp(value, -ClampOffset, ClampOffset);
}

