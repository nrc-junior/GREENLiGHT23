using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorDialogo : MonoBehaviour{
    public static ControladorDialogo instancia;
    
    [SerializeField] RectTransform canvas;


    [SerializeField] Text nome;
    [SerializeField] Text dialogo;
    [SerializeField] Text prefOpcao;
    
    [SerializeField] RectTransform containerDialogo;
    [SerializeField] RectTransform containerOpcao;

    [SerializeField] Image profilePic;
    
    [Space(10)] [SerializeField] MovimentoIsometrico movimento;


    List<string> eventosOuvidos = new List<string>();

    Action NEXT;
    bool tocando = false;
    bool fim {get => idx >= dialogos.Length; }

    void Awake(){
        instancia = this;
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvas);
        canvas.gameObject.SetActive(false);
    }
    public void Update(){
        
        if(tocando && !estaEscolhendo && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))){
            if(fim || comando_sair){
                comando_sair = false;
                Sair();
                return;
            }
            
            if(tocandoOpcao){

                if(idxJ >= ramoOpcao.Length){
                    tocandoOpcao = false;
                    ramoOpcao = null;
                    idxJ = 0;
                    
                    if(retonaPraUltimaOpcao){
                        Exibir(dialogos[ultimaIdxOpcao]);
                    }else if(!fim){
                        Exibir(dialogos[idx]);
                    }else{
                        Sair();
                    }
                    return;
                }

                Exibir(ramoOpcao[idxJ]);
            }else{
                Exibir(dialogos[idx]);
            }
        }
    }

    public void CarregarDialogo(string dialogoNome){
        Debug.Log(dialogoNome);
    }


    string nomeCoadjuvante;
    int idx = 0;
    Roteiro.Data[] dialogos;
    
    int ultimaIdxOpcao;
    bool tocandoOpcao;
    bool retonaPraUltimaOpcao;
    Roteiro.Data[] ramoOpcao;
    int idxJ = 0;

    bool estaEscolhendo;

    public void TocarDialogo(string dialogo){
        if(tocando) return;
        
        movimento.travado = true;
        tocando = true;

        Dialogo.Data data = JsonUtility.FromJson<Dialogo.Data>(dialogo);
        
        idx = 0;
        nomeCoadjuvante = data.ator;
        dialogos = data.dialogos;
        comando_sair = false;

        canvas.gameObject.SetActive(true);
        Exibir(dialogos[idx]);

    }
    bool comando_sair;

    void Exibir(Roteiro.Data dialogData, bool recursivo = false){
        nome.text = dialogData.falaDoProtagonista ? "Chad" : nomeCoadjuvante;


        if(dialogData.opcao){
            containerDialogo.gameObject.SetActive(false);
            containerOpcao.gameObject.SetActive(true);
            
            ultimaIdxOpcao = idx;
            GerarOpcoes();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerOpcao);

        }else{
        
            if(dialogData.disparaEvento){
                string evento = dialogData.textoDispararEvento;
                comando_sair = evento.ToLower().Trim() == "sair" || evento.ToLower().Trim() == "exit";
                eventosOuvidos.Add(evento);
            }

            containerOpcao.gameObject.SetActive(false);
            containerDialogo.gameObject.SetActive(true);
            dialogo.text = dialogData.textoDialogo;
            
            // LayoutRebuilder.ForceRebuildLayoutImmediate(containerDialogo);
        }

        // incrementa dialogo.
        if(tocandoOpcao){
            idxJ++;
        }else if(!recursivo){
            idx++;
        }       
        
        // if(recursivo)
        //     idx--;

    }

    void GerarOpcoes(){
        List<Roteiro.Data> opcoes = new List<Roteiro.Data>();

        for (var i = idx; i < dialogos.Length; i++){
            if(!dialogos[i].opcao){
                idx = i-1;
                break;
            }else{
                idx = i+1;
                opcoes.Add(dialogos[i]);
            }
        }

        GameObject[] objOpcoes = new GameObject[opcoes.Count]; //objetos para limpar depois
        int j = 0;

        foreach (var opcao in opcoes){
            
            if(opcao.ouvirEvento){
                if(!eventosOuvidos.Contains(opcao.textoEscutarEvento)){
                    continue;
                }
            }

            Text txOpcao = GameObject.Instantiate(prefOpcao, containerOpcao.transform);
            objOpcoes[j++] = txOpcao.gameObject;
            txOpcao.gameObject.SetActive(true);

            txOpcao.text = $"<color=red>{opcao.textoDialogo}</color>";
            txOpcao.GetComponent<Button>().onClick.AddListener(SelecionarOpcao);

            // chamadas local
            void SelecionarOpcao(){
                
                foreach (GameObject obj in objOpcoes){
                    Destroy(obj);
                }

                EscolheuOpcao(opcao);
                
            }
        }

        if(j == 0){
            idx++;
            if(!fim){
                Exibir(dialogos[idx], true);
            }else{
                Sair();
            }
        }else{
            estaEscolhendo = true;
        }
    }

    void EscolheuOpcao(Roteiro.Data opcao){
        estaEscolhendo = false;

        if(opcao.disparaEvento){
            string evento = opcao.textoDispararEvento;
            comando_sair = evento.ToLower().Trim() == "sair" || evento.ToLower().Trim() == "exit";
            
            if(comando_sair){
                Sair();
                return;
            }else{
                eventosOuvidos.Add(evento);
            }
        }

        ramoOpcao = opcao.childrens;
        tocandoOpcao = ramoOpcao.Length > 0;
        
        if(opcao.retornar){
            idx = ultimaIdxOpcao;
        };
        
        if(tocandoOpcao){
            Exibir(ramoOpcao[0]);
        }else if(!fim){
            Exibir(dialogos[idx]);
        }else{
            Sair();
        }
    }

    void Sair(){
        tocando = false;
        canvas.gameObject.SetActive(false);
        movimento.travado = false;
        return;
    }
    
    public void LimparEventos(){
        eventosOuvidos.Clear();
    }


}