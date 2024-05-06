using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMoveScript : MonoBehaviour
{
    
    public Transform cam;

    float xRot = 0f;
    float yRot = 0f;

    public float sensX = 5f;
    public float sensY = 5f;

    public float moveSpeed = 5f;

    public float gravity = 0.1f;

    float velocity = 0f;

    public float jumpPower = 1f;

    public bool grounded = false;

   

    CharacterController controller;
    
    public Color InspecColor;
    public static Color BlueColor;

    private AirshipWorldScript AWS;

    // Start is called before the first frame update
    void Start()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        BlueColor= InspecColor;
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
       
       

    }

    // Update is called once per frame
    void Update()
    {   

        if(!AWS.Building){
            float MouseX = Input.GetAxisRaw("Mouse X") * sensX;
            float MouseY = Input.GetAxisRaw("Mouse Y") * sensY;

            xRot-=MouseY;
            xRot = Mathf.Clamp(xRot,-90,90);
            yRot+=MouseX;

            cam.rotation = Quaternion.Euler(xRot,yRot,0);
            cam.position = new Vector3(transform.position.x,transform.position.y+0.6f,transform.position.z);
            transform.rotation = Quaternion.Euler(0,yRot,0);

        
            float Horizontal = Input.GetAxisRaw("Horizontal");
            float Vertical = Input.GetAxisRaw("Vertical");

            Vector3 dir = Vector3.zero;

            dir = transform.forward*Vertical + transform.right * Horizontal;

            dir = dir.normalized;

            controller.Move(dir*moveSpeed);

            if(!grounded){
            velocity -=gravity;
            }

            bool Jump = Input.GetKey("space");
            
            if(grounded && Jump){
                velocity = jumpPower;
            }
            grounded = false;
        
            controller.Move(Vector3.up*velocity);
        }

    }
    
    private void OnTriggerStay(Collider other) {
        if(other.gameObject.layer != 3){
            grounded = true;
        }
        
        
    }



}

