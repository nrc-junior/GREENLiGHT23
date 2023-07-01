using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public static CameraFade instance;
    public float speedScale = 1f;
    public AnimationCurve CurveOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public AnimationCurve CurveIn = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public bool startFadedOut = false;
    public float fadeDuration = 1f;
    public Action Fadeout;
    public Action Fadein;
    public Image texture;
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
        if (startFadedOut) alpha = 1f; else alpha = 0f;
        texture.color = Color.Lerp(CorInicio, CorFim, alpha);
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
            if(alpha > 0f)
            {
                //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
            }

            if(direction == -1)
            {
                time += (direction * Time.fixedDeltaTime * speedScale);
                alpha = Mathf.Lerp(0f, 1f, (Mathf.Abs(time) / fadeDuration) * CurveOut.Evaluate(time) * speedScale);
                texture.color = Color.Lerp(CorInicio, CorFim, alpha);
                //Debug.Log("Evaluete: " + CurveOut.Evaluate(time));
                //Debug.Log("Alpha   : " + alpha);
                //Debug.Log("Time    : " + time);

                if (-time >= fadeDuration && alpha >= 1)
                {
                    Debug.Log("Acabou o Fade");
                    direction = 0;
                    Fadeout?.Invoke();
                }
            }

            if (direction == 1)
            {
                time += (direction * Time.fixedDeltaTime * speedScale);
                alpha = Mathf.Lerp(1f, 0f, (Mathf.Abs(time) / fadeDuration) * CurveIn.Evaluate(time / fadeDuration) * speedScale);
                texture.color = Color.Lerp(CorInicio, CorFim, alpha);
                //Debug.Log("Evaluete: " + CurveOut.Evaluate(time));
                //Debug.Log("Alpha   : " + alpha);
                //Debug.Log("Time    : " + time);

                if (time >= fadeDuration && alpha <= 1)
                {
                    direction = 0;
                    Fadein?.Invoke();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}



