using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirshipWorldScript : MonoBehaviour
{
    private Transform cam;
    public GameObject[] Blocks;
    public GameObject[] Decks;
    public GameObject BaseBlock;

    public int Selected = 0;

    public static GameObject SelectedObj;

    public Vector3 facePos = Vector3.zero;

    public Vector3 GhostPos = Vector3.zero;
    public Vector3 Rot = Vector3.zero;
    public float Mirror = 1;
    private LayerMask mask;

    private GameObject Crystal;
    public float buildRotSpeed = 5f;
    public float buildZoomSpeed = 5f;


    
    private float distFromCryst = -10f;
    public bool Building = false;
    // Start is called before the first frame update
    void Start()
    {
       cam = GameObject.FindWithTag("MainCamera").transform;
       mask = LayerMask.GetMask("OldBlock");
       Crystal = GameObject.FindWithTag("Crystal");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.B)){
            if(Building){
                Building = false;
            }else{
                Building = true;
                cam.position = Crystal.transform.position + new Vector3(1,-1,2).normalized*distFromCryst;
                cam.LookAt(Crystal.transform);
            }
        }

        if(Building){
        
            float Horizontal = Input.GetAxisRaw("Horizontal");
            float Vertical = Input.GetAxisRaw("Vertical");
            cam.RotateAround(Crystal.transform.position,Vector3.up,-Horizontal*buildRotSpeed);
            cam.RotateAround(Crystal.transform.position,cam.right,Vertical*buildRotSpeed);
            if(Input.GetKey(KeyCode.Equals) && distFromCryst < -1 ){
                cam.position = cam.position + cam.forward*buildZoomSpeed;
                distFromCryst += buildZoomSpeed;
            }
            if(Input.GetKey(KeyCode.Minus) && distFromCryst > -15 ){
                cam.position = cam.position - cam.forward*buildZoomSpeed;
                distFromCryst -= buildZoomSpeed;
            }
        

            Cursor.lockState = CursorLockMode.None;
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit,Mathf.Infinity,mask)){
                BlockSideScript Side = hit.collider.gameObject.GetComponent<BlockSideScript>();
                if(Side != null){
                    Side.mouseOver = true;
                }
            }

            GhostPos = new Vector3(Mathf.Round(hit.point.x),Mathf.Round(hit.point.y),Mathf.Round(hit.point.z)) + facePos;
        }else{
            Cursor.lockState = CursorLockMode.Locked;
            SelectedObj = null;
        }

        
    }
            
       
    
    public void SpawnBlock(Vector3 pos){
        Quaternion rot = Quaternion.Euler(0,0,0);
       
        
        GameObject NewBlock = Instantiate(BaseBlock,pos,Quaternion.Euler((Rot.x*90)+rot.eulerAngles.x,(Rot.y*90)+rot.eulerAngles.y,(Rot.z*90)+rot.eulerAngles.z), Crystal.transform) as GameObject;
        GameObject CenterHolder = NewBlock.transform.Find("CenterHolder").gameObject;
        CenterHolder.transform.localScale = new Vector3(Mirror,1,1);
        GameObject center = Instantiate(Blocks[Selected],NewBlock.transform.position,NewBlock.transform.rotation,CenterHolder.transform) as GameObject;
        

        center.SetActive(true);

        NewBlock.GetComponent<AirshipBlockScript>().center = CenterHolder;

        
        
    }
}
