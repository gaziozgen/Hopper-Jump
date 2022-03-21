using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;


public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance { get => instance; }

    [SerializeField] private int targetFrameRate;

    [SerializeField] private GameObject ballPrefab = null;
    [SerializeField] private int extraBallsCount = 2;

    [SerializeField] private float speed = 2;
    [SerializeField] private float minJumpHeight = 2;
    [SerializeField] private float maxJumpHeight = 6;
    [SerializeField] private float jumpHeightChange = 1;
    [SerializeField] private float moveTimeDiff = 0.02f;
    [SerializeField] private float minDashTimeOffset = 0.3f;
    [SerializeField] private float jumpHeightDiffRatio = 0.25f;
    [SerializeField] private float squishedOffset = 0.7f;

    [SerializeField] private Ball myBall = null;
    [SerializeField] private Transform ballParent = null;
    [HideInInspector] public Transform _transform = null;
    private List<Ball> balls = null;

    private float jumpHeight;
    private bool isDashing = false;
    private float dashedJupmRatio;
    private float currentFloorHeight = 0;
    private float nextFloorHeight;
    private float lastJumpTime;
    private bool isEnded = false;
    private float currentSpeed;
    private float currentMinDashTimeLatecy;

    private bool sloping = false;
    private float slopeStartZ;
    private float slopeAim;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = targetFrameRate;
        _transform = transform;
        currentSpeed = speed;
        jumpHeight = minJumpHeight;
        balls = new List<Ball>();
        balls.Add(myBall);

        // extra balls edit
        myBall.transform.localPosition = Vector3.up * squishedOffset * extraBallsCount;
        for (int i = 0; i < extraBallsCount; i++)
        {
            balls.Add(Instantiate(ballPrefab, _transform.position + Vector3.up * squishedOffset * (extraBallsCount - (i + 1)), Quaternion.identity, ballParent).GetComponent<Ball>());
        }

        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].InstantSquish();
        }
        UpdateCameraTraget();
        RecalculateMinDashTime();
    }

    private void Start()
    {
        LeanTween.delayedCall(0.05f, () => // geliþmeli
        {
            Jump();
        });
    }

    void Update()
    {
        if (!isEnded)
        {
            if (Input.GetMouseButton(0) && !isDashing && (Time.time > lastJumpTime + currentMinDashTimeLatecy))
            {
                Dash();
            }
            MoveForward();
        }

        if (sloping) {
            UpdateHeigthsToSlope();
        }
    }

    private void MoveForward()
    {
        _transform.position = _transform.position + Vector3.forward * currentSpeed * Time.deltaTime;
    }

    public void AddBall()
    {
        Vector3 targetPosition = _transform.position;
        targetPosition.y = 0;
        Ball newBall = Instantiate(ballPrefab, targetPosition, Quaternion.identity, ballParent).GetComponent<Ball>();
        balls.Add(newBall);
        newBall.Squish();
        UpdateStackOrder();
    }

    private void Dash()
    {
        isDashing = true;
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].DashToFloor();
        }

        float jumpState = balls[balls.Count - 1].GetJumpState();
        //print("dashed height: " + jumpState.ToString());
        dashedJupmRatio = jumpState;

    }

    public void Jump()
    {
        if (!sloping)
        {
            lastJumpTime = Time.time;
            //float oldJumpHeight = jumpHeight;
            if (isDashing)
            {
                isDashing = false;

                jumpHeight *= dashedJupmRatio;
                jumpHeight += jumpHeightChange;

                if (jumpHeight > maxJumpHeight)
                {
                    jumpHeight = maxJumpHeight;
                }
                else if (jumpHeight < minJumpHeight)
                {
                    jumpHeight = minJumpHeight;
                }
            }
            else
            {
                jumpHeight -= jumpHeightChange;

                if (jumpHeight < minJumpHeight)
                {
                    jumpHeight = minJumpHeight;
                }
            }
            //print("jump high: " + oldJumpHeight.ToString() + " -> " + jumpHeight.ToString());

            float ballJumpHeight;
            int backOrderInStack;
            float ballStartJumpHeight;

            for (int i = 0; i < balls.Count; i++)
            {
                backOrderInStack = (balls.Count - i - 1);
                ballJumpHeight = jumpHeight + backOrderInStack * jumpHeightDiffRatio;
                ballStartJumpHeight = currentFloorHeight + backOrderInStack * squishedOffset;
                balls[i].JumpCommand(i * moveTimeDiff, ballStartJumpHeight, backOrderInStack, ballJumpHeight);
            }
        }
        else
        {
            currentSpeed = speed * 3f;
        }
    }

    private void UpdateHeigthsToSlope()
    {
        float newHeigth = currentFloorHeight - ((_transform.position.z - slopeStartZ) * slopeAim);

        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].UpdateFloorHeight(newHeigth);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            Floor floor = other.GetComponent<Floor>();
            float floorHeight = floor.GetHeight();
            if (currentFloorHeight != floorHeight)
            {
                if (currentFloorHeight > floorHeight && !sloping)
                {
                    if (!balls[balls.Count - 1].IsReadeyForLowerFloor())
                    {
                        //print("wait");
                        currentSpeed = 0;
                        nextFloorHeight = floorHeight;
                    }
                    else
                    {
                        currentFloorHeight = floorHeight;
                        UpdateFloorHeightForBalls(floorHeight);
                    }
                }
                else
                {
                    currentFloorHeight = floorHeight;
                    UpdateFloorHeightForBalls(floorHeight);
                }
            }
            
            if (sloping)
            {
                sloping = false;
                currentSpeed = speed;
                Jump();
            }
        }
        else if (other.CompareTag("Slope"))
        {
            sloping = true;
            Slope slope = other.GetComponent<Slope>();
            slopeAim = slope.GetSlope();
            slopeStartZ = slope._transform.position.z;
        }
    }

    private void UpdateFloorHeightForBalls(float floorHeight)
    {
        for (int i = balls.Count-1; i > -1; i--)
        {
            if (balls[i]._transform.position.y > currentFloorHeight)
            {
                balls[i].UpdateFloorHeight(floorHeight);
            }
            else
            {
                balls[i].Kill();
                balls.RemoveAt(i);
            }
        }

        if (balls.Count > 0)
        {
            UpdateStackOrder();
        }
        else
        {
            isEnded = true;
        }
    }

    private void UpdateStackOrder()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].UpdateOrderFromEndInStack(balls.Count - (i + 1));
        }
        UpdateCameraTraget();
        RecalculateMinDashTime();
    }

    public void ContinueMove()
    {
        //print("ContinueMove");
        currentSpeed = speed;
        currentFloorHeight = nextFloorHeight;
        UpdateFloorHeightForBalls(currentFloorHeight);
    }

    private void UpdateCameraTraget()
    {
        Camera.main.GetComponent<CameraFollow>().Target = balls[balls.Count -1]._transform;
    }

    private void RecalculateMinDashTime()
    {
        currentMinDashTimeLatecy = balls.Count * moveTimeDiff + minDashTimeOffset;
    }
}
