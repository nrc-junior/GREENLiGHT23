using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCamera : MonoBehaviour {
    Transform thisTransform;
    [SerializeField] Transform alvoParaSeguir;
    [SerializeField] float velocidade = 1;

    void Awake(){
        thisTransform = Camera.main.transform;
    }

    void Update() {
        thisTransform.SetParent(null, true);
        SuavizaMovimento();
    }

    void SuavizaMovimento(){
        Vector3 posAlvoAtual = alvoParaSeguir.position;

        if(Vector2.Distance(thisTransform.position, posAlvoAtual) > 0.001f ){
            posAlvoAtual = Vector2.Lerp(thisTransform.position, posAlvoAtual, velocidade * Time.fixedDeltaTime);
        }

        posAlvoAtual.z = thisTransform.position.z;
        thisTransform.position = posAlvoAtual;
        
    }
}
