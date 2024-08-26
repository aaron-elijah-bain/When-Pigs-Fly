using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct shipPart
{
    public int Type;
    public Vector3 Pos;
    public Quaternion Rot;
    public float MyMirror;
    public KeyCode PosInput;
    public KeyCode NegInput;
    public float Speed;
    public bool[] Decks;
    public int GunIndex;




    public shipPart(int type, Vector3 pos, Quaternion rot, float mirror, KeyCode posInput, KeyCode negInput,float speed, bool[] decks, int gunIndex){
        this.Type = type;
        this.Pos = pos;
        this.Rot = rot;
        this.MyMirror = mirror;
        this.PosInput = posInput;
        this.NegInput = negInput;
        this.Speed = speed;
        this.Decks = decks;
        this.GunIndex = gunIndex;


    }
}
[System.Serializable]
public struct ship
{
    public bool[] CrystDecks;
    public List<shipPart> shipParts;

    public ship (bool[] crystDecks, List<shipPart> parts){
        this.CrystDecks = crystDecks;
        this.shipParts = parts;

    }
}

[System.Serializable]
public struct WorldCell
{

    public float[] Entropy;
    public bool Enquened;

    public int Index;
    public int Rot;

    public WorldCell(float[] entropy, bool enquened, int index, int rot){
        this.Entropy = entropy;
        this.Enquened = enquened;
        this.Index = index;
        this.Rot = rot;



    }
}
[System.Serializable]
public struct RuleBlock
{

    public string Sides;
    public string Symmetry;
    public int Index;
    public int Rot;

    public RuleBlock(string sides, string symmetry, int index, int rot){
        this.Sides = sides;
        this.Symmetry = symmetry;
        this.Index = index;
        this.Rot = rot;



    }
}


public class AirshipWorldScript : MonoBehaviour
{
    //This script controlls happenings in the world
    private Transform cam; // The main camera
    private Transform MyCam;
    public GameObject[] Blocks; //The spawnable blocks
    public GameObject[] Decks; //Different decks
    public GameObject[] Guns; //Guns, not blocks, can be placed in blocks
    

    public GameObject WorldBlock; //The World Block
    private WorldCell[,] World = new WorldCell[50,50];

    private List<Vector2> Quene = new List<Vector2>();
    
    public float[] WorldProb = {1,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f,0.5f};
    /*
        -X: No Symmetry, every rotation and mirror changes nothing
        -Q: All Rotations, and mirror is the same as one rotation
        -E: All Rotations, and mirror is the same as two rotations
    */
    public string[] Symmetries = {"X", "Q", "E", "Q", "X"};
    public string[] Rules = {"ssssssss", "pssssssp", "pppssssp", "pppppssp", "pppppppp"};

    private int[] Transfer = {5,4,7,6,1,0,3,2};

    private List<RuleBlock> RuleBlocks = new List<RuleBlock>();

    public float[,] ValidNeighborsN = { 
        
        
    };
    public float[,] ValidNeighborsE = { 
        
        
    };
    public float[,] ValidNeighborsS = { 
        
        
    };
    public float[,] ValidNeighborsW = { 
       
        
    };
    
    public GameObject BaseBlock; //The base of every block

    public int Selected = -1; //Index of selected block for spawning

    public int SelectedGun = 0; //The gun to spawn on click if not zero

    public static List<GameObject> SelectedObj = new List<GameObject>(); //The blocks that are selected (Build)

    public static List<CenterScript> SelectedCenter = new List<CenterScript>(); //The center script of the senter of the selected block

    public Vector3 facePos = Vector3.zero; //The direction to place the block based on which face you clicked

    public Vector3 GhostPos = Vector3.zero; //The position to spawn, or ghost (NTA)
    public Vector3 Rot = Vector3.zero; //The rotation of the block, in vector3 format where 1 = 90 (Re-add)
    public float Mirror = 1; //Weather or not to mirror
    private LayerMask mask; //Masks OldBlock

    public GameObject Crystal; //The Crystal (Make array for more crystals??)

    public float BuildHeight = 10;

    
    public float buildRotSpeed = 5f;//Rot speed in build mode
    public float buildZoomSpeed = 5f;//Zoom speed in build mode (Should make build mode controls better)

    Vector3 Center = Vector3.zero;
    
    private float distFromCryst = -10f; //The camera distance from the crystal (Used for storage)
    private Vector3 nonZoomedCamPos = Vector3.zero;
    public bool Building = false; //If you are building

    public bool MouseOverUI = false; //Used to check if the mouse is over UI


    public void ExpandRuleset(){
        //Adds all of the rules into the RuleBlocks, and then expand them to account for symmetry

        for(int i = 0; i< Rules.Length; i++){
            RuleBlocks.Add(new RuleBlock(Rules[i], Symmetries[i], i ,0));
            ExpandRule(RuleBlocks.Count-1);
        }
    }
    public void ExpandRule(int index){
        //Expands a single rule in the RuleBlocks, using the index of that block. 
        //The new rules will be added directly after the index
        int Rotations = 0;
        //int Mirror = 0;

        if(RuleBlocks[index].Symmetry == "Q"){
            Rotations = 3;
        }
        if(RuleBlocks[index].Symmetry == "E"){
            Rotations = 3;
        }

        string lastRotation = RuleBlocks[index].Sides;
        for(int Rot = 0; Rot< Rotations; Rot++){
            lastRotation = RotateRule(lastRotation);
            RuleBlocks.Add(new RuleBlock(lastRotation, RuleBlocks[index].Symmetry, RuleBlocks[index].Index, Rot+1));
            
        }

    }
    public string RotateRule(string starter){
        string last2 = starter.Substring(starter.Length-2, 2);
        starter = starter.Remove(starter.Length-2);
        return(last2 + starter);
    }
    public void MakeRules(){
        //Finds what can go next to what and adds that to the ValidNeighbors arrays
        int ValidAmount = RuleBlocks.Count;
        ValidNeighborsN = new float[ValidAmount,ValidAmount];
        ValidNeighborsE = new float[ValidAmount,ValidAmount];
        ValidNeighborsS = new float[ValidAmount,ValidAmount];
        ValidNeighborsW = new float[ValidAmount,ValidAmount];

        
        for(int B1 = 0; B1< RuleBlocks.Count; B1++){
            for(int B2 = 0; B2< RuleBlocks.Count; B2++){
                MakeRule(B1,B2);
            }
        }
        

    }
    public void MakeRule(int Block1, int Block2){
        //Finds what can go next to the Index in RuleBlocks, and then adds that info to the array direction
        
        if(CheckConpat(Block1, Block2, 0)){
            ValidNeighborsN[Block1,Block2] = 1;
            ValidNeighborsS[Block2,Block1] = 1;
        }
        if(CheckConpat(Block1, Block2, 2)){
            ValidNeighborsE[Block1,Block2] = 1;
            ValidNeighborsW[Block2,Block1] = 1;
        }
        if(CheckConpat(Block1, Block2, 4)){
            ValidNeighborsS[Block1,Block2] = 1;
            ValidNeighborsN[Block2,Block1] = 1;
        }
        if(CheckConpat(Block1, Block2, 6)){
            ValidNeighborsW[Block1,Block2] = 1;
            ValidNeighborsE[Block2,Block1] = 1;
        }
        
    }

    public bool CheckConpat(int Block1, int Block2, int Side){
        return(RuleBlocks[Block1].Sides[Side] == RuleBlocks[Block2].Sides[Transfer[Side]] && RuleBlocks[Block1].Sides[Side+1] == RuleBlocks[Block2].Sides[Transfer[Side+1]]);
        
    }
    
    
    public List<int> FindOptions(int x, int y){
        List<int> Options = new List<int>();

        for(int i = 0; i< World[x,y].Entropy.Length; i++){
            if(World[x,y].Entropy[i] != 0){
                Options.Add(i);
            }
        }
        return(Options);
    }

    public int WeightedRandom(int x, int y ){
        List<int> MyOptions = FindOptions(x,y);

        float total = 0;
        foreach(int i in MyOptions){
            total += WorldProb[i];
        }
        float random = Random.Range(0,total);
        float counter = 0;

        foreach(int i in MyOptions){
            counter += WorldProb[i];
            if(random <= counter ){
                return(i);
            }
        }
        return(0);
    }
    public void Collapse( int x, int y){
        
        int Chosen = WeightedRandom(x,y);

        
        
        for(int i = 0; i< World[x,y].Entropy.Length; i++){
            if(i == Chosen){
                World[x,y].Entropy[i] = 1;
                World[x,y].Index = RuleBlocks[i].Index;
                World[x,y].Rot = RuleBlocks[i].Rot;
            }else{
                World[x,y].Entropy[i] = 0;
            }
        }

        
        
        //GameObject newWorldPart = Instantiate(WorldParts[Chosen], new Vector3(x*5,0,y*5), Quaternion.identity);

        //newWorldPart.SetActive(true);

        Process(x,y);
        
       
       
        
    }

    public bool IsCompatible( float[] AOptions, int OptionB, float[,] RuleArray){
        for(int i = 0; i< AOptions.Length; i++){

            if(AOptions[i] > 0 && RuleArray[i,OptionB] > 0){
                return(true);
            }
        }
        return(false);
    }

    public void ProcessSingle(int x, int y, float[] AOptions, float[,] RuleArray){
        if(World[x, y].Enquened == false){
        bool hasChanged = false;
        for(int OptionB = 0; OptionB < AOptions.Length; OptionB++){

            if(World[x,y].Entropy[OptionB] > 0 && FindOptions(x,y).Count > 1){
                if(!IsCompatible(AOptions, OptionB, RuleArray)){
                    World[x,y].Entropy[OptionB] = 0;
                    hasChanged = true;
                    
                    
                }
            }
        }
        if(FindOptions(x,y).Count == 0){
            Debug.LogError("No Options at " + x + ", " + y);
        }
        if(hasChanged){
            Quene.Add(new Vector2(x,y));
            World[x, y].Enquened = true;
        }
        }
            
    }
    public void Process(int x, int y){
        

        //Debug.Log(AddedEntropy[0] + ", " + AddedEntropy[1] + ", " + AddedEntropy[2] + ", " + AddedEntropy[3] + ", " + AddedEntropy[4] + ", " + AddedEntropy[5]);
        
        if(x + 1 < World.GetLength(0)){
            ProcessSingle(x + 1, y, World[x,y].Entropy, ValidNeighborsE);
        }
        if(y + 1 < World.GetLength(1)){
            ProcessSingle(x, y + 1, World[x,y].Entropy, ValidNeighborsN);
        }
        
        if(x - 1 >= 0){
            ProcessSingle(x - 1, y, World[x,y].Entropy, ValidNeighborsW);
        }
        
        if(y - 1 >= 0){
            ProcessSingle(x, y - 1, World[x,y].Entropy, ValidNeighborsS);
        }
        
        
    }

    
    // Start is called before the first frame update
    void Start()
    {

        ExpandRuleset();
        MakeRules();
        foreach(RuleBlock block in RuleBlocks){
            //Debug.Log(block.Sides + ", " + block.Symmetry+ ", "+ block.Index);
        }
        foreach(int x in ValidNeighborsW){
            //Debug.Log(x);
        }
        //Set starting entropy of each block
        for(int x = 0; x<World.GetLength(0); x++){
            for(int y = 0; y<World.GetLength(1); y++){
                World[x,y].Entropy = new float[RuleBlocks.Count];
                for(int i = 0; i<World[x,y].Entropy.Length; i++){
                    
                    World[x,y].Entropy[i] = 1;
                }
            }
        }
        
        //Collapse!!
        
        for(int i = 0; i<World.GetLength(0)*World.GetLength(1); i++){
            float CurrentLeast = 0;
            List<Vector2> CurrentLeastPositions = new List<Vector2>();
            for(int x = 0; x<World.GetLength(0); x++){
                for(int y = 0; y<World.GetLength(1); y++){
                    World[x,y].Enquened = false;
                    float totalEntropy = 0;
                    for(int num = 0; num < World[x,y].Entropy.Length; num++){
                        totalEntropy += World[x,y].Entropy[num];
                    }

                    if(totalEntropy > 1){
                        if(CurrentLeast == 0){
                            CurrentLeast = totalEntropy;
                            CurrentLeastPositions.Add(new Vector2(x,y));
                        }else{
                            if(totalEntropy == CurrentLeast){
                                CurrentLeastPositions.Add(new Vector2(x,y));
                            }else if(totalEntropy < CurrentLeast){
                                CurrentLeast = totalEntropy;
                                CurrentLeastPositions.Clear();
                                CurrentLeastPositions.Add(new Vector2(x,y));
                            }

                        }
                    }
                    
                }
            }

            
            
            
            if(CurrentLeast == 0){
                
                for(int X = 0; X<World.GetLength(0); X++){
                    for(int Y = 0; Y<World.GetLength(1); Y++){
                        int index = 0;
                        int Rot = 0;
                        for(int I = 0; I< World[X,Y].Entropy.Length; I++){
                            if(World[X,Y].Entropy[I] == 1){
                                index = RuleBlocks[I].Index;
                                Rot = RuleBlocks[I].Rot;
                            }
                        }
                        Debug.Log(index);
                        GameObject newWorldPart = Instantiate(WorldBlock, new Vector3(X*2,0,Y*2), Quaternion.Euler(0,Rot*90,0));

                        newWorldPart.SetActive(true);
                        newWorldPart.GetComponent<WorldBlockScript>().Index = World[X,Y].Index;
                        
                    }
                }
                
                break;
            }else{
                Vector2 CollapsingPos = new Vector2(2,2);
                if(i != 0){
    
                    CollapsingPos = CurrentLeastPositions[Random.Range(0, CurrentLeastPositions.Count)];
                }
            
                Collapse(Mathf.RoundToInt(CollapsingPos.x), Mathf.RoundToInt(CollapsingPos.y));

                while(Quene.Count > 0){
                    Vector2 myPos = Quene[0];
                    Quene.RemoveAt(0);
                    Process(Mathf.RoundToInt(myPos.x),Mathf.RoundToInt(myPos.y));
                    
                }
            }

            
            
            
        }
        

        

       cam = GameObject.FindWithTag("MainCamera").transform.parent; //Set camera
       MyCam = GameObject.FindWithTag("MainCamera").transform;
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
                MyCam.localPosition = Vector3.zero;
                MyCam.localRotation = Quaternion.Euler(0,0,0);

                

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
                        Crystal.transform.position = new Vector3(Mathf.Round(Crystal.transform.position.x),BuildHeight,Mathf.Round(Crystal.transform.position.z));
                        distFromCryst = -10;
                        cam.rotation = Quaternion.Euler(0,90,0);
                        MyCam.LookAt(Crystal.transform);
                        Center = Crystal.transform.position;


                    }
                }
            }
        }

        if(Building){//If building...
            
            if(EventSystem.current.IsPointerOverGameObject()){
                MouseOverUI = true;

            }else{
                MouseOverUI = false;
            }
            
            
            

            Vector3 total = Vector3.zero;
            if(SelectedObj.Count != 0){
                foreach(GameObject Obj in SelectedObj){
                    if(Obj != null){
                        total += Obj.transform.position;
                    }   
                }
            }
            if(SelectedObj.Count != 0){
                Center = total/SelectedObj.Count;
            }


            
            
            
                       
            cam.position = Center;
            
            
            

            if(MouseOverUI == false){

                float Horizontal = Input.GetAxisRaw("Horizontal");//Axies for camera movement
                float Vertical = Input.GetAxisRaw("Vertical");
                
                cam.rotation = Quaternion.AngleAxis( -Horizontal*buildRotSpeed*Time.deltaTime, Vector3.up) * cam.rotation;
                cam.rotation = Quaternion.AngleAxis(Vertical*buildRotSpeed*Time.deltaTime, MyCam.right) * cam.rotation;
                
                if(Input.GetKey(KeyCode.Equals) && distFromCryst < -1 ){//Zoom In
                
                    distFromCryst += buildZoomSpeed;
                }
                if(Input.GetKey(KeyCode.Minus) && distFromCryst > -15 ){//Zoom Out
                
                    distFromCryst -= buildZoomSpeed;
                }

                if(Input.GetKeyDown(KeyCode.R)){
                    Center = Crystal.transform.position;
                    SelectedObj = new List<GameObject>();
                }
            
            }
            
            MyCam.position =  cam.position + (MyCam.forward*distFromCryst);
            MyCam.LookAt(cam.position);

            
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
            SelectedObj = new List<GameObject>();
        }

        
    }

    private void OnDrawGizmos() {
        if(Crystal != null){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Crystal.transform.TransformPoint(Crystal.GetComponent<Rigidbody>().centerOfMass),0.5f);
        Gizmos.color = Color.yellow;
        

        }
    }
            
       
    
    public void SpawnBlock(shipPart Part, GameObject Parent){
        if(Parent != null){
        AirshipBlockScript ParentScript = Parent.GetComponent<AirshipBlockScript>();
        
        ParentScript.myCrystal.ActualCOM = RecalcCOM(Part.Pos, Blocks[Part.Type].GetComponent<CenterScript>().Mass,1,ParentScript);
        
        GameObject NewBlock = Instantiate(BaseBlock,Part.Pos,Part.Rot, Parent.transform) as GameObject;
        AirshipBlockScript BScript = NewBlock.GetComponent<AirshipBlockScript>();
        BScript.crystal = Parent.GetComponent<AirshipBlockScript>();
        BScript.myDecks = Part.Decks;
        BScript.gunIndex = Part.GunIndex;
        
        GameObject CenterHolder = NewBlock.transform.Find("CenterHolder").gameObject;
        BScript.Index = Part.Type;
        CenterHolder.transform.localScale = new Vector3(Part.MyMirror,1,1);
        GameObject center = Instantiate(Blocks[Part.Type],NewBlock.transform.position,NewBlock.transform.rotation,CenterHolder.transform) as GameObject;
        CenterScript CScript = center.GetComponent<CenterScript>();
        CScript.Crystalrb = Parent.GetComponent<Rigidbody>();
        CScript.posInput = Part.PosInput;
        CScript.negInput = Part.NegInput;
        CScript.Speed = Part.Speed;
        
        
        
        SelectedObj = new List<GameObject>();
        SelectedObj.Add(NewBlock);
        SelectedCenter = new List<CenterScript>();
        SelectedCenter.Add(CScript);

        center.SetActive(true);

        NewBlock.GetComponent<AirshipBlockScript>().center = CenterHolder;

        
        

        Rigidbody CrystRB = Parent.GetComponent<Rigidbody>();

        CrystRB.centerOfMass = new Vector3(Mathf.Round(ParentScript.myCrystal.ActualCOM.x),Mathf.Round(ParentScript.myCrystal.ActualCOM.y),Mathf.Round(ParentScript.myCrystal.ActualCOM.z));

    
        }
        
        
        
    }

    public Vector3 RecalcCOM(Vector3 SpawnPos, float SpawnMass, float sign, AirshipBlockScript Cryst){
        
        Rigidbody CrystRB = Cryst.gameObject.GetComponent<Rigidbody>();
        //Only works because cryst rot is the same as world rot
        Vector3 newCOM = ((Cryst.myCrystal.ActualCOM * CrystRB.mass) + ((CrystRB.transform.InverseTransformPoint(SpawnPos)) * SpawnMass)*sign)/((SpawnMass*sign) + CrystRB.mass);
        CrystRB.mass += SpawnMass*sign;
        return(newCOM);

    }
}

