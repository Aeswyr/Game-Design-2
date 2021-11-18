using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    private int[][] playersStats =  new int[4][];

    [SerializeField] private TextMeshProUGUI blueGemsP1, blueGemsP2, blueGemsP3, blueGemsP4;
    [SerializeField] private TextMeshProUGUI greenGemsP1, greenGemsP2, greenGemsP3, greenGemsP4;
    [SerializeField] private TextMeshProUGUI redGemsP1, redGemsP2, redGemsP3, redGemsP4;
    [SerializeField] private GameObject blueCrownP1, blueCrownP2, blueCrownP3, blueCrownP4;
    [SerializeField] private GameObject redCrownP1, redCrownP2, redCrownP3, redCrownP4;
    [SerializeField] private GameObject greenCrownP1, greenCrownP2, greenCrownP3, greenCrownP4;
    [SerializeField] private GameObject battleCrownP1, battleCrownP2, battleCrownP3, battleCrownP4;
    [SerializeField] private GameObject PlayerImageP1, PlayerImageP2, PlayerImageP3, PlayerImageP4;

    public void updateCount(int [][] input){
        blueGemsP1.text = input[0][0].ToString();
        greenGemsP1.text = input[0][1].ToString();
        redGemsP1.text = input[0][2].ToString();
        if(input[0][3] == 1){
            battleCrownP1.SetActive(true);
            // GameObject.Find("BlueCrown").GetComponent<Image>().enabled = true;
        }
        if(input[0][4] == 1){
            blueCrownP1.SetActive(true);
        }
        if(input[0][5] == 1){
            greenCrownP1.SetActive(true);
        }
        if(input[0][6] == 1){
            redCrownP1.SetActive(true);
        }

        blueGemsP2.text = input[1][0].ToString();
        greenGemsP2.text = input[1][1].ToString();
        redGemsP2.text = input[1][2].ToString();
        if(input[1][3] == 1){
            battleCrownP2.SetActive(true);
            // GameObject.Find("BlueCrown").GetComponent<Image>().enabled = true;
        }
        if(input[1][4] == 1){
            blueCrownP2.SetActive(true);
        }
        if(input[1][5] == 1){
            greenCrownP2.SetActive(true);
        }
        if(input[1][6] == 1){
            redCrownP2.SetActive(true);
        }
        
        blueGemsP3.text = input[2][0].ToString();
        greenGemsP3.text = input[2][1].ToString();
        redGemsP3.text = input[2][2].ToString();
        if(input[2][3] == 1){
            battleCrownP3.SetActive(true);
            // GameObject.Find("BlueCrown").GetComponent<Image>().enabled = true;
        }
        if(input[2][4] == 1){
            blueCrownP3.SetActive(true);
        }
        if(input[2][5] == 1){
            greenCrownP3.SetActive(true);
        }
        if(input[2][6] == 1){
            redCrownP3.SetActive(true);
        }

        blueGemsP4.text = input[3][0].ToString();
        greenGemsP4.text = input[3][1].ToString();
        redGemsP4.text = input[3][2].ToString();
        if(input[3][3] == 1){
            battleCrownP4.SetActive(true);
            // GameObject.Find("BlueCrown").GetComponent<Image>().enabled = true;
        }
        if(input[3][4] == 1){
            blueCrownP4.SetActive(true);
        }
        if(input[3][5] == 1){
            greenCrownP4.SetActive(true);
        }
        if(input[3][6] == 1){
            redCrownP4.SetActive(true);
        }
        
    }

    public void PushStats() {
        try{
            
            int [] P1 = new int[8];
            Array.Copy(FindObjectsOfType<PlayerController>()[0].getGems(), P1, 3) ;
            List<PickupType> Crowns = FindObjectsOfType<PlayerController>()[0].GetCrowns();
            if (Crowns.Contains(PickupType.CROWN_BATTLE))
                P1[3] = 1;
            if (Crowns.Contains(PickupType.CROWN_BLUE))
                P1[4] = 1;
            if (Crowns.Contains(PickupType.CROWN_GREEN))
                P1[5] = 1;
            if (Crowns.Contains(PickupType.CROWN_RED))
                P1[6] = 1;
            playersStats[0] = P1;

            int [] P2 = new int[8];
            if(FindObjectsOfType<PlayerController>().Length>1){
            Array.Copy(FindObjectsOfType<PlayerController>()[1].getGems(), P2, 3) ;
            Crowns = FindObjectsOfType<PlayerController>()[1].GetCrowns();
            if (Crowns.Contains(PickupType.CROWN_BATTLE))
                P2[3] = 1;
            if (Crowns.Contains(PickupType.CROWN_BLUE))
                P2[4] = 1;
            if (Crowns.Contains(PickupType.CROWN_GREEN))
                P2[5] = 1;
            if (Crowns.Contains(PickupType.CROWN_RED))
                P2[6] = 1;
            playersStats[1] = P2;
            }

            if(FindObjectsOfType<PlayerController>().Length>2){
            int [] P3 = new int[8];
            Array.Copy(FindObjectsOfType<PlayerController>()[2].getGems(), P3, 3) ;
            Crowns = FindObjectsOfType<PlayerController>()[2].GetCrowns();
            if (Crowns.Contains(PickupType.CROWN_BATTLE))
                P3[3] = 1;
            if (Crowns.Contains(PickupType.CROWN_BLUE))
                P3[4] = 1;
            if (Crowns.Contains(PickupType.CROWN_GREEN))
                P3[5] = 1;
            if (Crowns.Contains(PickupType.CROWN_RED))
                P3[6] = 1;
            playersStats[2] = P3;
            }

            if(FindObjectsOfType<PlayerController>().Length>3){
            int [] P4 = new int[8];
            Array.Copy(FindObjectsOfType<PlayerController>()[3].getGems(), P4, 3) ;
            Crowns = FindObjectsOfType<PlayerController>()[3].GetCrowns();
            if (Crowns.Contains(PickupType.CROWN_BATTLE))
                P4[3] = 1;
            if (Crowns.Contains(PickupType.CROWN_BLUE))
                P4[4] = 1;
            if (Crowns.Contains(PickupType.CROWN_GREEN))
                P4[5] = 1;
            if (Crowns.Contains(PickupType.CROWN_RED))
                P4[6] = 1;
            playersStats[3] = P4;
            }
            
        }catch( NullReferenceException e){
            Debug.Log(e.ToString());
        }
    }
    void Start(){
        DontDestroyOnLoad(this.gameObject);
        if(greenGemsP1!=null)
            updateCount(playersStats);
    }
    void FixedUpdate(){
        
        // playersStats = FindObjectsOfType<PlayerController>()[0].getGems();
        // Debug.Log(FindObjectsOfType<PlayerController>().Length);
    }

    public void UpdateGems(){
        // playersStats = FindObjectsOfType<PlayerController>()[0].getGems();
    }
}
