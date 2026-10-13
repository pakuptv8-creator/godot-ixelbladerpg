using System;
using System.Collections.Generic;

namespace JrpgGame.Dialogue
{
    public class DialogueOption
    {
        public string Text { get; set; }
        public string NextNodeId { get; set; }
        public Action? OnSelect { get; set; }

        public DialogueOption(string text, string nextNodeId, Action? onSelect = null)
        {
            Text = text;
            NextNodeId = nextNodeId;
            OnSelect = onSelect;
        }
    }

    public class DialogueNode
    {
        public string Id { get; set; }
        public string SpeakerName { get; set; }
        public string Text { get; set; }
        public List<DialogueOption> Options { get; set; } = new List<DialogueOption>();

        public DialogueNode(string id, string speakerName, string text)
        {
            Id = id;
            SpeakerName = speakerName;
            Text = text;
        }
    }
}
