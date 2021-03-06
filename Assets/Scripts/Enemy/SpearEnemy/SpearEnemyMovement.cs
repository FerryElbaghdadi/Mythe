﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpearEnemyMovement : MonoBehaviour {

    private GameObject _Player;
    private GameObject _LeftShooter;
    private GameObject _RightShooter;
    
    private Rigidbody2D rb;
    private Rigidbody2D _PlayerRb;

    protected bool _FacingRight = true;
    private bool _InAir;

    private int _LayerMask;
    private int _ObstacleMask;
    private int _JumpCoolDown;

    private float _XStartPoint;

    [SerializeField] private float _JumpProximity;

    private Vector2 _JumpForce = new Vector2(0, 350);
    private Vector2 _MoveForce = new Vector2(3.5f, 0);

    private RaycastHit2D _LineOfFire;

    private SpearShooter _SpearThrowR;
    private SpearShooter _SpearThrowL;

    void Start()
    {
        _Player = GameObject.FindWithTag(GameTags.player);
        _PlayerRb = _Player.GetComponent<Rigidbody2D>();

        rb = GetComponent<Rigidbody2D>();

        _LayerMask = LayerMask.GetMask("Player");
        _ObstacleMask = LayerMask.GetMask("Ground");

        _LeftShooter = GameObject.FindWithTag("LShoot");
        _RightShooter = GameObject.FindWithTag("RShoot");

        _SpearThrowL = _LeftShooter.GetComponent<SpearShooter>();
        _SpearThrowR = _RightShooter.GetComponent<SpearShooter>();
    }

    void Update()
    {
        //_JumpForce = new Vector2(Random.Range(-100f, 100f), Random.Range(250f, 350f));
        if(_JumpCoolDown < 0)
        {
            _JumpCoolDown = 0;
        }
        Patrol();

        Debug.Log(_MoveForce);
    }

    public void Attack()
    {
        PlayerHeightCheck();
        SearchForTarget();

        //only jump if player is above you
    }
    
    void SlowJump()
    {
        //slow the enemy down when in the air
        if (_InAir)
            _MoveForce.x--;
    }

    void PlayerHeightCheck()
    {
        if (_PlayerRb.position.y > rb.position.y)
            Jump();
        else
            Debug.Log("Dont Jump");
    }

    void SearchForTarget()
    {
        //determine what side enemy must look at
        if (transform.position.x < _PlayerRb.position.x)
            _FacingRight = true;
        else
            _FacingRight = false;

        //cast raycast in facing direction
        if (_FacingRight)
        {
            _LineOfFire = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, _LayerMask);
            Throw();
        }
        else
        {
            _LineOfFire = Physics2D.Raycast(transform.position, -transform.right, Mathf.Infinity, _LayerMask);
            Throw();
        }  
    }

    void ObstacleCheck()
    {
        if (_FacingRight == true)
        {
            //Raycast to the right to detect when the enemy needs to jump over an obstacle
            //Jump proximity indicates at what distance the enemy needs to jump
            RaycastHit2D obstacleCheck = Physics2D.Raycast(transform.position, Vector2.right, _JumpProximity, _ObstacleMask);

            //Raycast to check if the player is past a block or not and if not, do not jump. go for player instead
            RaycastHit2D playerCheck = Physics2D.Raycast(transform.position, Vector2.right, _JumpProximity, _LayerMask);

            //check if wall is too close
            if(obstacleCheck.distance < 0.52f && obstacleCheck.distance > 0 && _FacingRight == true)
            {
                Debug.Log("wall too close on right");
                TurnAround();
            }

            if (obstacleCheck.collider.tag == GameTags.ground && _JumpCoolDown == 0 && playerCheck.collider == null)
            {
                //jump
                Jump();
            }
        }
        else
        {
            //Raycast to the left to detect when the enemy needs to jump over an obstacle
            RaycastHit2D obstacleCheck = Physics2D.Raycast(transform.position, Vector2.left, _JumpProximity, _ObstacleMask);
            //playercheck raycast to the left
            RaycastHit2D playerCheck = Physics2D.Raycast(transform.position, Vector2.left, _JumpProximity, _LayerMask);

            //check if wall is too close
            if (obstacleCheck.distance < 0.52f && obstacleCheck.distance > 0 && _FacingRight == false)
            {
                TurnAround();
            }

            if (obstacleCheck.collider.tag == GameTags.ground && _JumpCoolDown == 0 && playerCheck.collider == null)
            {
                //jump
                Jump();
            }
        }
    }

    void Jump()
    {
        if(_JumpCoolDown == 0)
        {
            //jump
            rb.AddForce(_JumpForce);
            _JumpCoolDown = 25;
            _InAir = true;
        }        
    }

    void Throw()
    {
        //throw spear
        if (_LineOfFire.collider.tag == GameTags.player)
        {
            
        }
    }

    void Patrol()
    {
        //make spear enemy walk around aimlessly, jumping over obstacles.
        if (_FacingRight) // walk right
            rb.AddForce(_MoveForce);
        else //walk left
            rb.AddForce(-_MoveForce);

        ObstacleCheck();     
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if(coll.gameObject.tag == GameTags.ground)
        {
            _JumpCoolDown--;
            _InAir = false;
        }
    }

    void TurnAround()
    {
        //turn enemy around
        if (_FacingRight == true)
            _FacingRight = false;
        else
            _FacingRight = true;
    }
}