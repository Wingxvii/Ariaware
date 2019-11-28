// Copied from John's 2nd Year Mocap Assignment

using UnityEngine;

public class DroidMovement : MonoBehaviour
{

    public Animator anim;
    public Transform player;

    private float _rotY;
    public float mouseSensitivity = 50.0f;

    // Use this for initialization
    void Start()
    {
        //get objects
        anim = this.GetComponent<Animator>();
        player = this.GetComponent<Transform>();

        //lock mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

        //mouse rotation 
        float mX = Input.GetAxis("Mouse X");
        _rotY += mX * mouseSensitivity * Time.deltaTime;
        Quaternion localRot = Quaternion.Euler(0.0f, _rotY, 0.0f);
        player.transform.rotation = localRot;

        anim.SetFloat("Walk", Input.GetAxis("Vertical"));
        anim.SetFloat("Turn", Input.GetAxis("Horizontal"));

        //extra animations
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.Play("Death");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.Play("Jump");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.Play("Attack");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.Play("Spin Attack");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.Play("Dance");
        }
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    anim.Play("Defuse");
        //}
    }
}
