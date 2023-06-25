using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoIsometrico : MonoBehaviour {
    private Rigidbody2D rb;

    [SerializeField] float velocidade = 1;
    Vector2 direcao;

    public Vector2 RawInput {get; private set;}

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        RawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direcao = RawInput;
        ArrumarForcaDoInput();
    }

    /// <summary> divide por 2 se estiver apertando os dois botões ao mesmo tempo, pra n aumentar a velocidade  </summary>
    void ArrumarForcaDoInput(){
        if(direcao.magnitude > 1){
            direcao *= 0.707107f;
        }
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + ((direcao * velocidade) * Time.fixedDeltaTime));
    }
}
