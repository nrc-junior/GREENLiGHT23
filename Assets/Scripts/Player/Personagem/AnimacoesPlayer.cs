using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovimentoIsometrico))]
public class AnimacoesPlayer : MonoBehaviour {
    
    MovimentoIsometrico movimento;
    [SerializeField] Animator animator;
    Vector2 ultimoInput;

    void Awake(){
        movimento ??= GetComponentInChildren<MovimentoIsometrico>();
        animator ??= GetComponentInChildren<Animator>();
    }

    void Update() {
        Vector2 direcao = movimento.RawInput;
        direcao = direcao == Vector2.zero ? ultimoInput : direcao;
        ultimoInput = direcao; // guarda referencia da ultima direção que andou pro personagem sempre ficar olhando pra la.
        
        animator.SetFloat("x", direcao.x);
        animator.SetFloat("y", direcao.y);
    }
}
