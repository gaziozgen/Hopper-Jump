using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private bool isFirstBall = false;
    [SerializeField] private float gravityMultiplier = 1;
    [SerializeField] private float dashAcceleration = 6;
    [SerializeField] private Transform mesh = null;
    [SerializeField] private float squishedOffset = 0.7f;
    [SerializeField] private float antiErrorOffset = 0.01f;

    [HideInInspector] public Transform _transform = null;
    private float gravity = -9.8f;
    private bool killed = false;
    private bool waitingToSendFeedback = false;

    // sets with jump function
    private bool isJumping = false; // false with stop
    private bool willJump = false; // false with jump
    private float lastJumpTime;
    private float initialUpVelocity;
    private float startJumpHeight;
    private float floorHeight = 0;
    private float orderInStack;
    private float extraDashForce = 0;

    // sets with dash function
    private bool isDashing = false; // false with stop

    void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (isJumping && !killed)
        {
            if (isDashing)
            {
                extraDashForce += Time.deltaTime * dashAcceleration;
            }

            float time = Time.time - lastJumpTime;
            float currentJumpHeight = ((initialUpVelocity + gravity * gravityMultiplier * time / 2) * time) - (extraDashForce * extraDashForce);
            float nextHeight;

            float currentSpeed = initialUpVelocity + gravity * gravityMultiplier * (Time.time - lastJumpTime);
            if (startJumpHeight + currentJumpHeight + antiErrorOffset < floorHeight + (orderInStack * squishedOffset) && currentSpeed < 0)
            {
                nextHeight = floorHeight + orderInStack * squishedOffset;
                Squish();
                isJumping = false;
                isDashing = false;
            }
            else
            {
                nextHeight = startJumpHeight + currentJumpHeight;
            }

            //_transform.localPosition = Vector3.Lerp(_transform.localPosition, Vector3.up * nextHeight, Time.fixedDeltaTime * 100);
            _transform.localPosition = Vector3.up * nextHeight;
        }
    }

    public void JumpCommand(float latecy, float startJumpHeight, int orderInStack, float jumpHeight)
    {
        willJump = true;
        this.lastJumpTime = Time.time + latecy;
        StartCoroutine(Jump(latecy, startJumpHeight, orderInStack, jumpHeight));
    }

    private IEnumerator Jump(float time ,float startJumpHeight, int orderInStack, float jumpHeight)
    {
        yield return new WaitForSeconds(time);

        if (waitingToSendFeedback)
        {
            waitingToSendFeedback = false;
            Player.Instance.ContinueMove();
        }
        
        Release();
        this.startJumpHeight = startJumpHeight;
        this.orderInStack = orderInStack;
        initialUpVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity * gravityMultiplier);
        extraDashForce = 0;
        willJump = false;
        isJumping = true;
    }

    public void DashToFloor()
    {
        if (isJumping)
        {
            isDashing = true;
        }
    }

    public void UpdateFloorHeight(float newFloorHeight)
    {
        floorHeight = newFloorHeight;
        if (!(isJumping || willJump))
        {
            _transform.localPosition = Vector3.up * (floorHeight + orderInStack * squishedOffset);
        }
    }

    public void UpdateOrderFromEndInStack(int newOrder)
    {
        orderInStack = newOrder;
    }

    public void Squish()
    {
        LeanTween.cancel(mesh.gameObject);
        mesh.LeanScale(new Vector3(1.3f, 0.7f, 1.3f), 0.1f).setOnComplete(() =>
        {
            if (isFirstBall)
            {
                Player.Instance.Jump();
            }
        });
    }

    public void Release()
    {
        LeanTween.cancel(mesh.gameObject);
        mesh.LeanScale(Vector3.one, 0.1f);
    }

    public float GetJumpState()
    {
        float currentSpeed = initialUpVelocity + gravity * gravityMultiplier * (Time.time - lastJumpTime);
        if (currentSpeed > 0)
        {
            return 1 - (currentSpeed / initialUpVelocity);
        }
        else
        {
            return 1;
        }
    }

    public void InstantSquish()
    {
        mesh.localScale = new Vector3(1.3f, 0.7f, 1.3f);
    }

    public void Kill()
    {
        //Debug.Log("destoy", this);
        killed = true;
        _transform.SetParent(null);
        Release();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(Random.Range(-300f, 300f), 0, -300);
        LeanTween.delayedCall(2f, () =>
        {
            Destroy(gameObject);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollectableBall") && other.GetComponent<CollectableBall>().GetBall())
        {
            Player.Instance.AddBall();
        }
    }

    public bool IsReadeyForLowerFloor()
    {
        if (!isJumping)
        {
            waitingToSendFeedback = true;
        }
        return isJumping;
    }

}
