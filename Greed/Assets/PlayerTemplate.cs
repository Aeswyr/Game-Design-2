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
    private float time = 0;
    
    
    private PlayerStats stats;
    private PlayerData data;
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<PlayerStats>(); 
        data = stats.GetData();
        Player.sprite = data.sprite;
        for(int i = 0; i < 3; i++){
            Gems[i].text = data.gemCount[i].ToString();
        }
        for(int i = 0; i < 4; i++){
            Crowns[i].SetActive(data.crowns[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        time+= Time.deltaTime;
        if(time>450){
            Debug.Log("200 seconds or frames?");
            time = 0;
        }
    }
}
