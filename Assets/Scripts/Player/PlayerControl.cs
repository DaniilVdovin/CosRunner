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
    [Space(10)]
    [Header("Statys")]
    public bool CanJump = true;
    public bool isRun = false;
    public bool isLive = false;
    public bool isGround = true;
    public bool CameraFlow = false;
    public bool isShield;

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

    private float ShieldCounddown = 0f;
    private float? last_mouse_pos = null;
    private Vector3 mouse_down_pos;
    private int angle_rotate = 90;

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

        GameUI.ConnectPlayer(this);

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
            Run();
            SideMove();
            OnRotate();
            Autorunning();
            Clamp();
            Jump();
            Shieldet();
            if (isRun)
                UIUpdate();
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
            Score += 0.01f;
        }
        if (isLive)
        {
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
    private bool IsAnimationPlaying(string AnimationName)
    {
        AnimatorStateInfo animatorState = Animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(animatorState.nameHash);
        if (animatorState.IsName(AnimationName))
            return true;
        return false;
    }
    public void Shieldet()
    {
        ShieldCounddown += 10f;
        if (isShield == true)
        {

            GameObject shield = gameObject.transform.Find("PlayerShield").gameObject;
            shield.SetActive(true);
            if (ChankNow != null && ChankNow.WeRot == true)
                ChankNow.WeRot = false;
            while (ShieldCounddown > 0)
            {
                isShield = false;
                ShieldCounddown -= Time.deltaTime;
            }
            shield.SetActive(false);

        }
    }

    private void UIUpdate()
    {
        GameUI.SetCoinsAndScore(Coins, Score);
    }
    private void Die()
    {
        if (!isShield)
        {
            if (RaycastConfigure(transform.position + Vector3.up * 4 + transform.forward, 3f, out RaycastHit ht, transform.forward) && !ht.collider.CompareTag("Item") ||
            RaycastConfigure(transform.position + Vector3.up * 4 + transform.forward, 3f, out RaycastHit hts, transform.forward)
                && !hts.collider.CompareTag("Item"))
            {
                Destroy(Instantiate(Boom, transform.position + Vector3.up * 4, Quaternion.identity), 4f);

                Rigidbody.velocity = Vector3.zero;
                Debug.Log("Die" + RaycastConfigure(transform.position + Vector3.up * 4 + transform.forward, 3f, out RaycastHit htt, transform.forward) + htt.collider.tag + htt.collider.name);
                isLive = false;
                isRun = false;
                GameUI.Die();
            }
        }

    }
    public void PreRessurect()
    {
        int now = MapGenerator.Map.LastIndexOf(ChankNow.gameObject);
        ChankControl chank_now = MapGenerator.Map[now].GetComponent<ChankControl>();
        ChankControl chank_next = MapGenerator.Map[now + 1].GetComponent<ChankControl>();
        if (chank_next.type == ChankControl.Ttype.Pivot) chank_next = MapGenerator.Map[now + 2].GetComponent<ChankControl>(); ;
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
        if (ChankNow != null && isRun && last_mouse_pos is not null)
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
        if (CanJump == false) return;
        int offset = 30;
        if (isGround == true)
        {

            if (Input.GetMouseButtonDown(0))
            {
                mouse_down_pos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {

                
                if ((Input.mousePosition.y > mouse_down_pos.y) && ((Input.mousePosition.x - mouse_down_pos.x) <= offset) && (mouse_down_pos.x - Input.mousePosition.x <= offset))
                {

                    isRun = false;
                    Vector3 force = 220 * JumpForce * Vector3.up;
                    Rigidbody.AddForce(force, ForceMode.Impulse);
                    Animator.SetTrigger("Jump");

                    isGround = false;
                    isRun = true;
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

