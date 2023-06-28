using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Dialogs {
    
    public class XMLHandler{
        
        private XmlNamespaceManager xmlHandler;
        private XmlReader reader;
        
        public void Start(string path) {
            Dictionary<int,Node> nodes = new Dictionary<int,Node>();
            Node curNode = new Node();
            curNode.id = -1;

            reader = XmlReader.Create(path);
            int dialogID = 0;
            bool firstEdge = true;
            string data = "";
            while (reader.Read()) {
                switch (reader.Name.ToString()) {
                    case "node":
                        try {
                            int id = int.Parse(reader.GetAttribute(0)[1..]);
                            if (id == curNode.id) continue;

                            if(curNode.id >= 0) nodes.Add(curNode.id, curNode);   
                            curNode = new Node {
                                id = id
                            };
                            
                        } catch (Exception e) {
                            Debug.LogError("não foi posssivel gerar node... " + e);
                        } 
                        break;
                    
                    case "data":
                        data = reader.ReadString().Trim();
                        if(data.Length > 0 && data[0] == '{')
                            curNode.SetValue(data);
                        break;
                    
                    case "y:NodeLabel":
                        data = reader.ReadString();
                        if (data.Trim().Length > 0) {
                            curNode.SetLabel(data);
                        }
                        break;
                    
                    case "edge":
                        try {
                            if (firstEdge) {
                                // quando começa a mapear edges adiciona o ultimo node.
                                nodes.Add(curNode.id, curNode);
                                firstEdge = false;
                            }
                            
                            if (reader.GetAttribute(0)?[0] == 'e') {
                                int root = int.Parse(reader.GetAttribute(1)[1..]);
                                int target = int.Parse(reader.GetAttribute(2)[1..]);
                                
                                nodes[root].targets.Push(nodes[target]);
                                nodes[target].roots.Push(nodes[root]);
                            }
                        }
                        catch (Exception e) {
                            Console.WriteLine(e);
                        }
                        break;
                }
            }
            
            
            // foreach (var node in nodes) {
            //     Debug.Log(node.Value.ToString());
            // }
            
            DialogsHandler.instance.loadedDialogs.Add(dialogID, nodes); 
        }
    }

    public enum NodeType {
        START,
        EVENT,
        ANSWER,
        OPTIONS,
        DIALOG,
        LABEL,
        END
    }

    public class Node {
        public int id = -1;
        public NodeType type;
        public Stack<Node> roots = new Stack<Node>();
        public Stack<Node> targets = new Stack<Node>();
        public string values = "{}";
        
        public void SetLabel(string label) {
            type = GetNodeType(label);
        }
        
        public void SetValue(string value) {
            values = value;
        }

        public new string ToString() {
            return $"node {id}\ntype: {type}\nvalues: {values}\n total_roots: {roots.Count}\n total_targets: {targets.Count}";
        }
        
        private NodeType GetNodeType(string label) {
            switch(label){
                case "Start": return NodeType.START; 
                case "Answer": return NodeType.ANSWER; 
                case "Dialog": return NodeType.DIALOG; 
                case "Event": return NodeType.EVENT; 
                case "Options": return NodeType.OPTIONS; 
                case "End": return NodeType.END;
                default:
                    SetValue(label);
                    return NodeType.LABEL;
                    break;
            };
        }
        
    }
}

