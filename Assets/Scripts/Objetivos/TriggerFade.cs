using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TriggerFade : MonoBehaviour
{
    public static TriggerFade instance;
    [Header("Configuracoes do Fade")]
    public float ForcaDaCurva = 1f;
    public AnimationCurve Curva = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public float DuracaoDoFade = 1f;
    public Action Fade;
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
        alpha = 0f;
        ImagenFade.color = Color.Lerp(CorInicio, CorFim, alpha);
    }

    public void TFade()
    {
        Debug.Log("FadeOut");
        alpha = 0f;
        time = 1f;
        direction = -1;
        StartCoroutine(FadeRotina());
    }

    public IEnumerator FadeRotina()
    {
        while (direction != 0)
        {
            if (direction == -1)
            {
                time += Time.fixedDeltaTime;
                float progress = Mathf.Clamp01(time / DuracaoDoFade);
                float curveValue = Curva.Evaluate(progress);
                //Debug.Log("Alpha   : " + curveValue);
                // ImagenFade.color = Color.Lerp(CorInicio, CorFim, curveValue);

                // if (progress >= 1f)
                // {
                //     Debug.Log("Fade conclu�do");
                //     direction = 0;
                //     Fade?.Invoke();
                // }

            }
            yield return new WaitForFixedUpdate();
        }
    }
}
