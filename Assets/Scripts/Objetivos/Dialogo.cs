using System;
using UnityEngine;

public class Dialogo : Objetivo {
    /// <summary> classe para transformar o que foi digitado para json. </summary>
    [System.Serializable]
    public class Data {
        public string ator = null;
        public Roteiro.Data[] dialogos;
        
        public Data(){

        }

        public Data(string nome, Roteiro.Data[] dialogos = null){
            ator = nome;

            if(dialogos != null){
                Debug.Log(dialogos.Length);
                this.dialogos = dialogos;
            }
        }
    }
    
    public GameObject onPlayHide;

    public bool loopable = true;
    public bool estaAtivo = true;
    public string dialogoNome;

    string _rot;
    public string roteiro {get => _rot; set
        {
            _rot = value;
            
            if(!string.IsNullOrEmpty(_rot) && onPlayHide){
                onPlayHide.SetActive(true);
        }
        }
    }
    public Action QUIT;

    protected virtual void Update()  {
        if(!playerEstaNoTrigger) return;
        if(!estaAtivo) return;
        if(!Input.GetKeyDown(KeyCode.E)) return;
        
        PlayDialog();
    }
    
    public virtual void PlayDialog(){
        if(!string.IsNullOrEmpty(roteiro))

        ControladorDialogo.instancia.TocarDialogo(this);
        if(!loopable) 
            roteiro = null;
    
        if(onPlayHide){
            onPlayHide.SetActive(false);
        }
    }
}

    // protected override void OnTriggerEnter2D(Collider2D col){
    //     base.OnTriggerEnter2D(col);
    // }

    // protected override void OnTriggerExit2D(Collider2D col){
    //     base.OnTriggerExit2D(col);
    // }
