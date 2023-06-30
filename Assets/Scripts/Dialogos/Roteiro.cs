using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] 
public class Roteiro : MonoBehaviour {
    
    /// <summary> informações que são salvas no arquivo </summary>
    [System.Serializable] public class Data{
        public bool falaDoProtagonista = true;
        public bool disparaEvento = false;
        public bool ouvirEvento = false;
        public bool opcao = false;
        public string textoDialogo = null;
        public string textoEscutarEvento = null;
        public string textoDispararEvento = null;
        public bool retornar = false;
        
        public Data[] childrens;

        // * não serializavel vvv * ///
        internal Roteiro parent = null;
        internal string _ator = null;
        internal bool mostrarRamos = false;
    }

    [HideInInspector] public Data data = new Data();

    public string ator {get => data._ator; set { 
            data._ator = value;
            nomeLabel.text = data.falaDoProtagonista ? "Prot." : ator.Substring(0,4)+'.';
        }
    }

    public bool isBranched {get; set; } = false;
    
    public Action<Roteiro> DELETADO;
    public Action<Roteiro> NOVO_DIALOGO;
    public Action<Roteiro, bool> SETOU_OPCAO;


    [Header("Internal References")]
    [SerializeField] protected Button btnOpcao;
    public Button btnAlterarAtor;
    public Button btnDispararEvento;
    public Button btnOuvirEvento;
    
    [SerializeField] protected Button btnRetornar;
    [SerializeField] protected Button btnExtender;
    [SerializeField] protected Button btnAddNovoDialogo;
    public RectTransform barraOpcoes;
    public RectTransform linhaRamo;


    [SerializeField] protected Button btnDeleta;
    [SerializeField] protected Button btnArrastarCima;
    [SerializeField] protected Button btnArrastarBaixo;


    [SerializeField] protected GameObject campoDispararEvento;
    [SerializeField] protected GameObject campoOuvirEvento;

    [SerializeField] protected Color corAtivo;
    [SerializeField] protected Color corDesativo;

    [SerializeField] protected Color corProtagnista;
    [SerializeField] protected Color corOutroAtor;
    [SerializeField] protected Color corInputDialogo;
    [SerializeField] protected Color corInputEscolha;

    [SerializeField] protected Image imgDispararEvento;
    [SerializeField] protected Image imgOuvirEvento;
    [SerializeField] protected Image imgOpcao;
    
    [SerializeField] protected Text nomeLabel;
    [SerializeField] protected Image imgNome;
    [SerializeField] protected Image imgInput;

    [SerializeField] protected Text placeholderEscutarEvento;
    [SerializeField] protected Text placeholderDispararEvento;

    [SerializeField] protected InputField inputDialogo;
    [SerializeField] protected InputField inputEscutarEvento;
    [SerializeField] protected InputField inputDispararEvento;
    internal List<Roteiro> childrens = new List<Roteiro>();

    const string _ouvirEventoOpcao = "Nome do evento que precisa acontecer pra aparecer essa opcão...";
    const string _dispararEventoOpcao = "Nome do evento disparado após escolher essa opção...";
    const string _dispararEventoDialogo = "Nome do evento disparado após esse dialogo...";
    const string _ouvirEventoDialogo = "Nome do evento que precisa acontecer pra aparecer esse texto...";

    private RectTransform rect; 
    void Awake(){
        rect = GetComponent<RectTransform>();

        btnAlterarAtor.onClick.AddListener(TrocarAtor);
        btnDispararEvento.onClick.AddListener(SetarCampoDispararEvento);
        btnOuvirEvento.onClick.AddListener(SetarCampoOuvirEvento);
        btnOpcao.onClick.AddListener(SetarOpcao);
        
        btnDeleta.onClick.AddListener(DeletarDialogo);
        btnArrastarCima?.onClick.AddListener(MoverCima);
        btnArrastarBaixo?.onClick.AddListener(MoverBaixo);

        inputDialogo.onEndEdit.AddListener(SalvarInputDialogo);
        inputEscutarEvento.onEndEdit.AddListener(SalvarInputEscutarEvento);
        inputDispararEvento.onEndEdit.AddListener(SalvarInputDispararEvento);
        inputEscutarEvento.gameObject.SetActive(data.ouvirEvento);
        inputDispararEvento.gameObject.SetActive(data.disparaEvento);
        
        if(btnExtender) btnExtender.interactable = false;

        if(placeholderEscutarEvento != null)
        placeholderEscutarEvento.text = _ouvirEventoDialogo;
        
        if(placeholderDispararEvento != null)
        placeholderDispararEvento.text = _dispararEventoDialogo;


        if(barraOpcoes != null){
            btnAddNovoDialogo.onClick.AddListener(AdicionarNovoDialogo);
            btnExtender.transform.parent.GetComponent<RectTransform>();
            btnExtender.onClick.AddListener(ExibirRamo);
            btnRetornar.onClick.AddListener(SetarRetorno);
            inputEscutarEvento.onEndEdit.AddListener(Rebuild);
            inputDispararEvento.onEndEdit.AddListener(Rebuild);
        }

    }

    /// <summary> adiciona um dialogo apartir desse roteiro </summary>
    public void AdicionarNovoDialogo(){
        NOVO_DIALOGO?.Invoke(this);
    }

    void SetarRetorno(){
        data.retornar = !data.retornar;
        btnRetornar.targetGraphic.color = data.retornar ? new Color(0.96f,0.74f,0.25f, 255) : new Color(0.25f,0.25f,0.25f, 255) ;
    }

    void ExibirRamo(){
        data.mostrarRamos = !data.mostrarRamos;
        btnExtender.targetGraphic.color = data.mostrarRamos ? new Color(0.96f,0.74f,0.25f, 255) : new Color(0.25f,0.25f,0.25f, 255) ;
    }



    public void DeletarDialogo(){
        DELETADO?.Invoke(this);
        Destroy(gameObject);
    }

    public void TrocarAtor(){
        data.falaDoProtagonista = !data.falaDoProtagonista;

        if(data.opcao && !data.falaDoProtagonista){ 
            SetarOpcao();
        }
        
        imgOpcao.color = data.falaDoProtagonista ? corDesativo: new Color(.4f,0,0,255);
        btnOpcao.interactable = data.falaDoProtagonista;

        nomeLabel.text = data.falaDoProtagonista ? "Prot." : ator.Substring(0,4)+'.';
        imgNome.color = data.falaDoProtagonista ? corProtagnista : corOutroAtor;
    }

    public void SetarCampoDispararEvento(){
        data.disparaEvento = !data.disparaEvento;
        imgDispararEvento.color = data.disparaEvento ? corAtivo : corDesativo;
        campoDispararEvento.SetActive(data.disparaEvento);
        AtualizarTxtEventosPlaceholders();
    }

    public void SetarCampoOuvirEvento(){
        data.ouvirEvento = !data.ouvirEvento;
        imgOuvirEvento.color = data.ouvirEvento ? corAtivo : corDesativo;
        campoOuvirEvento.SetActive(data.ouvirEvento);
        AtualizarTxtEventosPlaceholders();
    }

    public void SetarOpcao(){
        data.opcao = !data.opcao;
        SETOU_OPCAO?.Invoke(this, data.opcao);
        return;
        
        if(!data.falaDoProtagonista) data.opcao = false;

        imgOpcao.color = data.opcao ? corAtivo : corDesativo;
        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;
        AtualizarTxtEventosPlaceholders();

    }

    void AtualizarTxtEventosPlaceholders(){
        
        if(placeholderEscutarEvento != null)
        placeholderEscutarEvento.text = data.opcao ? _ouvirEventoOpcao : _ouvirEventoDialogo;
        
        if(placeholderDispararEvento != null)
        placeholderDispararEvento.text = data.opcao ? _dispararEventoOpcao : _dispararEventoDialogo;

        if(barraOpcoes != null){
            LayoutRebuilder.ForceRebuildLayoutImmediate(barraOpcoes);
        }
    }

    void SalvarInputDialogo(string texto){
        data.textoDialogo = texto;
    }
    void SalvarInputEscutarEvento(string texto){
        data.textoEscutarEvento = texto;
    }
    void SalvarInputDispararEvento(string texto){
        data.textoDispararEvento = texto;
    }

    public void MoverCima(){
        int indiceHierarquia = transform.GetSiblingIndex() - 1;
        if( indiceHierarquia < 1 ) return;
        
        transform.SetSiblingIndex(indiceHierarquia);
    }

    public void MoverBaixo(){
        int indiceHierarquia = transform.GetSiblingIndex() + 1;
        if(indiceHierarquia > transform.parent.childCount-2) return;

        transform.SetSiblingIndex(indiceHierarquia);
    }

    public void SetData(Data data){
        this.data = data;

        imgDispararEvento.color = data.disparaEvento ? corAtivo : corDesativo;
        imgOuvirEvento.color = data.ouvirEvento ? corAtivo : corDesativo;
        
        campoDispararEvento.SetActive(data.disparaEvento);
        campoOuvirEvento.SetActive(data.ouvirEvento);
        
        imgOpcao.color = data.opcao ? corAtivo : corDesativo;
        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;
        
        imgOpcao.color = data.falaDoProtagonista ? imgOpcao.color: new Color(.4f,0,0,255);
        imgNome.color = data.falaDoProtagonista ? corProtagnista : corOutroAtor;
        btnOpcao.interactable = data.falaDoProtagonista;

        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;
        
        if(btnRetornar) btnRetornar.targetGraphic.color = data.retornar ? new Color(0.96f,0.74f,0.25f, 255) : new Color(0.25f,0.25f,0.25f, 255) ;
        if(btnExtender) btnExtender.targetGraphic.color = data.mostrarRamos ? new Color(0.96f,0.74f,0.25f, 255) : new Color(0.25f,0.25f,0.25f, 255) ;

        AtualizarTxtEventosPlaceholders();

        inputDialogo.text = data.textoDialogo;
        inputEscutarEvento.text = data.textoEscutarEvento;
        inputDispararEvento.text = data.textoDispararEvento;
        SETOU_OPCAO?.Invoke(this, data.opcao);
    }

    public void Rebuild(string _ = null){
        rect ??= GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        if(barraOpcoes){
            LayoutRebuilder.ForceRebuildLayoutImmediate(barraOpcoes);
        }
    }
}
