using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightColor : MonoBehaviour
{
    [SerializeField] Color night = new Color(0.34f, 0.36f, 0.43f,1);
    [SerializeField] Color day = Color.white;

    Light2D light2d;

    void Awake(){
        light2d = GetComponent<Light2D>();
    }

    public void SetColor(bool isDay){
        light2d.color = isDay ? day : night;
    }

}
