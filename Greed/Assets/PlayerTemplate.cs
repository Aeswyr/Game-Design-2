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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
