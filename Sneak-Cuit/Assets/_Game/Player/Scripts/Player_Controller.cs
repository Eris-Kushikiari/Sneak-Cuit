using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Player_Controller : MonoBehaviour
{
    private enum Directions { UP, DOWN, RIGHT, LEFT }

    [Header("Movement Attributes")]
    [SerializeField] float moveSpeed = 50f;

    [Header("Dependencies")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRender;


    private Vector2 _moveDirection = Vector2.zero;
    private Directions _facingDirection = Directions.RIGHT;

    private readonly int _animMoveRight = Animator.StringToHash("Player_Run_Animation");
    private readonly int _animIdleRight = Animator.StringToHash("Player_Idle_Animation");

    public CollectebleManager cm;
    
    // Update is called once per frame
    void Update()
    {
        GatherInputs();
        CalculateFacingDirection();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void GatherInputs()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.y = Input.GetAxisRaw("Vertical");
    }

    private void Movement()
    {
       rb.velocity = _moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
    }

    private void CalculateFacingDirection()
    {
        if (_moveDirection.x != 0)
        {
            if (_moveDirection.x > 0)
            {
                _facingDirection = Directions.RIGHT;
            }
            else if (_moveDirection.x < 0)
            {
                _facingDirection = Directions.LEFT;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (_facingDirection == Directions.LEFT)
        {
            _spriteRender.flipX = true;
        }
        else if (_facingDirection == Directions.RIGHT)
        {
            _spriteRender.flipX = false;
        }

        if (_moveDirection.sqrMagnitude > 0) // We're moving
        {
            _animator.CrossFade(_animMoveRight, 0);
        }
        else
        {
            _animator.CrossFade(_animIdleRight, 0);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Biscuit"))
        {
            Destroy(other.gameObject);
            cm.biscuitCount++;
        }
    }
}
