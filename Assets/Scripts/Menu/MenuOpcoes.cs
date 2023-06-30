using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuOpcoes : MonoBehaviour
{
    [Header("Opções Gráficas:")]
    public Toggle FullscreeenTog;
    public Toggle VsyncTog;
    public TMP_Dropdown ResolucoesDrop;
    public List<Resolucoes> ResolucoesSuportadas = new List<Resolucoes>();

    [Header("Opções de Áudio:")]
    public AudioMixer audioMixer;
    public TMP_Text masterLabel, musicaLabel, efeitosLabel;
    public Slider MasterSlider;
    public Slider MusicaSlider;
    public Slider EfeitosSlider;


    void OnEnable()
    {
        FullscreeenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            VsyncTog.isOn = false;
        }
        else
        {
            VsyncTog.isOn = true;
        }

        ResolucoesDrop.ClearOptions();

        Verificar_resolucoes();

        float masterVol = 0;
        audioMixer.GetFloat("MasterVol", out masterVol);
        MasterSlider.value = masterVol;
        masterLabel.text = Mathf.RoundToInt(MasterSlider.value + 100) + "%";
        float musicaVol = 0;
        audioMixer.GetFloat("MusicaVol", out musicaVol);
        MusicaSlider.value = musicaVol;
        musicaLabel.text = Mathf.RoundToInt(MusicaSlider.value + 100) + "%";
        float efeitosVol = 0;
        audioMixer.GetFloat("EfeitosVol", out efeitosVol);
        EfeitosSlider.value = efeitosVol;
        efeitosLabel.text = Mathf.RoundToInt(EfeitosSlider.value + 100) + "%";
    }

    private void Verificar_resolucoes()
    {
        List<string> opcoesResolucoes = new List<string>();

        int indiceResolucaoAtual = 0;

        if (Screen.resolutions != null)
        {
            ResolucoesSuportadas.Clear();
            foreach (var resolucao in Screen.resolutions)
            {
                ResolucoesSuportadas.Add(new Resolucoes(resolucao.width, resolucao.height /*, resolucao.refreshRate*/));
            }
        }

        for (int i = 0; i < ResolucoesSuportadas.Count; i++)
        {
            string opcao = ResolucoesSuportadas[i].ToString();
            opcoesResolucoes.Add(opcao);

            if (ResolucoesSuportadas[i].Largura == Screen.width &&
                               ResolucoesSuportadas[i].Altura == Screen.height)
            {
                Debug.Log("Resolução atual: " + opcao);
                indiceResolucaoAtual = i;
            }
        }

        ResolucoesDrop.AddOptions(opcoesResolucoes);
        ResolucoesDrop.value = indiceResolucaoAtual;
    }    

    public void AplicarGraficos()
    {
          if (FullscreeenTog.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }

        if (VsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        Screen.SetResolution(ResolucoesSuportadas[ResolucoesDrop.value].Largura, ResolucoesSuportadas[ResolucoesDrop.value].Altura, FullscreeenTog.isOn);
    }

    public void AplicarVolumeMaster()
    {
        audioMixer.SetFloat("MasterVol", MasterSlider.value);
        masterLabel.text = Mathf.RoundToInt(MasterSlider.value + 100) + "%";
    }
    public void AplicarVolumeMusica()
    {
        audioMixer.SetFloat("MusicaVol", MusicaSlider.value);
        musicaLabel.text = Mathf.RoundToInt(MusicaSlider.value + 80) + "%";
    }
    public void AplicarVolumeEfeitos()
    {
        audioMixer.SetFloat("EfeitosVol", EfeitosSlider.value);
        efeitosLabel.text = Mathf.RoundToInt(EfeitosSlider.value + 80) + "%";
    }
}

[System.Serializable]
public class Resolucoes
{
    public int Largura;
    public int Altura;
    //public int RefreshRate;

    public Resolucoes(int largura, int altura /*, int refreshRate */)
    {
        Largura = largura;
        Altura = altura;
        //RefreshRate = refreshRate;
    }

    public override string ToString()
    {
        return Largura + " x " + Altura /* + " @ " + RefreshRate + "Hz"*/;
    }
}