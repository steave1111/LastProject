using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float jumpForce = 4;

    [SerializeField] private Animator anim = null;
    [SerializeField] private Rigidbody body = null;
    [SerializeField] GameObject Food;

    private float currentY = 0;
    private float currentX = 0;

    private readonly float interpolation = 10;
    private readonly float walkScale = 0.33f;


    private bool wasGrounded;
    private Vector3 Vec = Vector3.zero;

    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;
    private bool IsJump = false;

    private bool IsGrounded;
    private bool IsPickingUp = false;

    private List<Collider> collisions = new List<Collider>();
    

    private void Awake()
    {
        if (!anim) 
        { 
            gameObject.GetComponent<Animator>(); 
        }
        if (!body) 
        { 
            gameObject.GetComponent<Animator>(); 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        //접촉 포인트들을 리스트에 담아둠
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                    Debug.Log("땅에 닿음");
                }
                IsGrounded = true;
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            anim.SetTrigger("Pickup");
            IsPickingUp = true;
            Invoke("DestroyFood", 0.6f);
            Invoke("Stay", 1.7f);
        }

    }

    private void DestroyFood()
    {
        Destroy(Food);
    }

    private void Stay()
    {
        IsPickingUp = false;
    }



    private void OnCollisionStay(Collision collision)
    {
        //접촉 중이면 걸는 모션이고 아니면 공중에 뜬 모션
        ContactPoint[] contactPoints = collision.contacts;
        bool CanWalk = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                CanWalk = true; break;
            }
        }

        if (CanWalk)
        {
            IsGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { IsGrounded = false; }
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        //접촉 중이지 않을 때 리스트에 모든 정보를 제거.
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
            Debug.Log("공중에 있음");
        }
        if (collisions.Count == 0) { IsGrounded = false; }

        IsPickingUp = false;
    }

    private void Update()
    {
        //스페이스바를 눌렀다면
        if (!IsJump && Input.GetKey(KeyCode.Space))
        {
            IsJump = true;
        }

    }

    private void FixedUpdate()
    {
        //착지 여부
        anim.SetBool("Grounded", IsGrounded);

        if (IsPickingUp == false)
        {
            Move();
        }
        


        wasGrounded = IsGrounded;
        IsJump = false;
    }


    private void Move()
    {
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            y *= walkScale;
            x *= walkScale;
        }

        currentY = Mathf.Lerp(currentY, y, Time.deltaTime * interpolation);
        currentX = Mathf.Lerp(currentX, x, Time.deltaTime * interpolation);

        Vector3 direction = camera.forward * currentY + camera.right * currentX;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            Vec = Vector3.Slerp(Vec, direction, Time.deltaTime * interpolation);

            transform.rotation = Quaternion.LookRotation(Vec);
            transform.position += Vec * moveSpeed * Time.deltaTime;
            anim.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        if (jumpCooldownOver && IsGrounded && IsJump)
        {
            jumpTimeStamp = Time.time;
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }

        if (!wasGrounded && IsGrounded)
        {
            anim.SetTrigger("Land");
        }

        if (!IsGrounded && wasGrounded)
        {
            anim.SetTrigger("Jump");
        }
    }
    
}
