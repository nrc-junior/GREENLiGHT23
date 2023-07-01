using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoIsometrico : MonoBehaviour {
    private Rigidbody2D rb;
    Transform cam;

    [SerializeField] float velocidade = 1;
    Vector2 direcao;

    public Vector2 RawInput {get; private set;}
    public bool _travado;
    public bool travado {get => _travado; set {
            RawInput = Vector2.zero;
            direcao = Vector2.zero;
            _travado = value;
        }
    }

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main.transform;
        Cenario.TELEPORTAR_PLAYER += Teleportar;
    }

    void OnDisable(){
        Cenario.TELEPORTAR_PLAYER -= Teleportar;
    }

    void Update(){
        if(travado) return;

        RawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direcao = RawInput;
        ArrumarForcaDoInput();
    }

    /// <summary> divide por 2 se estiver apertando os dois botões ao mesmo tempo, pra n aumentar a velocidade  </summary>
    void ArrumarForcaDoInput(){
        if(direcao.magnitude > 1){
            direcao *= 2f;
            direcao.y /= 4f;
            direcao.x /= 2;
        }
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + ((direcao * velocidade) * Time.fixedDeltaTime));
    }

    public void Teleportar(Vector3 posicao){
        transform.position = posicao;
        posicao.z = cam.position.z;
        cam.position = posicao;
    }
}
