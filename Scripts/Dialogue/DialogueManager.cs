using System;
using System.Collections.Generic;

namespace JrpgGame.Dialogue
{
    public class DialogueManager
    {
        private static DialogueManager? _instance;
        public static DialogueManager Instance => _instance ??= new DialogueManager();

        private Dictionary<string, DialogueNode> _dialogues = new Dictionary<string, DialogueNode>();
        public DialogueNode? CurrentNode { get; private set; }

        public event Action<DialogueNode>? OnDialogueStarted;
        public event Action<DialogueNode>? OnDialogueNodeChanged;
        public event Action? OnDialogueEnded;

        public void RegisterNode(DialogueNode node)
        {
            if (node != null && !string.IsNullOrEmpty(node.Id))
            {
                _dialogues[node.Id] = node;
            }
        }

        public bool StartDialogue(string startNodeId)
        {
            if (_dialogues.TryGetValue(startNodeId, out var node))
            {
                CurrentNode = node;
                OnDialogueStarted?.Invoke(node);
                return true;
            }
            return false;
        }

        public bool SelectOption(int optionIndex)
        {
            if (CurrentNode == null || optionIndex < 0 || optionIndex >= CurrentNode.Options.Count)
            {
                return false;
            }

            var option = CurrentNode.Options[optionIndex];
            option.OnSelect?.Invoke();

            if (string.IsNullOrEmpty(option.NextNodeId) || !_dialogues.ContainsKey(option.NextNodeId))
            {
                EndDialogue();
                return true;
            }

            CurrentNode = _dialogues[option.NextNodeId];
            OnDialogueNodeChanged?.Invoke(CurrentNode);
            return true;
        }

        public void EndDialogue()
        {
            CurrentNode = null;
            OnDialogueEnded?.Invoke();
        }
    }
}
