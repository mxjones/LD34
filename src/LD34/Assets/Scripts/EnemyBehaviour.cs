using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBehaviour : MonoBehaviour
{

    public GameObject Player;
    public float Size;
    public float Speed;
    public float MinTimeSinceLastMeal = 3.0f;
    public float MinSizeForShrinking = 10;
    public float MinTimeSinceLastShrink = 1.0f;
    public float SizeIncreaseRate;
    public float SizeDecreaseRate;
    public float SpeedIncreaseRate;
    public float MinSpeed;
    public float SpeedDecreaseRate;
    public Guid UniqueIdentifier;
    public float AggroRadius;
    public float FinalXPoint;
    public float FinalYPoint;
    public float StartXPoint;
    public float StartYPoint;
    public float MaxSpeed;
    public float MaxSizeOnSpawn = 5.0f;


    private Rigidbody _rigidbody;
    private float _timeElapsedSinceLastMeal;
    private float _timeElapsedSinceLastShrunk;

    private GameObject _currentTarget;
    private GameObject _closestEnemy;
    private GameObject _closestPickUp;
    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        var spawnX = Random.Range(-8, 19);
        var spawnY = Random.Range(-6, 22);

        transform.position = new Vector2(spawnX, spawnY);
        _rigidbody = GetComponent<Rigidbody>();
        _timeElapsedSinceLastMeal = 0;
        _timeElapsedSinceLastShrunk = 0;
        UniqueIdentifier = Guid.NewGuid();

        var initialSize = Random.Range(1, MaxSizeOnSpawn);
        for (int i = 0; i < initialSize; i++)
        {
            Grow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ResetGameObjects();
        _closestEnemy = FindClosestForTag("Enemy").Where(x => x != null && x.GetComponent<EnemyBehaviour>() != null).FirstOrDefault(x => x.GetComponent<EnemyBehaviour>().UniqueIdentifier != UniqueIdentifier);
        _closestPickUp = FindClosestForTag("PickUpItem").FirstOrDefault();

        Move();

        _timeElapsedSinceLastMeal += Time.deltaTime;
        _timeElapsedSinceLastShrunk += Time.deltaTime;

        if (_timeElapsedSinceLastShrunk > MinTimeSinceLastShrink)
        {
            if (_timeElapsedSinceLastMeal > MinTimeSinceLastMeal && Size > MinSizeForShrinking)
                Shrink();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO: Pick up Items shouldn't give us much growth as an enemy.
        if (other.gameObject.CompareTag("PickUpItem"))
        {
            // Pick Up
            Grow();
            Destroy(other.gameObject);
            _timeElapsedSinceLastMeal = 0;
        }

        if (other.gameObject.CompareTag("Enemy") && other.GetComponent<EnemyBehaviour>().Size < Size)
        {
            Grow();
            Destroy(other.gameObject);
            _timeElapsedSinceLastMeal = 0;
        }
    }

    Vector2 GetBoundaryForce()
    {
        Vector2 movement = new Vector2();
        if (transform.position.x >= FinalXPoint)
        {
            movement.x = -(Speed*Time.deltaTime) * 10;
        }

        // As far Left I can go
        if (transform.position.x <= StartXPoint)
        {
            movement.x = Speed * Time.deltaTime * 10;
        }

        // As far Up I can go
        if (transform.position.y >= FinalYPoint)
        {
            movement.y = -(Speed * Time.deltaTime) * 10;
        }

        // As far Down I can go
        if (transform.position.y <= StartYPoint)
        {
            movement.y = Speed * Time.deltaTime * 10;
        }

        return movement;
    }

    void CheckBoundaries(ref float horizontal, ref float vertical)
    {
        if (transform.position.x >= FinalXPoint)
        {
            if (horizontal > 0)
                horizontal = -horizontal;
        }


        // As far Left I can go
        if (transform.position.x <= StartXPoint)
        {
            if (horizontal < 0)
                horizontal = -horizontal;
        }

        // As far Up I can go
        if (transform.position.y >= FinalYPoint)
        {
            if (vertical > 0)
                vertical = -vertical;
        }

        // As far Down I can go
        if (transform.position.y <= StartYPoint)
        {
            if (vertical < 0)
                vertical = -vertical;
        }
    }
//

    void ClampTheSpeed()
    {
        _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, MaxSpeed);
    }

    // TODO: Does this need to be so long and horrible? Should refactor.
    void Move()
    {
        _currentTarget = null;


        // Check if player is closer than closest enemy
        //        if (Player.transform.position.x - transform.position.x < _closestEnemy.transform.position.x - transform.position.x
        //            && Player.transform.position.y - transform.position.y < _closestEnemy.transform.position.y - transform.position.y)
        if (Vector2.Distance(Player.transform.position, transform.position) <
           Vector2.Distance(_closestEnemy.transform.position, transform.position))
        {
            // Need to check if the Player is smaller than this enemy or not
            if (Vector2.Distance(Player.transform.position, transform.position) < AggroRadius)
            {
                if (Player.GetComponent<PlayerBehaviour>().Size < Size)
                {
                    // Player is smaller, GET SOME
                    _currentTarget = Player;
//                    var movement = (_currentTarget.transform.position - transform.position) * Speed * Time.deltaTime;
//                    _rigidbody.velocity = movement;
//                    var directionToMove = _currentTarget.transform.position - transform.position; 
//                    transform.position = Vector2.MoveTowards(transform.position, directionToMove, Speed * Time.deltaTime);
                    _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);

                    //_rigidbody.AddForce(movement);
                }
                else if (Player.GetComponent<PlayerBehaviour>().Size > Size)
                {
                    // Ut oh, Player is bigger.
                    // Go the opposite way
                    Debug.Log("Why are you not running");

                    _currentTarget = Player;
//                    var movement = (-_currentTarget.transform.position - transform.position) * Speed * Time.deltaTime;
//
//                    _rigidbody.velocity = movement;
//                    var directionToMove = _currentTarget.transform.position - transform.position;
//                    transform.position = Vector2.MoveTowards(transform.position, -directionToMove, Speed * Time.deltaTime);
                    _rigidbody.AddForce(-(_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);

                    //_rigidbody.AddForce(movement);
                }
                else
                {
                    // Same size, lets get some dinner.
                    _currentTarget = _closestPickUp;
//                    var movement = (_currentTarget.transform.position - transform.position) * Speed * Time.deltaTime;
//                    _rigidbody.velocity = movement;
//                    
                    _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);



                    // _rigidbody.velocity = movement;
                    //_rigidbody.AddForce(movement);
                }
            }
            else
            {
                // Same size, lets get some dinner.
                _currentTarget = _closestPickUp;
//                var movement = (_currentTarget.transform.position - transform.position) * Speed * Time.deltaTime;
//                _rigidbody.velocity = movement;
//                var directionToMove = _currentTarget.transform.position - transform.position;
//                transform.position = Vector2.MoveTowards(transform.position, directionToMove, Speed * Time.deltaTime);
                _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);

            }
        }
        else
        {
            // Check if Enemy is closer than food.
            if (Vector2.Distance(_closestEnemy.transform.position, transform.position) <
                Vector2.Distance(_closestPickUp.transform.position, transform.position))
            {
                // Enemy is closest.
                if (_closestEnemy.GetComponent<EnemyBehaviour>().Size < Size)
                {
                    _currentTarget = _closestEnemy;
//                    var movement = (_currentTarget.transform.position - transform.position) * (Speed * Time.deltaTime);
//                    _rigidbody.velocity = movement;
//                    var directionToMove = _currentTarget.transform.position - transform.position;
//                    transform.position = Vector2.MoveTowards(transform.position, directionToMove, Speed * Time.deltaTime);
                    _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);



                }
                else if (_closestEnemy.GetComponent<EnemyBehaviour>().Size > Size)
                {
                    _currentTarget = _closestEnemy;
//                    var movement = (-_currentTarget.transform.position - transform.position) * (Speed * Time.deltaTime);
//                    _rigidbody.velocity = movement;

//                    var directionToMove = _currentTarget.transform.position - transform.position;
//                    transform.position = Vector2.MoveTowards(transform.position, -directionToMove, Speed * Time.deltaTime);
                    _rigidbody.AddForce(-(_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);



                }
                else
                {
                    _currentTarget = _closestPickUp;
//                    var movement = (_currentTarget.transform.position - transform.position) * (Speed * Time.deltaTime);
//                    _rigidbody.velocity = movement;
//                    var directionToMove = _currentTarget.transform.position - transform.position;
//                    transform.position = Vector2.MoveTowards(transform.position, directionToMove, Speed * Time.deltaTime);
                    _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);



                }
            }
            else
            {
                _currentTarget = _closestPickUp;
                //var movement = (_currentTarget.transform.position - transform.position) * (Speed * Time.deltaTime);
                //_rigidbody.velocity = movement;
//                var directionToMove = _currentTarget.transform.position - transform.position;
//                transform.position = Vector2.MoveTowards(transform.position, directionToMove, Speed * Time.deltaTime);
                _rigidbody.AddForce((_currentTarget.transform.position - transform.position) * (Speed / 2) * Time.smoothDeltaTime);
            }
        }

        ClampTheSpeed();
        var movementBoundary = GetBoundaryForce();
        _rigidbody.AddForce(movementBoundary);
    }

    private void Grow()
    {
        Size += 1.0f;
        if (Speed > MinSpeed)
            Speed -= SpeedDecreaseRate;

        var scale = new Vector3(SizeIncreaseRate, SizeIncreaseRate, SizeIncreaseRate);
        transform.localScale += Vector3.Lerp(transform.localScale, scale, 2f);
    }

    private void Shrink()
    {
        Size -= 1.0f;
        if (Speed > MinSpeed)
            Speed += SpeedIncreaseRate;

        var scale = new Vector3(SizeDecreaseRate, SizeDecreaseRate, SizeDecreaseRate);
        transform.localScale -= Vector3.Lerp(transform.localScale, scale, 2f);

        _timeElapsedSinceLastShrunk = 0;
    }

    private List<GameObject> FindClosestForTag(string tagToFind)
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(tagToFind)
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
            .ToList();

        return gameObjects;
    }

    private void ResetGameObjects()
    {
        _currentTarget = null;
        _closestEnemy = null;
        _closestPickUp = null;
    }
}
