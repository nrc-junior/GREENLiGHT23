using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SetarNoite : Objetivo {
    
    bool eNoite = false;
    bool animando = false;

    Text texto;

    [SerializeField] Light2D luzDoMundo;
    [SerializeField] Color luzDia = Color.white;
    [SerializeField] Color luzNoite = Color.black;
    
    void Awake(){
        texto = GetComponentInChildren<Text>();
        texto.color = Color.red;
    }

    void Update(){

        if(playerEstaNoTrigger){
            if(Input.GetKeyDown(KeyCode.E) && !animando){
                
                if(!eNoite){
                    
                    // * LeanTween é um plugin pra animação, que funciona +/- igual uma Coroutine (iEnumerator). 
                    LeanTween.value(gameObject, 0, 1, 3).setOnUpdate((float v) => {
                        luzDoMundo.color = Color.Lerp(luzDia, luzNoite, v);
                    }).setOnComplete(() => {
                        texto.text = "E - Setar dia";
                        animando = false;
                    });
                    
                    animando = true;
                    eNoite = true;
                }else{
                    
                    LeanTween.value(gameObject, 0, 1, 3).setOnUpdate((float v) => {
                        luzDoMundo.color = Color.Lerp(luzNoite, luzDia, v);
                    }).setOnComplete(() => {
                        texto.text = "E - Setar Noite";
                        animando = false;
                    });
                    
                    animando = true;
                    eNoite = false;
                }
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        if(!playerEstaNoTrigger) return;
        
        texto.color = Color.green;
    }

    protected override void OnTriggerExit2D(Collider2D col){
        base.OnTriggerExit2D(col);
        if(playerEstaNoTrigger) return;
        
        texto.color = Color.red;
    }
}
