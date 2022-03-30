using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private bool isFirstBall = false;
    [SerializeField] private ParticleSystem dashEffect = null;
    [SerializeField] private float gravityMultiplier = 1;
    [SerializeField] private float dashAcceleration = 6;
    [SerializeField] private Transform mesh = null;
    [SerializeField] private float squishedOffset = 0.7f;
    [SerializeField] private Transform shadow = null;
    [SerializeField] private MeshRenderer meshToChangeColor = null;
    [SerializeField] GameObject trail = null;

    [HideInInspector] public Transform _transform = null;
    private BallState state = BallState.WAITING;
    private float gravity = -9.8f;

    // general
    private bool waitingToSendFeedback = false;
    private bool startGameOnJump = false;
    private float floorHeight = 0;
    private float orderInStack;

    // final
    private bool stayOnFloorOnNextJump = false;
    private bool stopPlayer = false;

    // sets with jump function
    private float lastJumpTime;
    private float initialUpVelocity;
    private float startJumpHeight;

    // sets with dash function
    private bool isDashing = false;
    private float lastDashTime;

    
    void Awake()
    {
        _transform = transform;
        meshToChangeColor.material.color = CreateRandomColor();
        trail.GetComponent<TrailRenderer>().startColor = meshToChangeColor.material.color;
    }

    private void Update()
    {
        if (state == BallState.JUMPING)
        {
            float time = Time.time - lastJumpTime;
            float currentJumpHeight = (initialUpVelocity + (gravity * gravityMultiplier * time / 2)) * time;

            if (isDashing)
            {
                float dashTime = (Time.time - lastDashTime);
                currentJumpHeight -= dashAcceleration * dashTime * dashTime / 2;
            }

            float nextHeight;
            //print((startJumpHeight + currentJumpHeight + antiErrorOffset).ToString() + " " + (floorHeight + (orderInStack * squishedOffset)).ToString() + " " + currentSpeed.ToString());
            if (startJumpHeight + currentJumpHeight < floorHeight + (orderInStack * squishedOffset) && GetCurrentSpeed() < 0)
            {
                nextHeight = floorHeight + orderInStack * squishedOffset;
                Squish();
                state = BallState.WAITING;
                isDashing = false;
                Player.Instance.SplashCheck(this);
            }
            else
            {
                nextHeight = startJumpHeight + currentJumpHeight;
            }

            //_transform.localPosition = Vector3.Lerp(_transform.localPosition, Vector3.up * nextHeight, Time.fixedDeltaTime * 100);
            _transform.localPosition = Vector3.up * nextHeight;
        }
    }

    public void JumpCommand(float latecy, float jumpHeight)
    {
        state = BallState.WILL_JUMP;
        this.lastJumpTime = Time.time + latecy;
        StartCoroutine(Jump(latecy, jumpHeight));
    }

    private IEnumerator Jump(float time, float jumpHeight)
    {
        yield return new WaitForSeconds(time);

        Release();
        if (stayOnFloorOnNextJump)
        {
            StayOnFloor();
        }
        else
        {
            state = BallState.JUMPING;
            this.startJumpHeight = floorHeight + orderInStack * squishedOffset;
            initialUpVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity * gravityMultiplier);

            if (waitingToSendFeedback)
            {
                waitingToSendFeedback = false;
                Player.Instance.ContinueMove();
            }

            if (startGameOnJump)
            {
                startGameOnJump = false;
                Player.Instance.StartGame();
            }
        }
    }

    public void DashToFloor()
    {
        if (state == BallState.JUMPING)
        {
            dashEffect.Play();
            lastDashTime = Time.time;
            isDashing = true;
        }
    }

    public void UpdateFloorHeight(float newFloorHeight)
    {
        floorHeight = newFloorHeight;
        if (state != BallState.JUMPING)
        {
            _transform.localPosition = Vector3.up * (floorHeight + orderInStack * squishedOffset);
        }
    }

    public void UpdateOrderFromEndInStack(int newOrder)
    {
        orderInStack = newOrder;
        if (state != BallState.JUMPING)
        {
            _transform.localPosition = Vector3.up * (floorHeight + orderInStack * squishedOffset);
        }
    }

    public float GetCurrentSpeed()
    {
        float time = Time.time - lastJumpTime;
        float currentSpeed = initialUpVelocity + (gravity * gravityMultiplier * time);
        if (isDashing)
        {
            float dashTime = (Time.time - lastDashTime);
            currentSpeed -= dashAcceleration * dashTime;
        }
        return currentSpeed;
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

    public void InstantSquish()
    {
        mesh.localScale = new Vector3(1.3f, 0.7f, 1.3f);
    }

    public void SetTrailActive(bool active)
    {
        trail.SetActive(active);
    }

    public void SetColor(Color color)
    {
        meshToChangeColor.material.color = color;
    }

    public Color GetColor()
    {
        return meshToChangeColor.material.color;
    }

    public void Kill()
    {
        state = BallState.KILLED;
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
        CollectableBall cb = other.GetComponent<CollectableBall>();
        if (other.CompareTag("CollectableBall") && cb.GetBall())
        {
            Player.Instance.AddBall(cb.GetColor());
        }
    }

    // first ball
    public float GetJumpStateForDash()
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

    // first ball
    public void StartGameWhenJump()
    {
        startGameOnJump = true;
    }

    // last ball
    public bool IsReadeyForNewHeight()
    {
        if (state != BallState.JUMPING) // ############### bu þekilde sadece jumping durumunda geçiþ yapýlýr ######################
        {
            waitingToSendFeedback = true;
        }
        return state == BallState.JUMPING;
    }

    // last ball
    public BallState GetBallState()
    {
        return state;
    }

    // last ball
    public void StayOnFloorOnNextJump(bool stopGame)
    {
        if (stopGame)
        {
            this.stopPlayer = stopGame;
        }
        stayOnFloorOnNextJump = true;
    }

    // last ball
    private void StayOnFloor()
    {
        Player player = Player.Instance;
        if (stopPlayer)
        {
            player.StopPlayer();
        }

        player.RemoveBallFromBalls(this);
        _transform.parent = null;

        if (!isFirstBall)
        {
            shadow.gameObject.SetActive(true);
        }
        else
        {
            player.FinishGame(true, 2f);
        }
    }

    public enum BallState { WAITING, WILL_JUMP, JUMPING, KILLED }

    public static Color CreateRandomColor()
    {
        return Random.ColorHSV(0, 1, 0.8f, 0.9f, 0.8f, 0.9f);
    }
}
