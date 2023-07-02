using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageEnd : MonoBehaviour {

    [SerializeField] Button exit;
    [SerializeField] CanvasGroup group;

    void Start(){
        exit.onClick.AddListener(()=> Application.Quit());
    }

    void OnEnable(){
        LeanTween.value(gameObject, 0, 1, 8).setOnUpdate((float v )=> {
            group.alpha = v;
        });
    }
}
