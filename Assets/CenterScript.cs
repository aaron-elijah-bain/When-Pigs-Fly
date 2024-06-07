using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterScript : MonoBehaviour
{
    public int[] Sides;
    public string[] Rots;

    public string Type = "";

    public float Mass = 0f;

    private PigMoveScript playerScript;

    public Rigidbody Crystalrb;

    private GameObject Rotor;

    private float RotorRot = 0f;

    public float RotorSpeed = 5f;

    public float Power = 1f;

    public float InputMultiplier = 0;

    public KeyCode posInput;
    public KeyCode negInput;

    public bool isAutomatic = false;

    private AirshipWorldScript AWS;


    
    // Start is called before the first frame update
    void Start()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        Type = gameObject.name;
        Type = Type.Replace("(Clone)","");
        playerScript = GameObject.FindWithTag("Player").GetComponent<PigMoveScript>();
        if(Type[0].ToString() == "b"){
            AWS.BalloonCount ++;
        }
        if(transform.parent.parent.tag == "Crystal"){
            Crystalrb = transform.parent.parent.GetComponent<Rigidbody>();
        }else{
            Crystalrb = transform.parent.parent.parent.GetComponent<Rigidbody>();
        }
        
        MeshRenderer[] MRS = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in MRS){
            if(mr.gameObject != gameObject){
                Rotor = mr.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerScript.isDriving  && AWS.Crystal == Crystalrb.gameObject){
            if(Type == "Wheel"){
                

                RotorRot+=RotorSpeed*InputMultiplier;   

                RotorRot = Mathf.Clamp(RotorRot,-50,50);
                
                if(Input.GetKey(negInput)){
                    InputMultiplier = -1;
                }else if(Input.GetKey(posInput)){
                    InputMultiplier = 1;
                }else{
                    InputMultiplier = 0;
                    if(RotorRot > 0){
                        RotorRot-=RotorSpeed;
                    }else if(RotorRot < 0){
                        RotorRot+=RotorSpeed;
                    }
                }

                Rotor.transform.rotation = Quaternion.AngleAxis(RotorRot,transform.forward)*transform.rotation;
                
            

                playerScript.transform.position = transform.position + transform.forward * 1.3f;
            
            }

            if(Type == "Prop"){
                
                RotorRot+=RotorSpeed*InputMultiplier;
                Rotor.transform.rotation = Quaternion.AngleAxis(RotorRot,transform.forward)*transform.rotation;
                if(Input.GetKey(posInput)){
                    InputMultiplier = 1;
                }else if(Input.GetKey(negInput)){
                    InputMultiplier = -1;
                }else{
                    InputMultiplier = 0;
                }
                
            }

            if(Type[0].ToString() == "b"){
            if(InputMultiplier > -1 && Input.GetKey(negInput)){
                InputMultiplier -=0.05f;
            }else if(InputMultiplier < 1 && Input.GetKey(posInput)){
                InputMultiplier +=0.05f;
            }else{
                InputMultiplier = 0;
            }
        }
        }

        
        
    }
    private void FixedUpdate() {
        
        if(playerScript.isDriving  && AWS.Crystal == Crystalrb.gameObject){
            if(Type == "Prop"){
                Crystalrb.AddForceAtPosition(transform.forward*Power*InputMultiplier,transform.position,ForceMode.Force);
            }
            
            if(Type[0].ToString() == "b"){
                
                
                
                Crystalrb.AddForceAtPosition(Vector3.up*(((Crystalrb.mass * -Physics.gravity.y)/AWS.BalloonCount)+(InputMultiplier*Power)),transform.position,ForceMode.Force);
                

            }
            
        }
    }

    private void OnDestroy() {
        if(Type[0].ToString() == "b"){
            AWS.BalloonCount--;
        }
    }
}
