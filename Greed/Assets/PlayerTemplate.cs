using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] Gems; // blue, green, red
    [SerializeField] private GameObject[] Crowns;
    [SerializeField] private Image Player;
    [SerializeField] private TextMeshProUGUI Winner;

    private int time;
    public int callOnce;
    public int WinnerTime = 20;
    
    
    
    private PlayerStats stats;
    private PlayerData data;
    // Start is called before the first frame update
    void Start()
    {
        callOnce = 0;
        stats = FindObjectOfType<PlayerStats>(); 
        data = stats.GetData();
        stats.awardBonusCrowns();
        stats.setWinner();
        if(data.winner==true){
            Winner.text = "Winner";
        }else{
            Winner.text = "loser";
        }
        Player.sprite = data.sprite;
        for(int i = 0; i < 3; i++){
            Gems[i].text = data.gemCount[i].ToString();
        }
        for(int i = 0; i < 4; i++){
            Crowns[i].SetActive(data.crowns[i]);
        }
        for(int i = 4; i < 7; i++ ){
            Crowns[i].SetActive(data.bonusCrowns[i-4]);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(callOnce == 0 && Time.time > WinnerTime ){
        //     // stats = FindObjectOfType<PlayerStats>(); 
        //     // data = stats.GetData();
            
        //     if(data.winner[0]==true){
        //         Winner[0].text = "Winner";
        //     }else{
        //         Winner[0].text = "loser";
        //     }
        //     callOnce = 1;
        // }
            
    }
}
