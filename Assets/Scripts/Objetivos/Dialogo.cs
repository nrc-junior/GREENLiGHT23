using UnityEngine;

public class Dialogo : Objetivo {
    public bool estaAtivo = true;
    public string dialogoNome;

    void Start()    {
        
    }


    protected virtual void Update()  {
        if(!playerEstaNoTrigger) return;
        if(!estaAtivo) return;
        if(!Input.GetKeyDown(KeyCode.E)) return;
        
        PlayDialog();
    }
    
    protected virtual void PlayDialog(){
        ControladorDialogo.instancia.CarregarDialogo(dialogoNome);
    }

    // protected override void OnTriggerEnter2D(Collider2D col){
    //     base.OnTriggerEnter2D(col);
    // }

    // protected override void OnTriggerExit2D(Collider2D col){
    //     base.OnTriggerExit2D(col);
    // }
}
