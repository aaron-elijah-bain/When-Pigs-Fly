using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSideScript : MonoBehaviour
{   
    //this script takes care of actions on one side of a block. 
    private MeshRenderer mr;//This Sides mesh renderer
    public bool mouseOver = false;//True if the mouse is over this face. Set by the AirshipWorldScript
    private AirshipWorldScript WorldSpawner;//Refrence to the AirshipWorldScript
    private AirshipBlockScript parentScript;//Refrence to this sides parent block's script

    public int DeckIndex = 0; //The index of the deck that should go on this face. Set by this side's parent
    public string DeckRot = ""; //The rotation of the deck that should go on this face. Set by this side's parent


    private GameObject MyDeck; //This blocks deck

    // Start is called before the first frame update
    void Start()
    {   //Set MeshRenderer, ParentScript, and World Spawner
        mr = GetComponent<MeshRenderer>();
        parentScript = transform.parent.gameObject.GetComponent<AirshipBlockScript>();
        WorldSpawner = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
    }

    // Update is called once per frame
    void Update()
    {   
        
        if(mouseOver){ //If the mouse is over this side...
            mr.material.color = PigMoveScript.BlueColor;//Set color to blue
        
            WorldSpawner.facePos = transform.forward; //Used to offset placed block in the direcion this face points.
            if(Input.GetMouseButtonDown(0)){//If you left click...
                WorldSpawner.SpawnBlock(WorldSpawner.GhostPos);//Spawn a block
            }
            if(Input.GetMouseButtonDown(1)){//If you right click...
                if(AirshipWorldScript.SelectedObj == transform.parent.gameObject){//If you are selected...
                    AirshipWorldScript.SelectedObj = null;//Make not selected
                }else{
                    AirshipWorldScript.SelectedObj = transform.parent.gameObject;//If you aren't selected then make selected
                }
            }
            if(Input.GetMouseButtonDown(2)){//If you midle click...
                if(MyDeck == null){//...And you don't have a deck...
                    float oldMirror = parentScript.center.transform.localScale.x;
                    parentScript.center.transform.localScale= new Vector3(1,1,1);
                    Quaternion AlignRot = Quaternion.identity;
                    if(DeckRot == "down"){
                    AlignRot.SetLookRotation(-transform.up,transform.forward);
                    }else if(DeckRot == "up"){
                    AlignRot.SetLookRotation(transform.up,transform.forward);
                    }else if(DeckRot == "right"){
                    AlignRot.SetLookRotation(transform.right,transform.forward);
                    }else if(DeckRot == "left"){
                    AlignRot.SetLookRotation(-transform.right,transform.forward);
                    }
                    if(DeckIndex < 4){//Spawn the block at the index \/  at this position \/  at this rotation plus 90 degress on x (alignment) plus a rotation along this face by Deck Rot as a child to this block
                        MyDeck = Instantiate(WorldSpawner.Decks[DeckIndex],transform.position,AlignRot,parentScript.center.transform) as GameObject;
                    }else if(DeckIndex > 4 && DeckIndex < 7){//Spawn the block at the index-1 (Fill 4-gap), at this position, at this rotation plus 90 degress on x (alignment) plus a rotation along this face by Deck Rot as a child to this block
                        MyDeck = Instantiate(WorldSpawner.Decks[DeckIndex-1],transform.position,AlignRot,parentScript.center.transform) as GameObject;
                    }else if( DeckIndex == 4){//Spawn the block at the index 3  at this position \/  at this rotation plus 90 degress on x (alignment) plus a rotation along this face by Deck Rot as a child to this block
                        MyDeck = Instantiate(WorldSpawner.Decks[3],transform.position,AlignRot,parentScript.center.transform) as GameObject;
                        MyDeck.transform.localScale = new Vector3(-1, 1,1); // And also mirror

                    }else if( DeckIndex == 7){//Spawn the block at the index 5  at this position \/  at this rotation plus 90 degress on x (alignment) plus a rotation along this face by Deck Rot as a child to this block
                        MyDeck = Instantiate(WorldSpawner.Decks[5],transform.position,AlignRot,parentScript.center.transform) as GameObject;
                        MyDeck.transform.localScale = new Vector3(-1, 1,1); // And also mirror

                    }
                    
                    MyDeck.SetActive(true);//Set the deck to be active
                    parentScript.center.transform.localScale= new Vector3(oldMirror,1,1);
                }else{
                    Destroy(MyDeck);//If you already have a deck, then destroy it and set refrence to null
                    MyDeck = null;
                }
            

        }
        }else{//If mouse not over...
            if(WorldSpawner.Building){//...And you are Building...
                mr.material.color = new Color(1,1,1,0.025f);//Set color to compleatly clear
            }else{
                mr.material.color = new Color(1,1,1,0);//Set color to slighly clear


                if(AirshipWorldScript.SelectedObj == transform.parent.gameObject){
                    mr.material.color = PigMoveScript.BlueColor;//If selected, then blue
                }
            }
            
        }

        
        
        mouseOver = false;
    }

    
        
    
    
}
