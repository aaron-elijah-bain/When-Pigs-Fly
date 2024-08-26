using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMoveScript : MonoBehaviour
{
    
    public Transform cam;
    private Transform myCamera;

    float xRot = 0f;
    float yRot = 0f;

    public float sensX = 5f;
    public float sensY = 5f;

    public float moveSpeed = 5f;

    public float gravity = 0.1f;

    float velocity = 0f;

    public float jumpPower = 1f;

    
    public bool grounded = false;
    public bool groundedOnShip = false;

   

    CharacterController controller;
    
    public Color InspecColor;
    public static Color BlueColor;

    public bool isDriving = false;

    private AirshipWorldScript AWS;

    // Start is called before the first frame update
    void Start()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        BlueColor= InspecColor;
        Cursor.lockState = CursorLockMode.Locked;
        myCamera = cam.gameObject.GetComponentInChildren<Camera>().transform;
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


            if(Input.GetKeyDown(KeyCode.Tab)){
                
                        if(myCamera.localPosition == new Vector3(0,0,-10)){
                           
                            myCamera.localPosition = new Vector3(0,0,0);

                        }else if(myCamera.localPosition == new Vector3(0,0,0)){
                            myCamera.localPosition = new Vector3(0,0,-10);
                        }
            }

            if(isDriving && AWS.Crystal != null){
                    transform.SetParent(AWS.Crystal.transform,true);

                    
            }
            if(Input.GetKeyDown(KeyCode.E)){
                if(!isDriving){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity,~LayerMask.NameToLayer("Player"))){
                    if(hit.collider != null){
                        
                        AirshipBlockScript ABS = hit.collider.gameObject.GetComponent<AirshipBlockScript>();
                    
                        if(ABS != null){
                                if(ABS.centerScript.Type == "Wheel"){
                                    
                                    
                                    isDriving = true;
                                    AWS.Crystal = ABS.transform.parent.gameObject;

                                    
                                    
                                }
                            
                        }
                    }
                }
                }else{
                    isDriving = false;
                }
            }
            
                
            }        


    }
    void FixedUpdate(){
        
            if(!isDriving && !AWS.Building){
                
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


                if(groundedOnShip && AWS.Crystal != null){
                    transform.SetParent(AWS.Crystal.transform,true);

                }else{
                    transform.SetParent(null, true);
                }
                groundedOnShip = false;
                grounded = false;

                
                
                
                controller.Move(Vector3.up*velocity);
                
                   
            }
    }
    
    private void OnTriggerStay(Collider other) {
        if(other.gameObject.layer != 3){
            grounded = true;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Blocks")){
            groundedOnShip = true;
        }
        
        
    }



}

