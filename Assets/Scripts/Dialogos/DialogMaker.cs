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
    [SerializeField] Roteiro prefabDialogo;
    RectTransform containerDialogos;

    [SerializeField] Dialogo testDialog;
    [SerializeField] RamosDialogo horizontalOption;


    bool alterouENaoSalvou = false;

    Dictionary<string, Dialogo.Data> conteudoDialogos = new Dictionary<string, Dialogo.Data>();
    Dialogo.Data roteirosAtual;
    

    List <Roteiro> curDialogos = new List<Roteiro>();

    protected override void Update(){
        base.Update();

        if(uiCriarDialogos.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)){
            Voltar();
        }
    }

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

            conteudoDialogos.Add(nome, new Dialogo.Data(nome));    
            if(btnPersonagem.isOn) roteirosAtual = conteudoDialogos[nome];
        }

        uiGerenciadorDialogos.SetActive(false);
    }
    
    void SetupCriacaoDeDialogo(){
        containerDialogos = prefabDialogo.transform.GetComponentInParent<RectTransform>();
        btnAddDialogo.onClick.AddListener(CriarDialogo);
        btnVoltar.onClick.AddListener(Voltar);
        uiCriarDialogos.SetActive(false);
        prefabDialogo.gameObject.SetActive(false);
        horizontalOption.gameObject.SetActive(false);
    }

    void SetupSalvar(){
        btnConfirmar.onClick.AddListener(() => {SOBSCREVER?.Invoke(true);});
        btnCancelar.onClick.AddListener(() => {SOBSCREVER?.Invoke(false);});
        confirmacaoSobscrever.SetActive(false);
    }

    void Voltar(){
        uiCriarDialogos.SetActive(false);
        controle.travado = false;
        testDialog.roteiro = Serializar();
    }

    public override void PlayDialog(){
        uiCriarDialogos.SetActive(true);
        controle.travado = true;

        foreach (Roteiro _dialogo in prefabDialogo.transform.parent.GetComponentsInChildren<Roteiro>()) {
            _dialogo.ator = roteirosAtual.ator;
        }
    }

    void QuandoDeletarDialogo(Roteiro roteiro){
        roteiro.data.parent?.childrens.Remove(roteiro);
        roteiro.DELETADO -= QuandoDeletarDialogo;
        curDialogos.Remove(roteiro);
        alterouENaoSalvou = true;
    }

    void CriarDialogo(){
        CriarDialogo(null);
    }

    Roteiro CriarDialogo(Roteiro.Data curData = null){
        Roteiro newDialog = GameObject.Instantiate(prefabDialogo, prefabDialogo.transform.parent);
        newDialog.data.falaDoProtagonista = curData == null ? true : curData.falaDoProtagonista;
        newDialog.ator = curData == null || string.IsNullOrEmpty(curData._ator) ? roteirosAtual.ator : curData._ator;

        newDialog.DELETADO += QuandoDeletarDialogo;
        newDialog.SETOU_OPCAO += QuandoSetarOpcao;
        
        newDialog.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerDialogos);
        
        btnAddDialogo.transform.parent.SetAsLastSibling();
        curDialogos.Add(newDialog);
        
        if(curData != null){
            newDialog.SetData(curData); // * Carrega um data já salvo.
        }else{
            alterouENaoSalvou = true;
        }
        
        return newDialog;
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

    void QuandoSetarOpcao(Roteiro roteiro, bool isBranch){
        RamosDialogo alreadyBranched = roteiro.transform.parent.GetComponent<RamosDialogo>();


        if(alreadyBranched && !isBranch){
            alreadyBranched.transform.GetSiblingIndex();
            Transform novoRoteiro = CriarDialogo(roteiro.data).transform;
            alreadyBranched.RemoverOpcao(roteiro);
            Destroy(roteiro.gameObject);

            novoRoteiro.transform.SetSiblingIndex(alreadyBranched.transform.GetSiblingIndex()+1);
            btnAddDialogo.transform.parent.SetAsLastSibling();
        }

        if(isBranch){
            Roteiro addedRoteiro;

            if(roteiro.transform.parent.GetChild(roteiro.transform.GetSiblingIndex()-1).TryGetComponent(out alreadyBranched) && alreadyBranched.isActiveAndEnabled && alreadyBranched.temEspaco){
                alreadyBranched.ator = roteirosAtual.ator;
                addedRoteiro = alreadyBranched.AddRoteiro(roteiro);

            }else{
                alreadyBranched = GameObject.Instantiate(horizontalOption, horizontalOption.transform.parent);
                alreadyBranched.NOVO_DIALOGO += QuandoRamoHorizontalCriaOpcao;
                alreadyBranched.ator = roteirosAtual.ator;
                alreadyBranched.gameObject.SetActive(true);
                alreadyBranched.transform.SetParent(roteiro.transform.parent);
                alreadyBranched.transform.SetSiblingIndex(roteiro.transform.GetSiblingIndex());
                addedRoteiro = alreadyBranched.AddRoteiro(roteiro);
            }
            
            var uis = alreadyBranched.GetComponentsInChildren<RectTransform>();
            foreach (var item in uis)            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            }
            
            btnAddDialogo.transform.parent.SetAsLastSibling();
            
            // linka os eventos do roteiro
            addedRoteiro.DELETADO += QuandoDeletarDialogo;
            addedRoteiro.SETOU_OPCAO += QuandoSetarOpcao;
        }
    }

    public void QuandoRamoHorizontalCriaOpcao(Roteiro roteiro){
        roteiro.SETOU_OPCAO += QuandoSetarOpcao;
    }

    void CarregarDialogoEscolhido(string path){
        Dialogo.Data desserializado = JsonUtility.FromJson<Dialogo.Data>(File.ReadAllText(path));
        SetDialogosAtual(desserializado);
    }

    void SalvarDialogo(){

        var path = StandaloneFileBrowser.SaveFilePanel("Salvar Roteiro", "", "Roteiro " + roteirosAtual.ator, "dialog");
        if(string.IsNullOrEmpty(path)) return;

        File.WriteAllText(path, Serializar());
        alterouENaoSalvou = false;
    }

    string Serializar(){
        Transform content = prefabDialogo.transform.parent;
        Dialogo.Data data = new Data();

        List<Roteiro.Data> serializado = new List<Roteiro.Data>();

        for (var i = 0; i < content.childCount; i++){
            GameObject obj = content.GetChild(i).gameObject;
            if(!obj.activeSelf) continue;

            RamosDialogo ramificacao;
            Roteiro dialogo;

            
            if(obj.TryGetComponent(out ramificacao)){
                
                for (var j = 0; j < ramificacao.roots.Count; j++){
                    // * escreve a coluna de dialogo na root da opção.
                    Roteiro optionBranch = ramificacao.roots[j];

                    optionBranch.data.childrens = new Roteiro.Data[optionBranch.childrens.Count];
                    for (var k = 0; k < optionBranch.childrens.Count; k++){
                        optionBranch.data.childrens[k] = optionBranch.childrens[k].data;
                    }
                    
                    serializado.Add(optionBranch.data);
                }

            }else if(obj.TryGetComponent(out dialogo)){
                serializado.Add(dialogo.data);
            }
        }

        roteirosAtual.dialogos = serializado.ToArray();
        return JsonUtility.ToJson(roteirosAtual, true);
    }


    void SelecionouPersonagem(string nome){
        roteirosAtual = conteudoDialogos[nome];
    } 

    protected override void OnTriggerEnter2D(Collider2D col){
        base.OnTriggerEnter2D(col);
        if(playerEstaNoTrigger)
        
        uiGerenciadorDialogos.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerPersonagens);
    } 
    
    protected override void OnTriggerExit2D(Collider2D col){
        base.OnTriggerExit2D(col);
        uiGerenciadorDialogos.SetActive(false);
    } 


    void SetDialogosAtual(Dialogo.Data desserializado){
        foreach (Toggle btnPersonagem in containerPersonagens.GetComponentsInChildren<Toggle>()){
            string nome = btnPersonagem.GetComponentInChildren<Text>().text;
            bool ativo = nome == desserializado.ator; 
            btnPersonagem.isOn = ativo;
            if(ativo) break;
        }

        Transform content = prefabDialogo.transform.parent;
        
        for (var i = 0; i < content.childCount; i++){
            GameObject obj = content.GetChild(i).gameObject;

            if(!obj.activeSelf) continue;


            if(obj.GetComponent<RamosDialogo>() || obj.GetComponent<Roteiro>()){
                Destroy(obj);
            }
        }

        curDialogos.Clear();
        roteirosAtual = desserializado;

        for (var i = 0; i < desserializado.dialogos.Length; i++){
            Roteiro.Data data = desserializado.dialogos[i];
            
            if(!data.opcao){
                CriarDialogo(data);
            }else{
                
                List<Roteiro.Data> branches = new List<Roteiro.Data>();
                
                for (var j = i; j < desserializado.dialogos.Length; j++){
                    data = desserializado.dialogos[j];
                
                    if(data.opcao){
                        i = j;
                        branches.Add(data);
                    }else{
                        i = j-1;
                        break;
                    }
                }
                
                Debug.Log("criando branch");
                CriarRamoHorizontal(branches, desserializado.ator);
            }
                
        }   

        testDialog.roteiro = Serializar();
    }

    void CriarRamoHorizontal(List<Roteiro.Data> branches, string ator){
        
        RamosDialogo options = GameObject.Instantiate(horizontalOption, horizontalOption.transform.parent);
        options.NOVO_DIALOGO += QuandoRamoHorizontalCriaOpcao;
        options.ator = roteirosAtual.ator;
        options.gameObject.SetActive(true);

        options.transform.SetParent(prefabDialogo.transform.parent);
        options.transform.SetAsLastSibling();
        btnAddDialogo.transform.parent.SetAsLastSibling();
        
        int added = 0;

        foreach (var data in branches){
            Roteiro opcaoRamo = options.AddRoteiro(null, null, data);
            
            if(opcaoRamo != null){
                opcaoRamo.SETOU_OPCAO += QuandoSetarOpcao;
                // opcaoRamo.DELETADO += LimparRamo;

                foreach (var childData in data.childrens){
                    childData._ator = ator;

                    Roteiro roteiroFilho = options.AddRoteiro(null, opcaoRamo.transform, childData);
                    if(roteiroFilho == null) continue;
                    
                    roteiroFilho.DELETADO += LimparFilho;
                    opcaoRamo.childrens.Add(roteiroFilho);
                    roteiroFilho.data.parent = opcaoRamo;
                }

                opcaoRamo.Rebuild();
            }

            added++;

            if(!options.temEspaco){
                branches.RemoveRange(0,added);
                if(branches.Count > 0){
                    Debug.Log("criar branch");
                    CriarRamoHorizontal(branches, ator);
                }
                break;
            }

            void LimparRamo(Roteiro excluido){
                options.roots.Remove(excluido);
            }

            void LimparFilho(Roteiro excluido){
                opcaoRamo.childrens.Remove(excluido);
            }
        }
    }
}
