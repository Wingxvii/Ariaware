using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool toggleFreeMovement;
    public float camSpeed = 4f;
    public float camTurnSpeed = 100f;
    public Vector3 storedTurn;
    Vector3 move;
    Vector2 adjustRot;

    bool mouseClicked = false;
    // Start is called before the first frame update
    void Start()
    {
        storedTurn = Vector2.zero;
    }

    private void Update()
    {
        mouseClicked = Input.GetMouseButton(0);
        //move = Movement.receiveCameraKeyboard();
        //if(mouseClicked)
        //    adjustRot = Movement.mouseRot();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (toggleFreeMovement)
        {
            move = Vector3.zero;
            adjustRot = Vector2.zero;

            move = Movement.receiveCameraKeyboard();
            move *= Time.fixedDeltaTime * camSpeed;

            if (mouseClicked)
            {
                adjustRot = Movement.mouseRot();
                adjustRot *= Time.fixedDeltaTime * camTurnSpeed;
            }

            move = transform.rotation * move;
            transform.position += move;

            storedTurn.x -= adjustRot.y;
            storedTurn.y += adjustRot.x;

            if (storedTurn.x > 90)
            {
                storedTurn.x = 90;
            }
            else if (storedTurn.x < -90)
            {
                storedTurn.x = -90;
            }

            transform.rotation = Quaternion.Euler(storedTurn);
        }
    }
}
