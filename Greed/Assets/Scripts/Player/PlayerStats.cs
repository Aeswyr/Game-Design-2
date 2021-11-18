using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    private int[,] playersStats;

    [SerializeField] private TextMeshProUGUI blueGemsP1, blueGemsP2, blueGemsP3, blueGemsP4;
    [SerializeField] private TextMeshProUGUI greenGemsP1, greenGemsP2, greenGemsP3, greenGemsP4;
    [SerializeField] private TextMeshProUGUI redGemsP1, redGemsP2, redGemsP3, redGemsP4;
    [SerializeField] private GameObject blueCrownP1, blueCrownP2, blueCrownP3, blueCrownP4;
    [SerializeField] private GameObject redCrownP1, redCrownP2, redCrownP3, redCrownP4;
    [SerializeField] private GameObject greenCrownP1, greenCrownP2, greenCrownP3, greenCrownP4;
    [SerializeField] private GameObject battleCrownP1, battleCrownP2, battleCrownP3, battleCrownP4;
    [SerializeField] private GameObject PlayerImageP1, PlayerImageP2, PlayerImageP3, PlayerImageP4;

    public void updateCount(int [] input){
        // blueGems.text = input[0].ToString();
        // greenGems.text = input[1].ToString();
        // redGems.text = input[2].ToString();
        
        
        
        // // blueCrown.SetActive(true);
        // // greenCrown.SetActive(true);
        // // redCrown.SetActive(false);
        // // battleCrown.SetActive(false);

        // // if(type== PickupType.GEM_BLUE){
        // //     blueGems.text = gems.ToString();
        // // }else if(type== PickupType.GEM_GREEN){
        // //     greenGems.text = gems.ToString();
        // // }else if(type== PickupType.GEM_RED){
        // //     redGems.text = gems.ToString();
        // // }else 
        


        // if(input[3] == 1){
        //     blueCrown.SetActive(true);
        //     // GameObject.Find("BlueCrown").GetComponent<Image>().enabled = true;
        // }
        // if(input[4] == 1){
        //     greenCrown.SetActive(true);
        // }
        // if(input[5] == 1){
        //     redCrown.SetActive(false);
        // }
        // if(input[6] == 1){
        //     battleCrown.SetActive(false);
            
        // }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        try{
            int [] gems = FindObjectsOfType<PlayerController>()[0].getGems();
            Debug.Log(gems[0].ToString()+" "+gems[1].ToString()+" "+gems[2].ToString());
        }catch( NullReferenceException e){
            Debug.Log(e.ToString());
        }
    }
    void Start(){
        playersStats = new int[4,7];
        // playersStats = new int[4,4]{{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}};
        int [] input = {5,5,5,1,1,1,1}; 
        // if(greenGems!=null)
        updateCount(input);
    }
    void FixedUpdate(){
        
        // playersStats = FindObjectsOfType<PlayerController>()[0].getGems();
        // Debug.Log(FindObjectsOfType<PlayerController>().Length);
    }

    public void UpdateGems(){
        // playersStats = FindObjectsOfType<PlayerController>()[0].getGems();
    }
}
