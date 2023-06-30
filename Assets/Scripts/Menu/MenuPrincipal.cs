using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField]
    public int PrimeiraFaseIndex;
    public GameObject Configuracoes;

    public void IniciarJogo()
    {
        SceneManager.LoadScene(PrimeiraFaseIndex);
    }

    public void AbrirConfiguracoes()
    {
        Configuracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        Configuracoes.SetActive(false);
    }

    public void FecharJogo()
    {
        Application.Quit();
    }

}


