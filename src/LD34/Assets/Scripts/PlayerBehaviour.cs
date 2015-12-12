using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float Speed;
    public float MaxSpeed;
    public float MinSpeed;
    public float SpeedDecreaseRate;
    public float SpeedIncreaseRate;
    public float SizeIncreaseRate;
    public float SizeDecreaseRate;
    public float Size = 0;
    public float MinSizeForShrinking;
    public float MinTimeSinceLastMeal;
    public float MinTimeSinceShrinking;

    public float FinalXPoint;
    public float FinalYPoint;
    public float StartXPoint;
    public float StartYPoint;

    public Sprite SheepDown;
    public Sprite SheepUp;
    public Sprite SheepLeft;
    public Sprite SheepRight;

    private float _targetOrtho;


    private Rigidbody _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private float _timeElapsedSinceLastMeal;
    private float _timeElapsedSinceLastShrunk;

    private Direction _direction;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _timeElapsedSinceLastMeal = 0;
        _timeElapsedSinceLastShrunk = 0;
        _targetOrtho = Camera.main.orthographicSize;
    }

    void ChangeSprite()
    {
        switch (_direction)
        {
            case Direction.Down:
                _spriteRenderer.sprite = SheepDown;
                break;
            case Direction.Left:
                _spriteRenderer.sprite = SheepLeft;
                break;
            case Direction.Right:
                _spriteRenderer.sprite = SheepRight;
                break;
            case Direction.Up:
                _spriteRenderer.sprite = SheepUp;
                break;
        }
    }

    void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        // As far Right I can go
        if (transform.position.x >= FinalXPoint)
        {
            if (horizontalMovement > 0)
                horizontalMovement = 0;
        }


        // As far Left I can go
        if (transform.position.x <= StartXPoint)
        {
            if (horizontalMovement < 0)
                horizontalMovement = 0;
        }

        // As far Up I can go
        if (transform.position.y >= FinalYPoint)
        {
            if (verticalMovement > 0)
                verticalMovement = 0;
        }

        // As far Down I can go
        if (transform.position.y <= StartYPoint)
        {
            if (verticalMovement < 0)
                verticalMovement = 0;
        }

        if (Math.Abs(horizontalMovement) > 0.1)
            _direction = horizontalMovement > 0 ? Direction.Right : Direction.Left;

        if (Math.Abs(verticalMovement) > 0.1)
            _direction = verticalMovement > 0 ? Direction.Up : Direction.Down;

        var maxSpeed = MaxSpeed;
        if (Math.Abs(horizontalMovement) > 0.1 && Math.Abs(verticalMovement) > 0.1)
            maxSpeed = MaxSpeed * 0.7f;

        if (_rigidBody.velocity.magnitude > MaxSpeed)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * maxSpeed;
            Debug.Log("Max Speed. " + maxSpeed);
        }
        else
        {
            Vector2 movement = new Vector2(horizontalMovement * Speed, verticalMovement * Speed);
            _rigidBody.velocity = movement;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.CompareTag("PickUpItem"))
        {
            // TODO: Add other than fruit.
            // TODO: Pick up shouldn't add as much growth as Enemy eaten..
            Grow();
            Destroy(other.gameObject);
            _timeElapsedSinceLastMeal = 0;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyBehaviour>().Size > Size)
            {
               // That's a loss.
            }
            else if(other.gameObject.GetComponent<EnemyBehaviour>().Size < Size)
            {
                // THAT'S A DINNER
                Grow();
                Destroy(other.gameObject);
                _timeElapsedSinceLastMeal = 0;
            } 
        }
    }

    void Grow()
    {
        Size += 1.0f;
        if (Speed > MinSpeed)
            Speed -= SpeedDecreaseRate;

        var scale = new Vector3(SizeIncreaseRate, SizeIncreaseRate, SizeIncreaseRate);
        transform.localScale += Vector3.Lerp(transform.localScale, scale, 2f);
    }

    void Shrink()
    {
        Size -= 1.0f;
        if (Speed > MinSpeed)
            Speed += SpeedIncreaseRate;

        var scale = new Vector3(SizeDecreaseRate, SizeDecreaseRate, SizeDecreaseRate);
        transform.localScale -= Vector3.Lerp(transform.localScale, scale, 2f);

        _timeElapsedSinceLastShrunk = 0;
    }

    void ZoomCameraOut()
    {
        _targetOrtho -= SizeIncreaseRate;
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, _targetOrtho, 2.0f * Time.deltaTime);
    }



    // Update is called once per frame
    void Update()
    {
        ChangeSprite();
        _timeElapsedSinceLastMeal += Time.deltaTime;
        _timeElapsedSinceLastShrunk += Time.deltaTime;

        if (_timeElapsedSinceLastShrunk > MinTimeSinceShrinking)
        {
            if (_timeElapsedSinceLastMeal > MinTimeSinceLastMeal && Size > MinSizeForShrinking)
                Shrink();
        }
    }
}
