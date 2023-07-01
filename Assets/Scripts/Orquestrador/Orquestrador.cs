using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Orquestrador : MonoBehaviour {
    public static Orquestrador instance;

    [SerializeField] Cenario cenario;

    [System.Serializable] public class SetterDialog {
        [Tooltip("Nome do roteiro")]
        public string name;
        public Dialogo dialogo;
        public bool loopable = true;
        public UnityEvent OnFinishDialog = new UnityEvent();
    }

    [System.Serializable] public class IntermediateEvent{
        public string name;
        public SetterDialog dialog;
        public UnityAction OnFire;
    }

    [System.Serializable] public class EventStep {
        public string name;
        
        [Space(10)]
        [Header("Begin Events")]
        public List<SetterDialog> startDialogs;
        public GameObject[] startActivate;
        public UnityEvent OnStart = new UnityEvent();

        [Space(10)]
        [Header("Intermediate Events")]
        public List<IntermediateEvent> intermediateEvents = new List<IntermediateEvent>();


        [Space(10)]
        [Header("End Events")]
        public GameObject[] endDeactivate;
        public UnityEvent OnEnd = new UnityEvent();

    }

    [SerializeField] List<EventStep> events = new List<EventStep>();
    EventStep currentEvent;

    public void Awake(){
        //todo carregar anteriores.
        instance = this;
    }

    public void Start(){
        // aplicar carregações..

    }

    public void ReceiveIntermediateEvent(string eventName){
        IntermediateEvent intermediate = currentEvent.intermediateEvents.Find(x => x.name == eventName);        
        if(intermediate == null) return;

    }

    public void EndCurrentStep(){

    }


    
    
    
}
