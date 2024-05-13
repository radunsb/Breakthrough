using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinScript : MonoBehaviour{
    void Start(){
        
    }

    void Update(){
        while (true){
            transform.rotation = Quaternion.Euler(Vector3.forward * 10);
        }
       
    }
}
