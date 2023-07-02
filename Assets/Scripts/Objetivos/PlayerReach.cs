using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerReach : Objetivo {
    public string dispatchOnReach;
    public bool disable = true;
    public bool destroy = false;

    public UnityEvent OnReach = new UnityEvent();

    protected override void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        if(playerEstaNoTrigger){
            Orquestrador.instance.ReceiveIntermediateEvent(dispatchOnReach);
            
            OnReach?.Invoke();
        
            if(disable){
                gameObject.SetActive(false);
                if(destroy){
                    Destroy(gameObject);
                }
            }
        }
        
    }
}
