using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.VisualScripting;

public class WorldRecycler : MonoBehaviour
{

    private AirshipWorldScript AWS;

    private GameObject Player;
    public int RenderDistance = 3;
    public int Scale = 4;

    private int playerX = 0;
    private int playerY = 0;

   

    public List<WorldBlockScript> InUse = new List<WorldBlockScript>();
    public List<WorldBlockScript> OutOfUse = new List<WorldBlockScript>();

    
    private int count = 0;
    private int waitFor = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        AWS = GetComponent<AirshipWorldScript>();

        

        playerX = Mathf.RoundToInt(Mathf.Round(Player.transform.position.x / Scale));
        playerY = Mathf.RoundToInt(Mathf.Round(Player.transform.position.z / Scale));



        for(int y = playerY - RenderDistance; y <= playerY + RenderDistance; y++){
            for(int x = playerX - RenderDistance; x <= playerX + RenderDistance; x++){
                MoveBlock(x,y);
            }
        }

       


        
    }

    // Update is called once per frame
    void Update()
    {
        
        count ++;
        if(count > waitFor){
            count = 0;
            UpdateBlocks();
        }


    }
    

    public void UpdateBlocks(){
        Profiler.BeginSample("UpdateBlocks");
        playerX = Mathf.RoundToInt(Mathf.Round(Player.transform.position.x / Scale));
        playerY = Mathf.RoundToInt(Mathf.Round(Player.transform.position.z / Scale));
        
        bool[,] FilledSlots = new bool[(RenderDistance*2) + 1, (RenderDistance *2) + 1];
        int RemovedCount = 0;
        int AddedCount = 0;
        int InsideCount = 0;
        
        for(int i = InUse.Count-1; i >=0; i--){
           
            if(!Inside(
                (InUse[i].transform.position.x/Scale)-playerX,
                (InUse[i].transform.position.z/Scale)-playerY
            )){              
                //Debug.LogError(i + ", " + ((InUse[i].transform.position.x/Scale)-playerX ) + ", "+ ((InUse[i].transform.position.z/Scale)-playerY));
                InUse[i].gameObject.SetActive(false);
                OutOfUse.Add(InUse[i]);
                InUse.RemoveAt(i);

                RemovedCount++;
                
                
            }else{ 
                int relX = Mathf.RoundToInt((InUse[i].transform.position.x/Scale)-playerX)+RenderDistance;
                int relY = Mathf.RoundToInt((InUse[i].transform.position.z/Scale)-playerY)+RenderDistance;

                FilledSlots[relX, relY] = true;

                InsideCount++;
            }
            
        }

        
        
        for(int y = 0; y < FilledSlots.GetLength(1); y++){
            for(int x = 0; x < FilledSlots.GetLength(0); x++){

                if(FilledSlots[x,y] == false){
                    AddedCount++;
                    MoveBlock(x-RenderDistance+playerX,y-RenderDistance+playerY);
                }
            }
        }
        
        Profiler.EndSample();

        
    }

    public void MoveBlock(int x, int y){
        if(x >=  0 && y >= 0 && x < AWS.World.GetLength(0) && y < AWS.World.GetLength(1)){
            WorldBlockScript changer = OutOfUse[0];
            OutOfUse.RemoveAt(0);
            changer.gameObject.SetActive(true);
            InUse.Add(changer);
            changer.transform.position = new Vector3(x * Scale, 0, y * Scale);
            
            

            
            changer.Index = AWS.RuleBlocks[AWS.World[x,y].Chosen].Index;
            changer.transform.rotation = Quaternion.Euler(0,AWS.RuleBlocks[AWS.World[x,y].Chosen].Rot*90,0);
            

            changer.Recalcualte();
            
        }

    }
    public bool Inside(float LocalX, float LocalY){
        return (Mathf.Abs(LocalX) <= RenderDistance) && (Mathf.Abs(LocalY) <= RenderDistance);

    }
}
