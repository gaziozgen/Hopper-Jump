using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;


public class Player : MonoBehaviour
{
    #region hide

    private static Player instance;
    public static Player Instance { get => instance; }

    #endregion

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
    [SerializeField] private float slopeSpeedMultiplier = 3f;
    [SerializeField] private float finalAreaSpeedMultiplier = 2f;

    [SerializeField] private Transform shadowTransform = null;
    [SerializeField] private Ball myBall = null;
    [SerializeField] private Transform ballParent = null;
    [SerializeField] private ParticleSystem splashEffect = null;
    [HideInInspector] public Transform _transform = null;
    private List<Ball> balls = null;

    // general
    private AreaType areaType = AreaType.BASIC;
    private float currentSpeed;
    private bool isEnded = false;
    private bool isStarted = false;
    private float currentFloorHeight = 0;
    private float nextFloorHeight;

    // jump
    private float jumpHeight;
    private float lastJumpTime;

    // dash
    private bool isDashing = false;
    private float dashedJupmRatio;
    private float currentMinDashTimeLatecy;

    // slope
    private float slopeStartZ;
    private float slopeAim;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = targetFrameRate;
        _transform = transform;
        currentSpeed = 0;
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
            balls[i].SetTrailActive(false);
        }
    }

    private void Start()
    {
        UpdateStackOrder();
        Jump();
    }

    void Update()
    {
        if (!isEnded)
        {
            if ((Input.GetMouseButton(0) || areaType == AreaType.FINAL) && !isDashing && (Time.time > lastJumpTime + currentMinDashTimeLatecy))
            {
                Dash();

                if (!isStarted)
                {
                    for (int i = 0; i < balls.Count; i++)
                    {
                        balls[i].SetTrailActive(true);
                    }
                    balls[0].StartGameWhenJump();
                }
            }
            MoveForward();
        }

        if (areaType == AreaType.SLOPE)
        {
            UpdateHeigthsToSlope();
        }

        if (balls.Count > 0)
        {
            CameraFollow.Instance.UpdateFarToHeight(balls[0]._transform.position.y - currentFloorHeight);
        }
    }

    public void StartGame()
    {
        isStarted = true;
        currentSpeed = speed;
    }

    private void MoveForward()
    {
        _transform.position = _transform.position + Vector3.forward * currentSpeed * Time.deltaTime;
    }

    public void AddBall()
    {
        PositionAndPlaySplashEffect();
        Ball newBall = Instantiate(ballPrefab, _transform.position, Quaternion.identity, ballParent).GetComponent<Ball>();
        balls.Add(newBall);

        newBall.UpdateFloorHeight(currentFloorHeight);
        newBall.UpdateOrderFromEndInStack(0);

        Ball.BallState state = balls[balls.Count - 2].GetBallState();
        if (state == Ball.BallState.JUMPING)
        {
            if (balls[balls.Count - 2].GetCurrentSpeed() < 0)
            {
                newBall.Squish();
            }
            else
            {
                newBall.InstantSquish();
                newBall.JumpCommand(lastJumpTime + moveTimeDiff * balls.Count - Time.time, jumpHeight-1);
            }
        }
        else if (state == Ball.BallState.WILL_JUMP)
        {
            newBall.InstantSquish();
            newBall.JumpCommand(lastJumpTime + moveTimeDiff * balls.Count - Time.time, jumpHeight-1);

        }
        else if (state == Ball.BallState.WAITING)
        {
            newBall.InstantSquish();
        }

        UpdateStackOrder();
    }

    private void Dash()
    {
        isDashing = true;
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].DashToFloor();
        }

        float jumpState = balls[balls.Count - 1].GetJumpStateForDash();
        //print("dashed height: " + jumpState.ToString());
        dashedJupmRatio = jumpState;

    }

    public void Jump()
    {
        if (areaType != AreaType.SLOPE)
        {
            if (areaType == AreaType.FINAL || areaType == AreaType.STICKY)
            {
                if (balls.Count > 1)
                {
                    balls[balls.Count - 1].StayOnFloorOnNextJump(false);
                }
                else
                {
                    balls[balls.Count - 1].StayOnFloorOnNextJump(true);
                }
            }
            lastJumpTime = Time.time;
            //float oldJumpHeight = jumpHeight;
            if (isDashing)
            {
                isDashing = false;

                jumpHeight *= dashedJupmRatio;
                jumpHeight += jumpHeightChange;
            }
            else
            {
                jumpHeight -= jumpHeightChange;
            }

            if (jumpHeight > maxJumpHeight)
            {
                jumpHeight = maxJumpHeight;
            }
            else if (jumpHeight < minJumpHeight)
            {
                jumpHeight = minJumpHeight;
            }

            //print("jump high: " + oldJumpHeight.ToString() + " -> " + jumpHeight.ToString());

            float ballJumpHeight;
            int backOrderInStack;

            for (int i = 0; i < balls.Count; i++)
            {
                backOrderInStack = (balls.Count - i - 1);
                ballJumpHeight = jumpHeight + backOrderInStack * jumpHeightDiffRatio;
                balls[i].JumpCommand(i * moveTimeDiff, ballJumpHeight);
            }
        }
        else
        {
            currentSpeed = speed * slopeSpeedMultiplier;
        }
    }

    private void UpdateHeigthsToSlope()
    {
        float newHeigth = currentFloorHeight + ((_transform.position.z - slopeStartZ) * slopeAim);

        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].UpdateFloorHeight(newHeigth);
        }
        CameraFollow.Instance.UpdateFloorHeight(newHeigth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            Floor floor = other.GetComponent<Floor>();
            float floorHeight = floor.GetHeight();

            AreaType oldAreaType = areaType;
            if (floor.GetFloorType() == Floor.FloorType.STICKY_FLOOR)
            {
                areaType = AreaType.STICKY;
            }
            else
            {
                areaType = AreaType.BASIC;

                if (oldAreaType == AreaType.SLOPE)
                {
                    HideShadow(false);
                    UpdateShadowPosition();
                    currentFloorHeight = floorHeight;
                    UpdateFloorHeightForBalls(floorHeight);

                    currentSpeed = speed;
                    if (balls[0].GetBallState() == Ball.BallState.WAITING)
                    {
                        Jump();
                    }
                }
                else if (floorHeight != currentFloorHeight && !balls[balls.Count - 1].IsReadeyForNewHeight())
                {
                    StopPlayer();
                    nextFloorHeight = floorHeight;
                }
                else
                {
                    currentFloorHeight = floorHeight;
                    UpdateFloorHeightForBalls(floorHeight);
                }
            }
        }
        else if (other.CompareTag("Slope"))
        {
            areaType = AreaType.SLOPE;
            HideShadow(true);

            Slope slope = other.GetComponent<Slope>();
            slopeAim = slope.GetSlope();
            slopeStartZ = slope._transform.position.z;
        }
        else if (other.CompareTag("Final"))
        {
            other.GetComponent<FinalPart>().Activate();
            if (areaType != AreaType.FINAL)
            {
                areaType = AreaType.FINAL;
                currentSpeed = speed * finalAreaSpeedMultiplier;
            }
        }
    }

    private void UpdateFloorHeightForBalls(float floorHeight) // and kills balls
    {
        print("UpdateFloorHeightForBalls");
        UpdateShadowPosition();
        CameraFollow.Instance.UpdateFloorHeight(floorHeight);
        for (int i = balls.Count-1; i > -1; i--)
        {
            if (balls[i]._transform.position.y >= currentFloorHeight)
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
            HideShadow(true);
            FinishGame(1f);
        }
    }

    private void UpdateStackOrder()
    {
        print("UpdateStackOrder");
        if (balls.Count > 0)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].UpdateOrderFromEndInStack(balls.Count - (i + 1));
            }
            RecalculateMinDashTime();
        }
        else
        {
            HideShadow(true);
            FinishGame(1f);
        }
    }

    public void ContinueMoveToNextHeigth()
    {
        print("ContinueMoveToNextHeigth");
        currentSpeed = speed;
        currentFloorHeight = nextFloorHeight;
        UpdateFloorHeightForBalls(currentFloorHeight);
    }

    public void ContinueMoveOnStickyFloor()
    {
        print("ContinueMoveOnStickyFloor");
        currentSpeed = speed;
    }

    public void StopPlayer()
    {
        print("StopPlayer");
        currentSpeed = 0;
    }

    public void RemoveBallFromBalls(Ball ball)
    {
        balls.Remove(ball);
        UpdateStackOrder();
    }

    public void SplashCheck(Ball ball)
    {
        if (balls[balls.Count-1] == ball)
        {
            PositionAndPlaySplashEffect();
        }
    }

    private void PositionAndPlaySplashEffect()
    {
        splashEffect.transform.localPosition = Vector3.up * (currentFloorHeight + 0.01f);
        splashEffect.Play();
    }

    private void RecalculateMinDashTime()
    {
        currentMinDashTimeLatecy = balls.Count * moveTimeDiff + minDashTimeOffset;
    }

    private void UpdateShadowPosition()
    {
        shadowTransform.localPosition = Vector3.up * currentFloorHeight;
    }

    private void HideShadow(bool hide)
    {
        shadowTransform.gameObject.SetActive(!hide);
    }

    public void FinishGame(float time)
    {
        isEnded = true;
        LeanTween.delayedCall(time, () =>
        {
            GameManager.Instance.LevelManager.FinishLevel(areaType == AreaType.FINAL);
        });
    }

    public bool IsThisLastBall(Ball ball)
    {
        return balls[balls.Count - 1] == ball;
    }

    public void RequestFeedbackFromNewLastBall()
    {
        print("RequestFeedbackFromNewLastBall");
        balls[balls.Count - 2].SendFeedbackToMoveOnStickyFloor();
    }

    public AreaType GetAreaType()
    {
        return areaType;
    }

    public enum AreaType { BASIC, SLOPE, STICKY, FINAL}
}
