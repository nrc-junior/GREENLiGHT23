using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{

    public GameObject MenuPause;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuPause.activeSelf)
            {
                MenuPause.SetActive(false);
            }
            else
            {
                MenuPause.SetActive(true);
            }
        }
    }
}
