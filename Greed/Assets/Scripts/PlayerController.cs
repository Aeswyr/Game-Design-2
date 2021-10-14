using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rbody.velocity = new Vector3(Input.GetAxis("Horizontal") * 5, rbody.velocity.y, 0);
    }
}
