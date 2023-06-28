using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs {
    public class Dialog : MonoBehaviour {
        private readonly Color _alpha = new Color(1, 1, 1, 0);
        public TextMeshProUGUI debugText;

        public RectTransform dialogInterface;
        public GameObject labelPrefab;
        public GameObject optionPrefab;
        private Dictionary<int, Node> dialog;

        private int curActor;
        private int curHumor;
        private string lastEvent;


        public void StartDialog(int id)
        {
            dialog = DialogsHandler.instance.loadedDialogs[id];

            Node start = GetFirstNodeType(NodeType.START);
            if (start == null) return;

            Node curNode = start.targets.Pop();
            PlayDialog(curNode);
        }

        private Dictionary<Node, Button> options;

        async void PlayDialog(Node node)
        {
            bool canContinue = true;

            //remover switch chamar um dict de 
            switch (node.type)
            {
                case NodeType.EVENT:
                    Event dialogEvent = JsonUtility.FromJson<Event>(node.values);
                    lastEvent = dialogEvent.callback;
                    UpdateDebugText();

                    break;
                case NodeType.ANSWER:
                    break;
                case NodeType.OPTIONS:
                    options = new Dictionary<Node, Button>(node.targets.Count);

                    foreach (var nodeTarget in node.targets)
                    {
                        Button option = Instantiate(optionPrefab, dialogInterface.transform).GetComponent<Button>();

                        options.Add(nodeTarget, option);
                        TextMeshProUGUI labelOption = option.GetComponentInChildren<TextMeshProUGUI>();
                        labelOption.text = nodeTarget.values;
                        labelOption.color *= _alpha;
                        LeanAlphaText(labelOption, 1, .4f);
                        option.onClick.AddListener(delegate { SelectOption(nodeTarget.id); });
                        await Task.Delay(500);
                    }

                    canContinue = false;
                    break;

                case NodeType.DIALOG:
                    Debug.Log(node.values);
                    Actor newActor = JsonUtility.FromJson<Actor>(node.values);
                    curActor = newActor.actorId;
                    curHumor = newActor.humorId;
                    UpdateDebugText();
                    break;

                case NodeType.LABEL:
                    TextMeshProUGUI label = Instantiate(labelPrefab, dialogInterface.transform)
                        .GetComponent<TextMeshProUGUI>();
                    label.text = node.values;
                    label.color *= _alpha;
                    LeanAlphaText(label, 1, .4f);
                    break;

                case NodeType.END:
                    Debug.Log("acabou o roteiro");
                    canContinue = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshUI();

            if (!canContinue) return;
            Node nextNode = node.targets.Pop();
            await Task.Delay(500);
            PlayDialog(nextNode);
        }

        private void UpdateDebugText()
        {
            debugText.text = $"current actor: {curActor}\ncurrent humor: {curHumor} \nlast event: {lastEvent}";
        }

        async void SelectOption(int id)
        {
            Node answer = new Node();
            foreach (var option in options)
            {
                option.Value.onClick.RemoveAllListeners();

                if (id == option.Key.id)
                {
                    option.Value.interactable = false;
                    answer = option.Key;
                }
                else
                {
                    LeanAlphaText(option.Value.GetComponentInChildren<TextMeshProUGUI>(), 0, .5f).setOnComplete(() =>
                    {
                        Destroy(option.Value.gameObject);
                        RefreshUI();
                    });
                }
            }

            await Task.Delay(500);
            PlayDialog(answer.targets.Pop());

            options = null;
        }

        void RefreshUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogInterface);
        }

        public Node GetFirstNodeType(NodeType type)
        {
            foreach (var e in dialog)
                if (e.Value.type == NodeType.START)
                {
                    return e.Value;
                }

            return null;
        }

        public static LTDescr LeanAlphaText(TextMeshProUGUI textMesh, float to, float time)
        {
            var _color = textMesh.color;
            var _tween = LeanTween
                .value(textMesh.gameObject, _color.a, to, time)
                .setOnUpdate((float _value) =>
                {
                    _color.a = _value;
                    textMesh.color = _color;
                });
            return _tween;
        }
    }
}