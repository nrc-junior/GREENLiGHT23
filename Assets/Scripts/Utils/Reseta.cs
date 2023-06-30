using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reseta : Objetivo
{
    
    public ControladorDialogo dig;

    void Update(){
        if(playerEstaNoTrigger && Input.GetKeyDown(KeyCode.E)){
            dig.LimparEventos();
        }        
    }
}
