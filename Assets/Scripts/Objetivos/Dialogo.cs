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

    public bool loopable = true;
    public bool estaAtivo = true;
    public string dialogoNome;

    public string dialogo {get; set;}

    protected virtual void Update()  {
        if(!playerEstaNoTrigger) return;
        if(!estaAtivo) return;
        if(!Input.GetKeyDown(KeyCode.E)) return;
        
        PlayDialog();
    }
    
    protected virtual void PlayDialog(){
            ControladorDialogo.instancia.TocarDialogo(dialogo);
            if(!loopable) 
                estaAtivo = false;
    }
    
}

    // protected override void OnTriggerEnter2D(Collider2D col){
    //     base.OnTriggerEnter2D(col);
    // }

    // protected override void OnTriggerExit2D(Collider2D col){
    //     base.OnTriggerExit2D(col);
    // }
