using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fade : MonoBehaviour {
    public static Fade instance;
    public Action FADEOUT_COMPLETE;
    public Action FADEIN_COMPLETE;

    [Header("Configuracoes do Fade")]
    public float forcaDaCurva = 1f;
    public AnimationCurve CurvaOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public AnimationCurve CurvaIn = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public bool comecarFadeOut = false;
    public float duracaoFade = 1f;
    public float delay { private get; set; } = 0;
   
    public Image imgFade;
    [SerializeField] private Text txFade;

    public Color CorInicio;
    public Color CorFim = new Color(0, 0, 0, 1);

    public Color corTextoInicio;
    public Color corTextoFim = new Color(1, 1, 1, 1);

    private float alpha = 0f;
    private int direction = 0;
    private float time = 0f;
    private static bool JaIniciou = false;

    void Awake() {
        if(instance != null && instance != this){
            Destroy(gameObject);
        }else{
            instance = this;
            txFade ??= GetComponentInChildren<Text>();
            imgFade ??= GetComponentInChildren<Image>();
            
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        if (JaIniciou) return;
        if (comecarFadeOut) {
            alpha = 1f; 
        } else {
            alpha = 0f;
        }

        imgFade.color = Color.Lerp(CorInicio, CorFim, alpha);
        txFade.text = "";
        
        if(comecarFadeOut){
            StartCoroutine(WaitSceneFinishLoad());
        }

        IEnumerator WaitSceneFinishLoad(){ // a unity da uma travadinha na cena quando carrega, coloca pra so tocar quando o fps tiver bom.
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            FadeOut();
        }
    }

    public void FadeOut(string text = "") {
        Debug.LogWarning("Chamou FadeOut");
        if(direction == -1){
            Debug.LogWarning("caiu aqui");
            return; // ja esta fazendo um fadeout.
        }else if(direction == 1){
            if(!string.IsNullOrEmpty(text)){
                txFade.text = text;
            }
            Debug.LogWarning("caiu aqui");

            FADEOUT_COMPLETE?.Invoke();
            return;
        }

        alpha = 0f;
        time = 0;
        direction = -1;
        txFade.text = text;
        imgFade.color = CorInicio;
        
        Debug.LogError("Esconder");
        StartCoroutine(FadeRotina());
    }

    bool isFadingText;

    public void FadeIn(string text = "") {
        Debug.LogWarning("Chamou FadeIn");
        if(!string.IsNullOrEmpty(text))
            Debug.LogWarning("caiu aqui");
            txFade.text = text;

        if(direction == 1){
            Debug.LogWarning("caiu aqui");

            if(!string.IsNullOrEmpty(txFade.text)){
                Debug.LogWarning("caiu aqui");
                isFadingText = true;
                LeanTween.value(gameObject, 0, 1, duracaoFade/2).setOnUpdate((float v)=> txFade.color = Color.Lerp(corTextoInicio, corTextoFim,v)).setOnComplete( () => isFadingText = false);
            }
        }

        alpha = 1f;
        time = 0f;
        direction = 1;
        imgFade.color = CorFim;

        Debug.LogError("Mostrar");
        StartCoroutine(FadeRotina());
    }
    
    IEnumerator FadeRotina() {
        if(direction == 1){
            yield return new WaitForSeconds(delay);
            Debug.Log("terminou a espera" + isFadingText);
            if(isFadingText){
                yield return new WaitUntil(() => !isFadingText);
            }
        }

        while(direction != 0) {

            if(direction == -1) { // Fadeout, Esconde a tela 

                time +=  Time.fixedDeltaTime * forcaDaCurva;
                alpha = Mathf.Lerp(0f, 1f, (time / duracaoFade) * CurvaOut.Evaluate(time) * forcaDaCurva);
                imgFade.color = Color.Lerp(CorInicio, CorFim, alpha);
                txFade.color = Color.Lerp(corTextoFim, corTextoInicio, alpha);

                if (time >= duracaoFade) {
                    direction = 0;
                    FADEOUT_COMPLETE?.Invoke();
                }
            }

            if (direction == 1) { // Fadein, Exibe a tela 

                time += Time.fixedDeltaTime * forcaDaCurva;
                alpha = Mathf.Lerp(1f, 0f, (Mathf.Abs(time) / duracaoFade) * CurvaIn.Evaluate(time / duracaoFade) * forcaDaCurva);
                imgFade.color = Color.Lerp(CorInicio, CorFim, alpha);
                txFade.color = Color.Lerp(corTextoInicio, corTextoFim, alpha);

                if (time >= duracaoFade) {
                    direction = 0;
                    FADEIN_COMPLETE?.Invoke();
                }
            }

            yield return new WaitForFixedUpdate();
        }
        delay = 0;
    }
}



