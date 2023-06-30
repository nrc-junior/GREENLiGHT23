using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cenario : MonoBehaviour {
    public static Cenario instance;

    [SerializeField] GameObject exterior;
    [SerializeField] GameObject interiores;

    
    /// <remarks> PS: Desinscrever do evento antes de sair da cena. </remarks>
    public static Action<Vector3> TELEPORTAR_PLAYER;
    
    public void AtivarCenario(bool ligarInterior){
        if(ligarInterior){
            interiores.SetActive(true);
            exterior.SetActive(false);
        }else{
            interiores.SetActive(false);
            exterior.SetActive(true);
        }
    }
}
