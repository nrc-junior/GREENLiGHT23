using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGamePause : MonoBehaviour
{
    [SerializeField]
    public int TelainicialIndex;
    public GameObject ConfiguracoesGame;
    public GameObject MenuPause;


    // Botei aqui somente para teste.



    public void VoltarJogo()
    {
        Debug.Log("Voltar Jogo");
        MenuPause.SetActive(false);
    }

    public void SairJogo()
    {
        SceneManager.LoadScene(TelainicialIndex);
    }

    public void AbrirPauseConfiguracoes()
    {
        ConfiguracoesGame.SetActive(true);
    }

    public void FecharPauseConfiguracoes()
    {
        ConfiguracoesGame.SetActive(false);
    }

}


