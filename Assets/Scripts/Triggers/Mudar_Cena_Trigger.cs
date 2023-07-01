using UnityEngine.SceneManagement;
using UnityEngine;


public class Mudar_Cena_Trigger : MonoBehaviour
{
    [Header("Configurações do Trigger")]
    public int QualCenaTrocar;
    public bool fazPiscar = false;

    private bool isTransitioning = false;
    private bool isTriggered = false;

    private void Awake()
    {
        //Camera_Fade CameraFade = FindObjectOfType<Camera_Fade>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return; // Ignore triggers while transitioning

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou no Trigger");
            Debug.Log(isTriggered);
            if (fazPiscar && !isTriggered)
            {
                Debug.Log("Piscou");
                isTriggered = true;
                TriggerFade.instance.TFade();
                TriggerFade.instance.Fade += triggerAcabou;
            }
            else if (!fazPiscar)
            {
                ChangeScene();
            }
        }
    }

    private void ChangeScene()
    {
        isTransitioning = true;

        if (CameraFade.instance != null)
        {
            // FadeOut
            CameraFade.instance.FadeOut();
            CameraFade.instance.Fadeout += EmAcabarFadeOut;
            Debug.Log("Ativou a cena");
        }

        isTransitioning = false;
    }

    private void EmAcabarFadeOut()
    {
        CameraFade.instance.FadeIn();
        SceneManager.LoadScene(QualCenaTrocar);
        CameraFade.instance.Fadeout -= EmAcabarFadeOut;

    }

    private void triggerAcabou()
    {
        isTriggered = false;
        CameraFade.instance.Fadeout -= triggerAcabou;
    }
}



