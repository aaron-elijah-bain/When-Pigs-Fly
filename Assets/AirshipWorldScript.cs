using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirshipWorldScript : MonoBehaviour
{
    //This script controlls happenings in the world
    private Transform cam; // The main camera
    public GameObject[] Blocks; //The spawnable blocks
    public GameObject[] Decks; //Different decks
    public GameObject BaseBlock; //The base of every block

    public int Selected = 0; //Index of selected block for spawning

    public static GameObject SelectedObj; //The block that is selected (Build)

    public static CenterScript SelectedCenter; //The center script of the senter of the selected block

    public Vector3 facePos = Vector3.zero; //The direction to place the block based on which face you clicked

    public Vector3 GhostPos = Vector3.zero; //The position to spawn, or ghost (NTA)
    public Vector3 Rot = Vector3.zero; //The rotation of the block, in vector3 format where 1 = 90 (Re-add)
    public float Mirror = 1; //Weather or not to mirror
    private LayerMask mask; //Masks OldBlock

    public GameObject Crystal; //The Crystal (Make array for more crystals??)
    public float buildRotSpeed = 5f;//Rot speed in build mode
    public float buildZoomSpeed = 5f;//Zoom speed in build mode (Should make build mode controls better)

    Vector3 Center = Vector3.zero;
    
    private float distFromCryst = -10f; //The camera distance from the crystal (Used for storage)
    public bool Building = false; //If you are building

    public float BalloonCount = 0;
    // Start is called before the first frame update
    void Start()
    {
       cam = GameObject.FindWithTag("MainCamera").transform; //Set camera
       mask = LayerMask.GetMask("OldBlock"); //Set old block mask
       
    }

    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Racast to center of screen (Mouse pos)
        RaycastHit hit;
        if(Input.GetKeyDown(KeyCode.B)){//If you press B, then build, or not build. If you build, then set cam pos
            if(Building){
                Building = false;
                Cursor.lockState = CursorLockMode.Locked;//When stop building, lock cursor
                
            }else{
                if(Physics.Raycast(ray,out hit, 20f)){
                    if(hit.transform.gameObject.tag == "Crystal"){
                        Crystal = hit.transform.gameObject;
                    }else if(hit.transform.parent.gameObject.tag == "Crystal"){
                        Crystal = hit.transform.parent.gameObject;
                        
                    
                    }

                    if(Crystal != null){
                        Building = true;
                        Cursor.lockState = CursorLockMode.None;//When start building, unlock cursor
                        Crystal.transform.position = new Vector3(Mathf.Round(Crystal.transform.position.x),2,Mathf.Round(Crystal.transform.position.z));
                        cam.position = Crystal.transform.position + new Vector3(1,-1,2).normalized*distFromCryst;
                        cam.LookAt(Crystal.transform);
                        Center = Crystal.transform.position;

                    }
                }
            }
        }

        if(Building){//If building...
            
            Vector3 Offset = cam.position - Center;
            if(SelectedObj != null){
                Center = SelectedObj.transform.position;
            }

            
            cam.position = Center + Offset;
            cam.LookAt(Center);

            float Horizontal = Input.GetAxisRaw("Horizontal");//Axies for camera movement
            float Vertical = Input.GetAxisRaw("Vertical");
            cam.RotateAround(Center,Vector3.up,-Horizontal*buildRotSpeed);//Rotate around
            cam.RotateAround(Center,cam.right,Vertical*buildRotSpeed);//Up and down
            if(Input.GetKey(KeyCode.Equals) && distFromCryst < -1 ){//Zoom In
                cam.position = cam.position + cam.forward*buildZoomSpeed;
                distFromCryst += buildZoomSpeed;
            }
            if(Input.GetKey(KeyCode.Minus) && distFromCryst > -15 ){//Zoom Out
                cam.position = cam.position - cam.forward*buildZoomSpeed;
                distFromCryst -= buildZoomSpeed;
            }
        

            
            RaycastHit blockHit;
            if(Physics.Raycast(ray, out blockHit,Mathf.Infinity,mask)){
                BlockSideScript Side = blockHit.collider.gameObject.GetComponent<BlockSideScript>();
                if(Side != null){
                    Side.mouseOver = true;
                }
            }

            GhostPos = new Vector3(Mathf.Round(blockHit.point.x),Mathf.Round(blockHit.point.y),Mathf.Round(blockHit.point.z)) + facePos;
        }else{
            Cursor.lockState = CursorLockMode.Locked;
            SelectedObj = null;
        }

        
    }

    private void OnDrawGizmos() {
        if(Crystal != null){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Crystal.GetComponent<Rigidbody>().centerOfMass+Crystal.transform.position,0.5f);
        }
    }
            
       
    
    public void SpawnBlock(Vector3 pos){
        
        Vector3 COM = RecalcCOM(pos);
        
        GameObject NewBlock = Instantiate(BaseBlock,pos,Quaternion.Euler((Rot.x*90),(Rot.y*90),(Rot.z*90)), Crystal.transform) as GameObject;
        GameObject CenterHolder = NewBlock.transform.Find("CenterHolder").gameObject;
        CenterHolder.transform.localScale = new Vector3(Mirror,1,1);
        GameObject center = Instantiate(Blocks[Selected],NewBlock.transform.position,NewBlock.transform.rotation,CenterHolder.transform) as GameObject;
        
        SelectedObj = NewBlock;
        SelectedCenter = center.GetComponent<CenterScript>();

        center.SetActive(true);

        NewBlock.GetComponent<AirshipBlockScript>().center = CenterHolder;
        Rigidbody CrystRB = Crystal.GetComponent<Rigidbody>();
        CrystRB.centerOfMass = COM;
        
        
        
    }

    public Vector3 RecalcCOM(Vector3 SpawnPos){
        Rigidbody CrystRB = Crystal.GetComponent<Rigidbody>();

        float SpawnMass = Blocks[Selected].GetComponent<CenterScript>().Mass;
        Vector3 newCOM = ((CrystRB.centerOfMass * CrystRB.mass) + ((SpawnPos-Crystal.transform.position) * SpawnMass))/(SpawnMass + CrystRB.mass);
        CrystRB.mass += SpawnMass;
        return(newCOM);

    }
}
