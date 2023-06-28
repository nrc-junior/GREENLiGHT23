using UnityEngine;

public class ControladorDialogo : MonoBehaviour{
    public static ControladorDialogo instancia;

    void Awake(){
        instancia = this;
    }

    public void CarregarDialogo(string dialogoNome){
        Debug.Log(dialogoNome);
        
    }
}