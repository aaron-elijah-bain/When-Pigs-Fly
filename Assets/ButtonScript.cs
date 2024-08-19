using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public string Type = "";
    public GameObject MyGameObject;
    public int num = 0;
    private  AirshipWorldScript AWS;
    private Image img;

    private Button butt;

    private TextMeshProUGUI text;

    private void Start() {
        
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();

        text = GetComponentInChildren<TextMeshProUGUI>();

        if(Type == "Block"){
        num = System.Array.IndexOf(AWS.Blocks,MyGameObject);
        
        

        }
        
        img = GetComponent<Image>();
        text.text = gameObject.name;
        butt = GetComponent<Button>();

        butt.onClick.AddListener(this.ChangeSelect);
    }
    private void Update() {
        if(Type == "Block"){
            if(AWS.Selected == num){
                img.color = Color.blue;
            }else{
                img.color = Color.white;
            }
        }
        if(Type == "Gun"){
            if(AWS.SelectedGun == num){
                img.color = Color.blue;
            }else{
                img.color = Color.white;
            }
        }

    }
    public void ChangeSelect(){
        if(Type == "Block"){
        AWS.Selected = num;
        AWS.SelectedGun = -1;
        }
        if(Type == "Gun"){
            AWS.Selected = -1;
            AWS.SelectedGun = num;
        }
    }
}
