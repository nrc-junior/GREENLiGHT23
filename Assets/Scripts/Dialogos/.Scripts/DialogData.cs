using UnityEngine;

namespace Dialogs
{
    
    public class DialogData {
        public DialogData(){}

    }

    public class Actor {
        public int actorId = -1;
        public int humorId = -1;

        public Actor(int arg0, int arg1) {
            actorId = arg0;
            humorId = arg1;
        }
    }

    public class Event {
        public string callback = "none";
        
        public Event(string arg0) {
            callback = arg0;
        }
    }
}
