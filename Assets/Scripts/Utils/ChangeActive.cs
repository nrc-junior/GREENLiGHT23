using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChangeActive : MonoBehaviour {
    
    [Flags]
    public enum STATE{
        Awake = 1,
        OnEnable = 2,
        Start = 4,
    } 

    public STATE executeOrder;
    
    public bool setActive = true;
    public GameObject target;
    public GameObject[] targets;


    public void Awake(){
        target ??= gameObject;
        
        if(executeOrder == STATE.Awake){
            target.SetActive(setActive);
            if(targets.Length > 0){
                foreach (GameObject t in targets) t.SetActive(setActive);
            }
        }
    }
    
    public void OnEnable(){
        if(executeOrder == STATE.OnEnable){
            target.SetActive(setActive);
            if(targets.Length > 0){
                foreach (GameObject t in targets) t.SetActive(setActive);
            }
        }
    }

    public void Start(){
        if(executeOrder == STATE.Start){
            target.SetActive(setActive);
            if(targets.Length > 0){
                foreach (GameObject t in targets) t.SetActive(setActive);
            }
        }
    }

}
