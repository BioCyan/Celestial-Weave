using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidBody;
    [SerializeField] bool isGrounded = true;
    [SerializeField] bool JumpUsed = false;
    private Animator anim;

    public float speed = 6;
    public float jumpIntensity = 40;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        anim.SetFloat("Moving", z);

        if (transform.rotation.x != 0f || transform.rotation.z != 0f)
            RotateThroughPortal();

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (!JumpUsed)))
        {
            anim.SetTrigger("Jump");
            if (!isGrounded)
            {
                JumpUsed = true;
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            }
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpIntensity, ForceMode.Impulse);
        }

        if (isGrounded && JumpUsed)
        {
            JumpUsed = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 newMove = new Vector3(move.x, rigidBody.velocity.y, move.z);

        rigidBody.velocity = newMove;
    }

    private void RotateThroughPortal()
    {
        float epsilon = 0.01f, rotSpeed = 1.5f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), rotSpeed * Time.deltaTime);
        // Snap rotation if small enough difference
        if ((transform.rotation.x < epsilon && transform.rotation.x > -epsilon) && (transform.rotation.z < epsilon && transform.rotation.z > -epsilon))
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
            isGrounded = false;
    }
}
