using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] public class DialogoData : MonoBehaviour {
    
    /// <summary> informações que são salvas no arquivo </summary>
    [System.Serializable] public class Data{
        internal string _ator = null;
        public bool falaDoProtagonista = true;
        public bool disparaEvento = false;
        public bool possuiEventoFlag = false;
        public bool opcao = false;
        public string textoDialogo = null;
        public string textoEscutarEvento = null;
        public string textoDispararEvento = null;
    }

    [HideInInspector] public Data data = new Data();

    public string ator {get => data._ator; set { 
            data._ator = value;
            nomeLabel.text = data.falaDoProtagonista ? "Prot." : ator.Substring(0,4)+'.';
        }
    }
    
    public Action<DialogoData> DELETADO;

    [Header("Internal References")]
    [SerializeField] protected Button btnAlterarAtor;
    [SerializeField] protected Button btnDispararEvento;
    [SerializeField] protected Button btnOuvirEvento;
    [SerializeField] protected Button btnOpcao;

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

    const string _ouvirEventoOpcao = "Nome do evento que precisa acontecer pra aparecer essa opcão...";
    const string _dispararEventoOpcao = "Nome do evento disparado após escolher essa opção...";
    const string _dispararEventoDialogo = "Nome do evento disparado após esse dialogo...";
    const string _ouvirEventoDialogo = "Nome do evento que precisa acontecer pra aparecer esse texto...";

    void Awake(){
        btnAlterarAtor.onClick.AddListener(TrocarAtor);
        btnDispararEvento.onClick.AddListener(SetarCampoDispararEvento);
        btnOuvirEvento.onClick.AddListener(SetarCampoOuvirEvento);
        btnOpcao.onClick.AddListener(SetarCampoOpcao);
        
        btnDeleta.onClick.AddListener(DeletarDialogo);
        btnArrastarCima.onClick.AddListener(MoverCima);
        btnArrastarBaixo.onClick.AddListener(MoverBaixo);

        inputDialogo.onEndEdit.AddListener(SalvarInputDialogo);
        inputEscutarEvento.onEndEdit.AddListener(SalvarInputEscutarEvento);
        inputDispararEvento.onEndEdit.AddListener(SalvarInputDispararEvento);
        inputEscutarEvento.gameObject.SetActive(false);
        inputDispararEvento.gameObject.SetActive(false);

        placeholderEscutarEvento.text = _ouvirEventoDialogo;
        placeholderDispararEvento.text = _dispararEventoDialogo;
    }

    public void DeletarDialogo(){
        DELETADO?.Invoke(this);
        Destroy(gameObject);
    }

    public void TrocarAtor(){
        data.falaDoProtagonista = !data.falaDoProtagonista;

        if(data.opcao && !data.falaDoProtagonista){ 
            SetarCampoOpcao();
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
        data.possuiEventoFlag = !data.possuiEventoFlag;
        imgOuvirEvento.color = data.possuiEventoFlag ? corAtivo : corDesativo;
        campoOuvirEvento.SetActive(data.possuiEventoFlag);
        AtualizarTxtEventosPlaceholders();
    }

    public void SetarCampoOpcao(){
        data.opcao = !data.opcao;
        if(!data.falaDoProtagonista) data.opcao = false;

        imgOpcao.color = data.opcao ? corAtivo : corDesativo;
        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;
        AtualizarTxtEventosPlaceholders();
    }

    void AtualizarTxtEventosPlaceholders(){
        placeholderEscutarEvento.text = data.opcao ? _ouvirEventoOpcao : _ouvirEventoDialogo;
        placeholderDispararEvento.text = data.opcao ? _dispararEventoOpcao : _dispararEventoDialogo;
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

        campoDispararEvento.SetActive(data.disparaEvento);
        campoOuvirEvento.SetActive(data.possuiEventoFlag);
        
        imgOpcao.color = data.opcao ? corAtivo : corDesativo;
        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;
        
        imgOpcao.color = data.falaDoProtagonista ? imgOpcao.color: new Color(.4f,0,0,255);
        imgNome.color = data.falaDoProtagonista ? corProtagnista : corOutroAtor;
        btnOpcao.interactable = data.falaDoProtagonista;

        imgInput.color = data.opcao ? corInputEscolha : corInputDialogo;

        AtualizarTxtEventosPlaceholders();

        inputDialogo.text = data.textoDialogo;
        inputEscutarEvento.text = data.textoEscutarEvento;
        inputDispararEvento.text = data.textoDispararEvento;
    }

}
