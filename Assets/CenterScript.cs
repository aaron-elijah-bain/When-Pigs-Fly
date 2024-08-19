using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterScript : MonoBehaviour
{
    public int[] Sides;
    public string[] Rots;

    public string Type = "";

    public float Mass = 0f;

    public PigMoveScript playerScript;

    public Rigidbody Crystalrb;
    private AirshipBlockScript CrystScript;
    

    private MeshRenderer MyRenderer;
    private List<GameObject> MyParts = new List<GameObject>();

    private float RotorRot = 0f;

    public float RotorSpeed = 5f;

    public float Speed = 1f;

    public float PowerUsage = 0;

    public float InputMultiplier = 0;

    public KeyCode posInput;
    public KeyCode negInput;

    public bool isAutomatic = false;

    public AirshipWorldScript AWS;

    


    public bool[] UINeeds = new bool[3];


    
    // Start is called before the first frame update
    void Awake()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        Type = gameObject.name;
        Type = Type.Replace("(Clone)","");
        playerScript = GameObject.FindWithTag("Player").GetComponent<PigMoveScript>();
        
        MyRenderer = GetComponent<MeshRenderer>();

        
        
        if(transform.parent.parent.tag == "Crystal"){
            Crystalrb = transform.parent.parent.GetComponent<Rigidbody>();

            
        }else if(transform.parent.parent.parent != null){
            Crystalrb = transform.parent.parent.parent.GetComponent<Rigidbody>();

            
        }
        CrystScript = Crystalrb.gameObject.GetComponent<AirshipBlockScript>();
        

        
        if(transform.parent.parent.tag != "Crystal"){
            CrystScript.myCrystal.TotalPowerUsage += PowerUsage;
        }

        
        MeshRenderer[] MRS = GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i< MRS.Length; i++){
            if(MRS[i].gameObject != gameObject && MRS[i].transform.parent == transform){
                
                MyParts.Add(MRS[i].gameObject);
                
            }
        }
        

        
    }
    void Start(){
        if(Type[0].ToString() == "b"){
            CrystScript.myCrystal.BalloonCount ++;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        
        
        MyRenderer.material.SetVector("_CrystPos", transform.parent.parent.localEulerAngles + transform.localEulerAngles);
        MyRenderer.material.SetFloat("_Mirror", transform.parent.localScale.x);


        

        if(playerScript.isDriving  && AWS.Crystal == Crystalrb.gameObject && AWS.Building == false){
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

                MyParts[0].transform.rotation = Quaternion.AngleAxis(RotorRot,transform.forward)*transform.rotation;
                
            

                playerScript.transform.position = transform.position + transform.forward * 1.3f;

            
            }

            if(Type == "Prop" || Type == "Prop1"){
                
                RotorRot+=RotorSpeed*InputMultiplier;
                MyParts[0].transform.rotation = Quaternion.AngleAxis(RotorRot,transform.forward)*transform.rotation;
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

        if(Type == "Prop" || Type == "Prop1"){
                RaycastHit[] hits;
                hits = Physics.RaycastAll(transform.position, -transform.forward, 2, LayerMask.GetMask("Blocks"));
                
                if(MyParts.Count > 1 ){
                    if(hits.Length != 0){
                        foreach(RaycastHit hit in hits){
                            if(hit.transform.gameObject != gameObject){
                                MyParts[1].SetActive(true);
                                Vector3 PropPos = transform.position-transform.forward;
                                Vector3 AimPos = hit.point + (Crystalrb.transform.position-PropPos).normalized*2;
                                
                                MyParts[1].transform.localScale = new Vector3(0.2f,(AimPos-PropPos).magnitude/2,0.2f);
                                
                                MyParts[1].transform.position = ((PropPos + AimPos)/2);

                                MyParts[1].transform.rotation = Quaternion.FromToRotation(transform.up,AimPos-PropPos);
                                

                            }
                        }
                    }else{
                        MyParts[1].SetActive(false);
                    }
            }

            
        }
        


        
        
        
    }
    private void FixedUpdate() {
        
        if(playerScript.isDriving  && AWS.Crystal == Crystalrb.gameObject && AWS.Building == false && CrystScript.myCrystal.TotalPowerUsage < CrystScript.myCrystal.MaxPower){
            if(Type == "Prop" || Type == "Prop1"){
                Crystalrb.AddForceAtPosition(transform.forward*Speed*InputMultiplier,transform.position,ForceMode.Force);
            }
            
            if(Type[0].ToString() == "b"){
                
                
                float MyForce = ((Crystalrb.mass * -Physics.gravity.y)/CrystScript.myCrystal.BalloonCount)+(InputMultiplier*Speed);
                
                MyForce = Mathf.Clamp(MyForce,0,500);
                Crystalrb.AddForceAtPosition(Vector3.up*MyForce,transform.position,ForceMode.Force);
                

            }
            
        }
        
    }

    private void OnDestroy() {
        if(Type[0].ToString() == "b"){
            CrystScript.myCrystal.BalloonCount--;
        }
        if(Type == "Wheel" && AWS.Crystal == Crystalrb.gameObject){
                playerScript.isDriving = false;
        }

        CrystScript.myCrystal.TotalPowerUsage -= PowerUsage;
    }
}
