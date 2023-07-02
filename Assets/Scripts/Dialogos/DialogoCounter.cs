using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogoCounter : MonoBehaviour {
    [SerializeField] private Dialogo[] listeners;
    [SerializeField] private string dispatchIntermediate;
    
    int counter;

    void Awake(){
        foreach (Dialogo dialogo in listeners){
            dialogo.QUIT += SumCounter;
        
            void SumCounter(){
                dialogo.QUIT -= SumCounter;
                counter++;
                if(counter >= listeners.Length){ // conversou com todo mundo
                    Orquestrador.instance.ReceiveIntermediateEvent(dispatchIntermediate);
                }
            }
        }
    }


}
