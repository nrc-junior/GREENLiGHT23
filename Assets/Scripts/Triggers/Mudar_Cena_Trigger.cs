using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class Mudar_Cena_Trigger : MonoBehaviour
{
    public int IndexCena;
    public Camera_Fade CameraFade; // Assign this variable in the Inspector

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return; // Ignore triggers while transitioning

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou no Trigger");
            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        isTransitioning = true;

        // Check if CameraFade is assigned
        if (CameraFade != null)
        {
            // FadeOut
            CameraFade.FadeOut();
            yield return new WaitForSeconds(CameraFade.fadeDuration);

            Debug.Log("Ativou a cena");
            //SceneManager.LoadScene(IndexCena);

            // FadeIn
            CameraFade.FadeIn();
            yield return new WaitForSeconds(CameraFade.fadeDuration);
        }
        else
        {
            Debug.LogError("CameraFade is not assigned in Mudar_Cena_Trigger.");
        }

        isTransitioning = false;
    }
}
