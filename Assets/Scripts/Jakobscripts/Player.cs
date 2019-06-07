using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Gravitized
{
    public Camera pCam;
    public float degreesPerSecond = 0.97f;
    public float playerAcceleration = 6f;
    public float playerMaxSpeed = 1f;
    public float turnSpeed = 1f;
    public float playerJumpForce = 10f;
    public float playerJumpCooldown = 0.1f;
    float jumpCooldown = 0f;
    public float touchBias = 0.05f;
    float timeSinceLastTouch = 0f;

    bool shouldBeAirborne = true;

    Vector3 collisionNormal = Vector3.up;
    int numCollisions = 0;

    Vector3 keyInput;
    Vector2 mouseInput;
    Vector3 pSpeed = Vector3.zero;

    private void Awake()
    {
        Initialize();
        //GetCam();
    }

    protected void GetCam()
    {
        pCam = FindObjectOfType<Camera>();
        pCam.transform.SetParent(transform, true);
    }

    private void FixedUpdate()
    {
        Gravitize();
        MovePlayer();
    }

    protected void MovePlayer()
    {
        //Quaternion updateTurn = Quaternion.Euler(0, mouseInput.x * Time.fixedDeltaTime, 0);

        //Vector3 rAng = pCam.transform.rotation.eulerAngles;
        //rAng.x += mouseInput.y * Time.fixedDeltaTime;
        //if (rAng.x > 90f)
        //{
        //    rAng.x = 90f;
        //}
        //else if (rAng.x < -90f)
        //{
        //    rAng.x = -90f;
        //}
        //
        //pCam.transform.rotation = Quaternion.Euler(rAng);
        //transform.rotation = updateTurn * transform.rotation;

        Vector3 directionToMove = transform.rotation * keyInput;
        if (timeSinceLastTouch > 0f)
            timeSinceLastTouch -= Time.fixedDeltaTime;
        if (timeSinceLastTouch < 0f)
            timeSinceLastTouch = 0f;
        if (numCollisions > 0)
            timeSinceLastTouch = touchBias;

        if (Movement.hasPlayerJumped() && jumpCooldown <= 0f)
        {
            jumpCooldown = playerJumpCooldown;
            shouldBeAirborne = true;
        }

        if (jumpCooldown > 0f)
            jumpCooldown -= Time.fixedDeltaTime;
        if (jumpCooldown < 0f)
            jumpCooldown = 0f;

        Vector3 jumpForce = transform.rotation * 
            Movement.receivePlayerJump(timeSinceLastTouch > 0f && jumpCooldown <= 0f) * 
            playerJumpForce;

        GetRB().AddForce(jumpForce / Time.fixedDeltaTime);

        transform.position += directionToMove * Time.fixedDeltaTime;
    }

    protected void Gravitize()
    {
        Vector3 tDown = transform.rotation * Vector3.down;
        Vector3 vCross = Vector3.Cross(totalForces, tDown);

        if (vCross.magnitude > 0)
        {
            float dProd = Vector3.Dot(totalForces.normalized, tDown.normalized);
            Quaternion Q = Quaternion.AngleAxis(
                -Mathf.Acos(Mathf.Clamp(dProd, -1f, 1f)) * Mathf.Rad2Deg,
                vCross.normalized);

            //float tVal = 1f - Mathf.Pow(1f - degreesPerSecond, Time.fixedDeltaTime);
            float tVal = Mathf.Clamp01(TimeUntilUpright(degreesPerSecond) * 
                Time.fixedDeltaTime / Quaternion.Angle(Quaternion.identity, Q));
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Q * transform.rotation,
                tVal);
        }
    }

    protected float TimeUntilUpright(float T)
    {
        return T;
    }

    protected void UpdateControls()
    {
        if (Input.GetMouseButton(0))
        {
            mouseInput = Movement.mouseRot() * turnSpeed;
        }

        keyInput = Movement.receivePlayerKeyboardArrows() * playerAcceleration;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Planet>() != null)
        {
            shouldBeAirborne = false;
            numCollisions++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<Planet>() != null)
        {
            numCollisions--;
        }
    }

    void Update()
    {
        UpdateControls();
    }
}
