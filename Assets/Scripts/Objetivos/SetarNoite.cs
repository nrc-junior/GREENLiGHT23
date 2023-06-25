using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SetarNoite : Objetivo {
    
    bool playerEstaNoTrigger = false;
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

    void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("Player")){
            playerEstaNoTrigger = true;
            texto.color = Color.green;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        playerEstaNoTrigger = false;
        texto.color = Color.red;
    }
}
