using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogs
{
    public class DialogsHandler
    {
        public static DialogsHandler instance;
        public Dictionary<int, Dictionary<int, Node>> loadedDialogs = new();
    }
}