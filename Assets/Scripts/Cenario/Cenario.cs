using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cenario : MonoBehaviour {
    public static Cenario instance;

    [SerializeField] GameObject exterior;
    [SerializeField] GameObject interiores;
    [SerializeField] LightColor sun;
    public bool IsDay {get; set;} = true;

    /// <remarks> PS: Desinscrever do evento antes de sair da cena. </remarks>
    public static Action<Vector3> TELEPORTAR_PLAYER;
    void Awake(){
        instance = this;
    }
    
    public void AtivarCenario(bool ligarInterior){
        if(ligarInterior){
            interiores.SetActive(true);
            exterior.SetActive(false);
            sun.SetColor(true);
        }else{
            interiores.SetActive(false);
            exterior.SetActive(true);
            sun.SetColor(IsDay);
        }
    }
}
