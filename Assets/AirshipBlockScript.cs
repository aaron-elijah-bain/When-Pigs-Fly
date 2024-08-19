using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipBlockScript : MonoBehaviour
{
    //This script is the base for all blocks

    public int Index;
    public Rigidbody rb;

    public List<GameObject> neighbors;
    public List<AirshipBlockScript> neighborScripts;

    private LayerMask Ground;

    public bool connected = true;

    public AirshipBlockScript crystal;
    public AirshipCrystalScript myCrystal; //For crystal only. If children need a reference, reference this variable of the above variable. 
    
    private AirshipWorldScript AWS;

    public GameObject center;
    public CenterScript centerScript;

    private BlockSideScript[] Sides;
        
    private bool firstFrame = true;

    public bool OldBuild = true;

    

    public bool[] myDecks = new bool[6];

    

    public float BlockHeath = 100;

    public int gunIndex = 0;
    public GameObject Gun;

    


    

    // Start is called before the first frame update
    void Start()
    {   
        
        

        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        neighbors = new List<GameObject>();
        neighborScripts = new List<AirshipBlockScript>();

        
        Sides = GetComponentsInChildren<BlockSideScript>();
        if(gameObject.tag == "Crystal"){
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.automaticCenterOfMass = false;
            rb.maxAngularVelocity = 1;

            rb.inertiaTensor = new Vector3(100,10,100);
            crystal = this;

            myCrystal = GetComponent<AirshipCrystalScript>();
            myCrystal.SpawnShip();


            
        }else{
            transform.SetParent(crystal.transform);
            
        }
        centerScript = center.GetComponentInChildren<CenterScript>();
        
        SpawnGun(gunIndex);
        
        

        LayerMask mask = LayerMask.GetMask("OldBlock");
        Ground = LayerMask.GetMask("Ground");

        

        

        gameObject.layer = LayerMask.NameToLayer("Blocks");

        for(int i = 0; i < Sides.Length; i++){
            Sides[i].gameObject.layer = LayerMask.NameToLayer("NewBlock");
        }
        
        Vector3 SideOffset = Vector3.zero;
        Vector3 dir = Vector3.zero;

        
        for(int i = 0; i< 6; i++){
            Vector3 perp1 = Vector3.zero;
            Vector3 perp2 = Vector3.zero;
            

            
            if(i == 0){
                dir = transform.forward;
                perp1 = transform.right;
                perp2 = transform.up;
            }else if(i == 1){
                dir = -transform.forward;
                perp1 = transform.right;
                perp2 = transform.up;
            }else if(i == 2){
                dir = transform.right;
                perp1 = transform.up;
                perp2 = transform.forward;
            }else if(i == 3){
                dir = -transform.right;
                perp1 = transform.up;
                perp2 = transform.forward;
            }else if(i == 4){
                dir = transform.up;
                perp1 = transform.forward;
                perp2 = transform.right;
            }else if(i == 5){
                dir = -transform.up;
                perp1 = transform.forward;
                perp2 = transform.right;
            }

            for(int j = 0; j< 4; j++){
                
                if(j == 0){
                    SideOffset = perp1*0.9f + perp2*0.9f;
                }else if(j == 1){
                    SideOffset = perp1*-0.9f + perp2*0.9f;
                }else if(j == 2){
                    SideOffset = perp1*-0.9f + perp2*-0.9f;
                }else if(j == 3){
                    SideOffset = perp1*0.9f + perp2*-0.9f;
                }
                RaycastHit hit = new RaycastHit();
           
                bool DidHit = false;

                DidHit = Physics.Raycast(transform.position+SideOffset,dir,out hit, 1.2f, mask);

                if(DidHit){
                    if(hit.transform.parent != null){
                        if(!neighbors.Contains(hit.transform.parent.gameObject)){
                            neighbors.Add(hit.transform.parent.gameObject);
                            neighborScripts.Add(hit.transform.parent.gameObject.GetComponent<AirshipBlockScript>());
                        }
                    
                    
                    
                    }
                }

            }
        }

        


        

        for(int i = 0; i<neighbors.Count; i++){
            AirshipBlockScript Neighbor = neighbors[i].GetComponent<AirshipBlockScript>();
           
            if(!Neighbor.neighbors.Contains(gameObject)){
                
                Neighbor.neighbors.Add(gameObject);
                Neighbor.neighborScripts.Add(this);
               
            }

            
        }

        

        
        for(int i = 0; i < Sides.Length; i++){

            Sides[i].gameObject.layer = LayerMask.NameToLayer("OldBlock");

            BlockSideScript BSS = Sides[i];
            

            if(Sides[i].gameObject.name == "Front"){
                BSS.DeckIndex = centerScript.Sides[0];
                BSS.DeckRot = centerScript.Rots[0];
                BSS.MyIndex = 0;
                if(myDecks[0]){
                    BSS.SpawnDeck();
                }
            }
            if(Sides[i].gameObject.name == "Back"){
                BSS.DeckIndex = centerScript.Sides[1];
                BSS.DeckRot = centerScript.Rots[1];
                BSS.MyIndex = 1;
                if(myDecks[1]){
                    BSS.SpawnDeck();
                }
            }
            if(Sides[i].gameObject.name == "Right"){
                BSS.DeckIndex = centerScript.Sides[2];
                BSS.DeckRot = centerScript.Rots[2];
                BSS.MyIndex = 2;
                if(myDecks[2]){
                    BSS.SpawnDeck();
                }
            }
             if(Sides[i].gameObject.name == "Left"){
                BSS.DeckIndex = centerScript.Sides[3];
                BSS.DeckRot = centerScript.Rots[3];
                BSS.MyIndex = 3;
                if(myDecks[3]){
                    BSS.SpawnDeck();
                }
            }
            if(Sides[i].gameObject.name == "Top"){
                BSS.DeckIndex = centerScript.Sides[4];
                BSS.DeckRot = centerScript.Rots[4];
                BSS.MyIndex = 4;
                if(myDecks[4]){
                    BSS.SpawnDeck();
                }
            }
             if(Sides[i].gameObject.name == "Bottom"){
                BSS.DeckIndex = centerScript.Sides[5];
                BSS.DeckRot = centerScript.Rots[5];
                BSS.MyIndex = 5;
                if(myDecks[5]){
                    BSS.SpawnDeck();
                }
            }
        }

        

        

        AirshipBlockScript[] crystalChildren = crystal.gameObject.GetComponentsInChildren<AirshipBlockScript>();

        foreach(AirshipBlockScript child in crystalChildren){
            child.connected = false;
        }
        crystal.MakeConnected();

        foreach(AirshipBlockScript child in crystalChildren){
            if(!child.connected){
            child.transform.SetParent(null, true);
            }
        }


        transform.localPosition = new Vector3(Mathf.Round(transform.localPosition.x),Mathf.Round(transform.localPosition.y),Mathf.Round(transform.localPosition.z));

        transform.rotation = Quaternion.Euler(Mathf.Round(transform.rotation.eulerAngles.x/90)*90,Mathf.Round(transform.rotation.eulerAngles.y/90)*90,Mathf.Round(transform.rotation.eulerAngles.z/90)*90);


    }

    // Update is called once per frame
    void Update()
    {
        
        if(BlockHeath <= 0){
            if(BlockHeath < 0){
                foreach(AirshipBlockScript Neighbor in neighborScripts){
                    Neighbor.BlockHeath -= (-BlockHeath)/neighborScripts.Count;
                }
            }
            Destroy(gameObject);
        }

        if(OldBuild != AWS.Building && AWS.Building == true){
            foreach(BlockSideScript Side in Sides){
                Side.gameObject.SetActive(true);
            }
        }else if(OldBuild != AWS.Building && AWS.Building == false){
            foreach(BlockSideScript Side in Sides){
                Side.gameObject.SetActive(false);
            }
        }

        
        
        


       

        foreach(GameObject Obj in AirshipWorldScript.SelectedObj){
            if(Obj == gameObject && gameObject.tag != "Crystal" && AWS.MouseOverUI == false){
                if(Input.GetKey(KeyCode.Backspace)){
                    Destroy(gameObject);
                }

                if(Input.GetKeyDown(KeyCode.X)){
                    transform.Rotate(90f,0f,0f,Space.World);
                }
                if(Input.GetKeyDown(KeyCode.Y)){
                    transform.Rotate(0f,90f,0f,Space.World);            
                }
                if(Input.GetKeyDown(KeyCode.Z)){
                    transform.Rotate(0f,0f,90f,Space.World);            
                }
                

                if(Input.GetKeyDown(KeyCode.M)){
                    
                    
                    center.transform.localScale = new Vector3(-center.transform.localScale.x,1,1);
                    
                    
                }

                AWS.Mirror = center.transform.localScale.x;





        }
        }

        Vector3 SideOffset = Vector3.zero;
        Vector3 dir = Vector3.zero;

        
        for(int i = 0; i< 6; i++){

            Vector3 perp1 = Vector3.zero;
            Vector3 perp2 = Vector3.zero;

            
            if(i == 0){
                dir = transform.forward;
                perp1 = transform.right;
                perp2 = transform.up;
            }else if(i == 1){
                dir = -transform.forward;
                perp1 = transform.right;
                perp2 = transform.up;
            }else if(i == 2){
                dir = transform.right;
                perp1 = transform.up;
                perp2 = transform.forward;
            }else if(i == 3){
                dir = -transform.right;
                perp1 = transform.up;
                perp2 = transform.forward;
            }else if(i == 4){
                dir = transform.up;
                perp1 = transform.forward;
                perp2 = transform.right;
            }else if(i == 5){
                dir = -transform.up;
                perp1 = transform.forward;
                perp2 = transform.right;
            }

            for(int j = 0; j< 4; j++){
                
                if(j == 0){
                    SideOffset = perp1*0.9f + perp2*0.9f;
                }else if(j == 1){
                    SideOffset = perp1*-0.9f + perp2*0.9f;
                }else if(j == 2){
                    SideOffset = perp1*-0.9f + perp2*-0.9f;
                }else if(j == 3){
                    SideOffset = perp1*0.9f + perp2*-0.9f;
                }

                //Debug.DrawRay(transform.position+SideOffset,dir,Color.red, 1.2f);
            }
           


        }

        if(firstFrame){
            transform.position = new Vector3(Mathf.Round(transform.position.x),Mathf.Round(transform.position.y),Mathf.Round(transform.position.z));

            transform.rotation = Quaternion.Euler(Mathf.Round(transform.rotation.eulerAngles.x/90)*90,Mathf.Round(transform.rotation.eulerAngles.y/90)*90,Mathf.Round(transform.rotation.eulerAngles.z/90)*90);
            firstFrame = false;

        }
        
        OldBuild = AWS.Building; 
       
    }

    public void SpawnGun(int Index){
        if(Gun != null && Index == 0 && gunIndex != 0){

            Destroy(Gun);
            gunIndex = 0;

        }else if(Gun == null && Index != 0){
            GameObject myGun = Instantiate(AWS.Guns[Index-1], transform.position, transform.localRotation, centerScript.transform);
            myGun.SetActive(true);
            //myGun.transform.rotation = Quaternion.AngleAxis(90, transform.forward);
            Gun = myGun;
            gunIndex = Index;
        }else if(Gun != null && Index != gunIndex && Index != 0){
            Destroy(Gun);
            GameObject myGun = Instantiate(AWS.Guns[Index-1], transform.position, transform.localRotation, centerScript.transform);
            myGun.SetActive(true);
            //myGun.transform.rotation = Quaternion.AngleAxis(90, transform.forward);
            Gun = myGun;
            gunIndex = Index;
        }

        
    }

    public void MakeConnected(){
        connected = true;
        for(var i = 0; i< neighborScripts.Count; i++){
            if(!neighborScripts[i].connected){
                neighborScripts[i].MakeConnected();
            }
        }
    }
    
    private void OnDestroy() {
        for(int i = 0; i<neighborScripts.Count; i++){
            int index = neighborScripts[i].neighbors.IndexOf(gameObject);
            neighborScripts[i].neighbors.RemoveAt(index);
            neighborScripts[i].neighborScripts.RemoveAt(index);
        }

        AirshipBlockScript[] crystalChildren;

       
        crystalChildren = crystal.gameObject.GetComponentsInChildren<AirshipBlockScript>();
        

        foreach(AirshipBlockScript child in crystalChildren){
            child.connected = false;
        }
        crystal.MakeConnected();

        Rigidbody CrystRb = crystal.GetComponent<Rigidbody>();
        
        foreach(AirshipBlockScript child in crystalChildren){
            if(!child.connected){
                
                crystal.myCrystal.ActualCOM = AWS.RecalcCOM(child.transform.position, child.centerScript.Mass,-1, crystal);
        
                

                CrystRb.centerOfMass = new Vector3(Mathf.Round(crystal.myCrystal.ActualCOM.x),Mathf.Round(crystal.myCrystal.ActualCOM.y),Mathf.Round(crystal.myCrystal.ActualCOM.z));
                
                
               
                child.rb = child.gameObject.AddComponent<Rigidbody>();
                child.rb.mass = 1; //Change if more mass is needed, make this a variable
                
            }
        }
                                
        foreach(AirshipBlockScript child in crystalChildren){
            if(!child.connected){

               
                    
                
                for(var i = 0; i < child.neighborScripts.Count; i++){
                    AirshipBlockScript MyNeighbor = child.neighborScripts[i];

                    
                    if(MyNeighbor.gameObject.tag != "Crystal"){
                        FixedJoint FJ = child.gameObject.AddComponent<FixedJoint>();
                        FJ.connectedBody = MyNeighbor.rb;
                        FJ.breakForce = Mathf.Infinity;
                        FJ.breakTorque = Mathf.Infinity;
                        FJ.enableCollision = true;
                    }

                }

                
                    
                
            child.transform.SetParent(null, true);
            }
        }
        if(transform.parent != null){

                
                crystal.myCrystal.ActualCOM = AWS.RecalcCOM(transform.position, centerScript.Mass,-1,crystal);
        
                

                CrystRb.centerOfMass = new Vector3(Mathf.Round(crystal.myCrystal.ActualCOM.x),Mathf.Round(crystal.myCrystal.ActualCOM.y),Mathf.Round(crystal.myCrystal.ActualCOM.z));
                
                foreach(GameObject myObj in AirshipWorldScript.SelectedObj){
                    if(myObj == gameObject){//If you are selected...
                        AirshipWorldScript.SelectedObj.Remove(gameObject);//Make not selected
                        AirshipWorldScript.SelectedCenter.Remove(centerScript);
                        return;
                    
                    }
                }

        }


        

    }
}
