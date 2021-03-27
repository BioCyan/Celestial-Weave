using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidBody;
    [SerializeField] bool isGrounded = true;
    [SerializeField] bool JumpUsed = false;

    public float speed = 6;
    public float jumpIntensity = 40;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (!JumpUsed)))
        {
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
