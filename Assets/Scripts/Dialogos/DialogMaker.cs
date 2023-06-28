using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class DialogMaker : Dialogo {
    

    [Space(10)]
    [Header("Referencias")]
    [SerializeField] MovimentoIsometrico controle;

    [Header("Controle Dialogos")]
    [SerializeField] GameObject uiGerenciadorDialogos;
    [SerializeField] Button btnCarregarDialogo;
    [SerializeField] Button btnSalvarDialogo;
    [SerializeField] GameObject uiCriarDialogos;
    [SerializeField] RectTransform containerPersonagens;

    [SerializeField] GameObject confirmacaoSobscrever;
    [SerializeField] Button btnConfirmar;
    [SerializeField] Button btnCancelar;
    Action<bool> SOBSCREVER;

    [Space(10)]
    [Header("Criação Dialogos")]
    [SerializeField] Button btnAddDialogo;
    [SerializeField] Button btnVoltar;
    [SerializeField] DialogoData prefabDialogo;
    RectTransform containerDialogos;

    bool alterouENaoSalvou = false;

    Dictionary<string, DialogoSerializado> conteudoDialogos = new Dictionary<string, DialogoSerializado>();
    DialogoSerializado curSelecionado;
    
    /// <summary> classe para transformar o que foi digitado para json. </summary>
    [System.Serializable]
    public class DialogoSerializado{
        public string ator = null;
        public DialogoData.Data[] dialogos;
        
        public DialogoSerializado(string nome, DialogoData.Data[] dialogos = null){
            ator = nome;

            if(dialogos != null){
                Debug.Log(dialogos.Length);
                this.dialogos = dialogos;
            }
        }
    }


    List <DialogoData> curDialogos = new List<DialogoData>();

    void Awake(){
        SetupSelecaoDePersonagem();
        SetupCriacaoDeDialogo();
        SetupSalvar();
    }


    void SetupSelecaoDePersonagem(){
        btnCarregarDialogo.onClick.AddListener(CarregarDialogo);
        btnSalvarDialogo.onClick.AddListener(SalvarDialogo);

        //o script escuta quando escolher um personagem, e faz configurações.
        foreach (Toggle btnPersonagem in containerPersonagens.GetComponentsInChildren<Toggle>()){
            string nome = btnPersonagem.GetComponentInChildren<Text>().text;
            btnPersonagem.onValueChanged.AddListener((bool isOn) => {
                if(isOn){
                    SelecionouPersonagem(nome);
                }
            });

            conteudoDialogos.Add(nome, new DialogoSerializado(nome));    
            if(btnPersonagem.isOn) curSelecionado = conteudoDialogos[nome];
        }

        uiGerenciadorDialogos.SetActive(false);
    }
    
    void SetupCriacaoDeDialogo(){
        containerDialogos = prefabDialogo.transform.GetComponentInParent<RectTransform>();
        btnAddDialogo.onClick.AddListener(CriarDialogo);
        btnVoltar.onClick.AddListener(Voltar);
        uiCriarDialogos.SetActive(false);
        prefabDialogo.gameObject.SetActive(false);
    }

    void SetupSalvar(){
        btnConfirmar.onClick.AddListener(() => {SOBSCREVER?.Invoke(true);});
        btnCancelar.onClick.AddListener(() => {SOBSCREVER?.Invoke(false);});
        confirmacaoSobscrever.SetActive(false);
    }

    void Voltar(){
        uiCriarDialogos.SetActive(false);
        controle.travado = false;
    }

    protected override void PlayDialog(){
        uiCriarDialogos.SetActive(true);
        controle.travado = true;

        foreach (DialogoData _dialogo in curDialogos) {
            _dialogo.ator = curSelecionado.ator;
        }
    }

    void QuandoDeletarDialogo(DialogoData data){
        data.DELETADO -= QuandoDeletarDialogo;
        curDialogos.Remove(data);
        alterouENaoSalvou = true;
    }
    void CriarDialogo(){
        CriarDialogo(null);
    }

    void CriarDialogo(DialogoData.Data curData = null){
        DialogoData newDialog = GameObject.Instantiate(prefabDialogo, prefabDialogo.transform.parent);
        newDialog.data.falaDoProtagonista = true;
        newDialog.ator = curSelecionado.ator;
        newDialog.DELETADO += QuandoDeletarDialogo;

        newDialog.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerDialogos);
        
        btnAddDialogo.transform.parent.SetAsLastSibling();
        curDialogos.Add(newDialog);
        
        if(curData != null){
            newDialog.SetData(curData); // * Carrega um data já salvo.
        }else{
            alterouENaoSalvou = true;
        }
    }

    void CarregarDialogo(){


        var extensions = new [] {
            new ExtensionFilter("Dialogos", "dialog" ),
            new ExtensionFilter("All Files", "*" ),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if(paths.Length == 0) return;

        if(alterouENaoSalvou){
            confirmacaoSobscrever.SetActive(true);
            SOBSCREVER += QuandoEscolherOpcao;
            return;
        }

        CarregarDialogoEscolhido(paths[0]);
        
        // metodo local
        void QuandoEscolherOpcao (bool sobscrever ){
            SOBSCREVER -= QuandoEscolherOpcao;
            confirmacaoSobscrever.SetActive(false);
            if(!sobscrever) return;
            CarregarDialogoEscolhido(paths[0]);
        }
    }

    void CarregarDialogoEscolhido(string path){
        DialogoSerializado desserializado = JsonUtility.FromJson<DialogoSerializado>(File.ReadAllText(path));
        SetDialogosAtual(desserializado);
    }

    void SalvarDialogo(){
        var path = StandaloneFileBrowser.SaveFilePanel("Salvar Roteiro", "", "Roteiro " + curSelecionado.ator, "dialog");
        if(string.IsNullOrEmpty(path)) return;
        
        List<DialogoData.Data> datas = new List<DialogoData.Data>();
        
        foreach(var _dialog in curDialogos ){
            datas.Add(_dialog.data);
        }

        curSelecionado.dialogos = datas.ToArray();

        string json = JsonUtility.ToJson(curSelecionado, true);
        File.WriteAllText(path, json);
        alterouENaoSalvou = false;
    }


    void SelecionouPersonagem(string nome){
        curSelecionado = conteudoDialogos[nome];
    } 

    protected override void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        if(playerEstaNoTrigger)
        
        uiGerenciadorDialogos.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerPersonagens);
    } 
    
    protected override void OnTriggerExit2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        uiGerenciadorDialogos.SetActive(false);
    } 


    void SetDialogosAtual(DialogoSerializado desserializado){
        foreach (Toggle btnPersonagem in containerPersonagens.GetComponentsInChildren<Toggle>()){
            string nome = btnPersonagem.GetComponentInChildren<Text>().text;
            bool ativo = nome == desserializado.ator; 
            btnPersonagem.isOn = ativo;
            if(ativo) break;
        }

        for (var i = 0; i < curDialogos.Count; i++){
            curDialogos[i].DELETADO -= QuandoDeletarDialogo;
            DestroyImmediate(curDialogos[i].gameObject);
        }

        curDialogos.Clear();
        curSelecionado = desserializado;

        foreach (DialogoData.Data d in desserializado.dialogos){
            CriarDialogo(d);
        }
        
    }
}
