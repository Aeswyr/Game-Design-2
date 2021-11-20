using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    [SerializeField] private GameObject player_template; 
    private int playerCount;
    // Start is called before the first frame update
    void Start()
    {
     playerCount = FindObjectOfType<PlayerStats>().getPlayerCount();
     for( int i = 0; i < playerCount; i++){
         Instantiate(player_template, transform);
     }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
