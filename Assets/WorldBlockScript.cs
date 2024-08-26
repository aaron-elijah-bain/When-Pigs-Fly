using UnityEngine;

public class WorldBlockScript : MonoBehaviour
{

    public int Index;        
    public int SpawnIndex;

    public Mesh[] Meshes;
    
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public BoxCollider Collider;

    private WorldBlockScript North;
    private WorldBlockScript East;
    private WorldBlockScript South;
    private WorldBlockScript West;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        Collider = GetComponent<BoxCollider>();
        Recalcualte();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Recalcualte(){
        SpawnIndex = Index;

        RaycastHit hit;
        if(Physics.Raycast(transform.position+new Vector3(0,20,0) + transform.forward*2, -Vector3.up, out hit, 100f)){
            North = hit.transform.gameObject.GetComponent<WorldBlockScript>();
        }
        if(Physics.Raycast(transform.position+new Vector3(0,20,0)  + transform.right*2, -Vector3.up, out hit, 100f)){
            East = hit.transform.gameObject.GetComponent<WorldBlockScript>();
        }
        if(Physics.Raycast(transform.position+new Vector3(0,20,0)  - transform.forward*2, -Vector3.up, out hit, 100f)){
            South = hit.transform.gameObject.GetComponent<WorldBlockScript>();
        }
        if(Physics.Raycast(transform.position+new Vector3(0,20,0) -transform.right*2, -Vector3.up, out hit, 100f)){
            West = hit.transform.gameObject.GetComponent<WorldBlockScript>();
        }
            
        
        if(Index == 1){
            if(NumOfType(3,"N", "E") >= 1){
                SpawnIndex = 5;
            }
            if(NumOfType(1,"N", "E") == 2){
                SpawnIndex = 0;
            }
            
        }
        if(Index == 3){
            if(NumOfType(1,"S", "W") == 2){
                SpawnIndex = 6;
            }
            if(NumOfType(3,"S", "W") == 2){
                SpawnIndex = 4;
            }
            
        }

        if(SpawnIndex == 0){
            Collider.enabled = false;
        }
        if(SpawnIndex != 0){
            Collider.enabled = true;
            if(SpawnIndex == 1 || SpawnIndex == 5){
            Collider.center = new Vector3(0.5f,5,0.5f);
            Collider.size = new Vector3(1,10,1);
            }
            if(SpawnIndex == 2){
            Collider.center = new Vector3(0,5,0.5f);
            Collider.size = new Vector3(2,10,1);
            }
            if(SpawnIndex == 3 || SpawnIndex == 4 || SpawnIndex == 6){
            Collider.center = new Vector3(0,5,0);
            Collider.size = new Vector3(2,10,2);
            }

            
        }

        meshFilter.mesh = Meshes[SpawnIndex];
        
    }

    public int NumOfType(int Type, string Choice1, string Choice2 ){
        int num = 0;

        if(North != null && (Choice1 == "N" || Choice2 =="N") &&North.Index == Type){
            num ++;
        }
        if(East != null && (Choice1 == "E" || Choice2 =="E") && East.Index == Type){
            num ++;
        }
        if(South != null && (Choice1 == "S" || Choice2 =="S")&& South.Index == Type){
            num ++;
        }
        if(West != null && (Choice1 == "W" || Choice2 =="W") && West.Index == Type){
            num ++;
        }
        return(num);

    }
}
