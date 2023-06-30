using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RamosDialogo : MonoBehaviour {
    [SerializeField] Button addNovaRoot;
    [SerializeField] Roteiro prefRoteiro;
    public bool temEspaco {get; set;}
    public string ator {get; set;}
    const int limite = 10;

    public Action<Roteiro> NOVO_DIALOGO;
    public List<Roteiro> roots = new List<Roteiro>();
    public RectTransform rect;
    
    void Awake(){
        TryGetComponent(out rect);
        // adiciona nova opção, que pode se tornar um ramo de dialogo.
        addNovaRoot.onClick.AddListener(AddRoteiro);
        prefRoteiro.gameObject.SetActive(false);
    }

    void AddRoteiro(){
        Roteiro roteiro = AddRoteiro(null);
        NOVO_DIALOGO?.Invoke(roteiro);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        // roteiro.btnAlterarAtor.interactable = false;
    }

    public Roteiro AddRoteiro(Roteiro roteiro = null, Transform parent = null, Roteiro.Data customData = null){
        Roteiro opcao = GameObject.Instantiate(prefRoteiro, parent == null ? transform : parent);        
        customData ??= roteiro?.data;
        Roteiro.Data data = customData;



        if(data == null){
            data = new Roteiro.Data();
            data._ator = ator;
            data.opcao = parent == null;
            data.falaDoProtagonista = true;
        } else if(roteiro){
            Destroy(roteiro.gameObject);
        }
        

        opcao.data.falaDoProtagonista = data.falaDoProtagonista;
        opcao.ator = data._ator;
        opcao.gameObject.SetActive(true);

        opcao.SetData(data); // * Carrega um data já salvo.
        addNovaRoot.transform.parent.SetAsLastSibling();
        
        temEspaco = transform.childCount < limite;
        addNovaRoot.gameObject.SetActive(temEspaco);
        opcao.linhaRamo.gameObject.SetActive(false);
        
        if(parent != null){
            opcao.linhaRamo.gameObject.SetActive(true);
            opcao.barraOpcoes.gameObject.SetActive(false);
            opcao.btnDispararEvento.interactable = false;
            opcao.btnOuvirEvento.interactable = false;

        }else{
            opcao.DELETADO += RemoverOpcao;
            opcao.NOVO_DIALOGO += AddDialogoFilho;
            roots.Add(opcao);
        }

        rect??= GetComponent<RectTransform>();
        if(rect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        return opcao;
    }

    /// <summary> adiciona dialogo filho da opção </summary>
    public void AddDialogoFilho(Roteiro parent){
        Roteiro roteiro = AddRoteiro(null, parent.transform);
        parent.childrens.Add(roteiro);
        roteiro.data.parent = parent;

        parent.Rebuild();
    }

    public void RemoverOpcao(Roteiro roteiro = null){
        if(roteiro){
            roots.Remove(roteiro);
            roteiro.DELETADO -= RemoverOpcao;
        }

        temEspaco = transform.childCount-1 < limite;
        addNovaRoot.gameObject.SetActive(temEspaco);
        if(transform.childCount-1 == 2) Destroy(gameObject);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

}
