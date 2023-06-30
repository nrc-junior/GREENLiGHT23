using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objetivo : MonoBehaviour {
    protected bool playerEstaNoTrigger = false;
    
    protected virtual void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("Player")){
            playerEstaNoTrigger = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D col){
        if(col.CompareTag("Player")){
            playerEstaNoTrigger = false;
        }
    }
}
