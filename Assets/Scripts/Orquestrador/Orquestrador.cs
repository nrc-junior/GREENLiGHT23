using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Orquestrador : MonoBehaviour {
    public static Orquestrador instance;

    [SerializeField] Cenario cenario;
    [System.Serializable] public class FadeEvent {
        public bool useFade;
        public string message = "";
        public bool startHiding;
        public bool waitToDispatch;
        public float fadeDuration = 1;
        public float waitDuration = 1;
    }

    [System.Serializable] public class SetterDialog {
        [Tooltip("Nome do roteiro")]
        public string name;
        public Dialogo dialogo;
        public bool loopable = true;
        public UnityEvent OnFinishDialog = new UnityEvent();
        public GameObject[] deactivateOnFinish;
    }

    [System.Serializable] public class IntermediateEvent{
        public string name;
        public FadeEvent fade = new FadeEvent();
        public GameObject[] deactiveList;
        public GameObject[] activeList;
        public List<SetterDialog> dialogsSetter = new List<SetterDialog>();
        public UnityEvent OnFireEvent = new UnityEvent();
    }

    [System.Serializable] public class EventStep {
        public string name;
        
        public bool isIndoor;

        [Space(10)]
        [Header("Begin Events")]
        public List<SetterDialog> startDialogs;
        public GameObject[] startActivate;
        public GameObject[] startDeactivate;
        public FadeEvent startFade = new FadeEvent();
        
        [Space]
        public UnityEvent OnStart = new UnityEvent();

        [Space(5)]
        [Header("Intermediate Events")]
        public List<IntermediateEvent> intermediateEvents = new List<IntermediateEvent>();


        [Space(5)]
        [Header("End Events")]
        public GameObject[] endDeactivate;
        public FadeEvent endFade = new FadeEvent();
        [Space]
        public UnityEvent OnEnd = new UnityEvent();

    }

    [SerializeField] List<EventStep> events = new List<EventStep>();
    EventStep currentEvent;
    int nextEventIdx = 0;
    Fade fadeControl;

    public void Awake(){
        //todo carregar anteriores.
        instance = this;
    }
    public void Start(){
        // aplicar carregações..
        fadeControl = Fade.instance;
        foreach (GameObject item in events[0].startDeactivate){
            item.SetActive(false);
        }

        Play(events[nextEventIdx++]);
    }

    void Play(EventStep step){
        Debug.Log($":: Iniciou o passo: {step.name} ::");
        
        FadeEvent fade = step.startFade;
        if(fade.useFade){
            fadeControl.duracaoFade = fade.fadeDuration;
            fadeControl.delay = fade.waitDuration;

            if(fade.startHiding){
                fadeControl.FadeIn(fade.message);
            }

        }

        cenario.AtivarCenario(step.isIndoor);
        
        foreach (SetterDialog dialogData in step.startDialogs){
            SetupDialog(dialogData);
        }
        
        foreach (GameObject activeItem in step.startDeactivate){
            activeItem.SetActive(false);    
        }
        
        foreach (GameObject activeItem in step.startActivate){
            activeItem.SetActive(true);    
        }

        step.OnStart?.Invoke();

        currentEvent = step;

    }

    void FadeIn(){
        fadeControl.FADEOUT_COMPLETE -= FadeIn;
        fadeControl.FadeIn();
    }

    public void ReceiveIntermediateEvent(string eventName){
        IntermediateEvent intermediate = currentEvent.intermediateEvents.Find(x => x.name == eventName);        
        if(intermediate == null) return;
        
        FadeEvent fade = intermediate.fade;
        
        if(fade.useFade){
            fadeControl.delay = fade.waitDuration;
            fadeControl.duracaoFade = fade.fadeDuration;
            
            if(fade.startHiding){
                if(fade.waitToDispatch)
                    fadeControl.FADEIN_COMPLETE += PlayIntermediateEvent;
                
                fadeControl.FadeIn(fade.message);
                
            }else{
                if(fade.waitToDispatch)
                    fadeControl.FADEOUT_COMPLETE += PlayIntermediateEvent;
                
                fadeControl.FadeOut(fade.message);
                fadeControl.FADEOUT_COMPLETE += FadeIn;
            }
        }

        if(!fade.waitToDispatch){
            PlayIntermediateEvent();
        }

        void  PlayIntermediateEvent(){
            Debug.Log($":: Tocou evento intermediario: {intermediate.name} ::");

            fadeControl.FADEIN_COMPLETE -= PlayIntermediateEvent;
            fadeControl.FADEOUT_COMPLETE -= PlayIntermediateEvent;
            
            foreach (GameObject deactive in intermediate.deactiveList){
                deactive.gameObject.SetActive(false);
            } 

            foreach (GameObject active in intermediate.activeList){
                active.gameObject.SetActive(true);
            } 

            foreach (SetterDialog dialogData in intermediate.dialogsSetter){
                SetupDialog(dialogData);
            }
            
            intermediate.OnFireEvent?.Invoke();
        }
        

    }

    void SetupDialog(SetterDialog data){
        Dialogo dialogo = data.dialogo;
        string roteiro = (Resources.Load(data.name) as TextAsset).text;
        dialogo.loopable = data.loopable;
        dialogo.roteiro = roteiro;
        dialogo.QUIT += StoppedPlaying;

        void StoppedPlaying(){
            dialogo.QUIT -= StoppedPlaying;
            data.OnFinishDialog?.Invoke();

            foreach (GameObject deactiveItem in data.deactivateOnFinish){
                deactiveItem.gameObject.SetActive(false);
            } 
        }
    }
    public void EndCurrentStep(){
        FadeEvent fade = currentEvent.endFade;
        
        if(fade.useFade){
            fadeControl.delay = fade.waitDuration;
            fadeControl.duracaoFade = fade.fadeDuration;
            
            if(fade.startHiding){
                if(fade.waitToDispatch)
                    fadeControl.FADEIN_COMPLETE += PlayEndEvent;
                
                fadeControl.FadeIn(fade.message);
                
            }else{
                if(fade.waitToDispatch)
                    fadeControl.FADEOUT_COMPLETE += PlayEndEvent;
                
                fadeControl.FadeOut(fade.message);
                fadeControl.FADEOUT_COMPLETE += FadeIn;
            }
        }

        if(!fade.waitToDispatch){
            PlayEndEvent();
        }

        void PlayEndEvent(){
            Debug.Log($":: Tocou fim do evento: {currentEvent.name} ::");

            fadeControl.FADEIN_COMPLETE -= PlayEndEvent;
            fadeControl.FADEOUT_COMPLETE -= PlayEndEvent;
            
            foreach (GameObject deactive in currentEvent.endDeactivate){
                deactive.gameObject.SetActive(false);
            } 

            currentEvent.OnEnd?.Invoke();

            if(nextEventIdx < events.Count){
                Play(events[nextEventIdx++]);
            }
        }
    }
}
