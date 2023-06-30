using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportarInterior : Objetivo {
    public bool habilitado = true;
    [SerializeField] bool ligarInterior;
    [SerializeField] Transform pontoTeleporte;


    void Awake(){
        if(habilitado && pontoTeleporte == null){
            Debug.LogError("<<Sem ponto de Teleporte definido.>>", gameObject);
        }
    }

    void Update(){
        if(playerEstaNoTrigger){
            if(Input.GetKeyDown(KeyCode.E)){
                TentarEntrarInterior();
            }
        }
    }

    /// <summary> faz o fade in da tela </summary>
    protected virtual void TentarEntrarInterior(){
        if(habilitado && pontoTeleporte){
            TeleportarPlayer(pontoTeleporte.position);
        }
    }

    protected virtual void TeleportarPlayer(Vector3 posicao){
        // todo fade in antes de chamar 
        Cenario.instance.AtivarCenario(ligarInterior);
        Cenario.TELEPORTAR_PLAYER?.Invoke(posicao);
    } 

    protected override void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
    }

    protected override void OnTriggerExit2D(Collider2D col){
        base.OnTriggerExit2D(col);
    }
}
