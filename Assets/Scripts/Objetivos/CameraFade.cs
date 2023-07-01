using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public static CameraFade instance;
    public Action Fadeout;
    public Action Fadein;
    [Header("Configuracoes do Fade")]
    public float ForçaDaCurva = 1f;
    public AnimationCurve CurvaOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public AnimationCurve CurvaIn = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public bool ComeçarFadeOut = false;
    public float DuracaoDoFade = 1f;
    public Image ImagenFade;
    public Color CorInicio;
    public Color CorFim = new Color(0, 0, 0, 1);


    private float alpha = 0f;
    private int direction = 0;
    private float time = 0f;
    private static bool JaIniciou = false;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        if (JaIniciou) return;
        if (ComeçarFadeOut) alpha = 1f; else alpha = 0f;
        ImagenFade.color = Color.Lerp(CorInicio, CorFim, alpha);
    }

    public void FadeOut()
    {
        Debug.Log("FadeOut");
        alpha = 0f;
        time = 1f;
        direction = -1;
        StartCoroutine(FadeRotina());
    }

    public void FadeIn()
    {
        Debug.Log("FadeIn");
        alpha = 1f;
        time = 0f;
        direction = 1;
        StartCoroutine(FadeRotina());
    }

    public IEnumerator FadeRotina()
    {
        while(direction != 0)
        {

            if(direction == -1)
            {
                time += (direction * Time.fixedDeltaTime * ForçaDaCurva);
                alpha = Mathf.Lerp(0f, 1f, (Mathf.Abs(time) / DuracaoDoFade) * CurvaOut.Evaluate(time) * ForçaDaCurva);
                ImagenFade.color = Color.Lerp(CorInicio, CorFim, alpha);
                //Debug.Log("Evaluete: " + CurveOut.Evaluate(time));
                //Debug.Log("Alpha   : " + alpha);
                //Debug.Log("Time    : " + time);

                if (-time >= DuracaoDoFade && alpha >= 1)
                {
                    Debug.Log("Acabou o Fade");
                    direction = 0;
                    Fadeout?.Invoke();
                }
            }

            if (direction == 1)
            {
                time += (direction * Time.fixedDeltaTime * ForçaDaCurva);
                alpha = Mathf.Lerp(1f, 0f, (Mathf.Abs(time) / DuracaoDoFade) * CurvaIn.Evaluate(time / DuracaoDoFade) * ForçaDaCurva);
                ImagenFade.color = Color.Lerp(CorInicio, CorFim, alpha);
                //Debug.Log("Evaluete: " + CurveOut.Evaluate(time));
                //Debug.Log("Alpha   : " + alpha);
                //Debug.Log("Time    : " + time);

                if (time >= DuracaoDoFade && alpha <= 1)
                {
                    direction = 0;
                    Fadein?.Invoke();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}



