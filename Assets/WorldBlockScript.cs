using UnityEngine;

public class WorldBlockScript : MonoBehaviour
{

    private AirshipWorldScript AWS;
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

        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Recalcualte(){
        SpawnIndex = Index;
        
        float height;
        

        if(SpawnIndex == 0){
            Collider.enabled = false;
        }else{

            if(SpawnIndex <= 4){
                height = 5;
            }else{
                height = 10;
            }
            Collider.enabled = true;
            if(SpawnIndex == 1 || SpawnIndex == 5){
            Collider.center = new Vector3(0.5f,height/2,0.5f);
            Collider.size = new Vector3(1,height,1);
            }
            if(SpawnIndex == 2 || SpawnIndex == 6){
            Collider.center = new Vector3(0,height/2,0.5f);
            Collider.size = new Vector3(2,height,1);
            }
            if(SpawnIndex == 3 || SpawnIndex == 7 || SpawnIndex == 4 || SpawnIndex == 8){
            Collider.center = new Vector3(0,height/2,0);
            Collider.size = new Vector3(2,height,2);
            }
            

            
        }

        meshFilter.mesh = Meshes[SpawnIndex];

        //transform.position = new Vector3(0,AWS.RuleBlocks[SpawnIndex].Height,0);
        
    }

}
