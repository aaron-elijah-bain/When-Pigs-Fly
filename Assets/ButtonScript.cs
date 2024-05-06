using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public GameObject MyGameObject;
    private int num = 0;
    private  AirshipWorldScript AWS;
    private Image img;

    private Button butt;

    private TextMeshProUGUI text;

    private void Start() {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();

        num = System.Array.IndexOf(AWS.Blocks,MyGameObject);

        text = GetComponentInChildren<TextMeshProUGUI>();

        text.text = gameObject.name;

        img = GetComponent<Image>();
        butt = GetComponent<Button>();

        butt.onClick.AddListener(this.ChangeSelect);
    }
    private void Update() {
        if(AWS.Selected == num){
            img.color = Color.blue;
        }else{
            img.color = Color.white;
        }

    }
    public void ChangeSelect(){
        AWS.Selected = num;
    }
}
