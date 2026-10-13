using System.Collections.Generic;
using System.Linq;
using JrpgGame.Character;
using JrpgGame.Core;
using JrpgGame.Items;

namespace JrpgGame.Quests
{
    public class QuestManager
    {
        private static QuestManager? _instance;
        public static QuestManager Instance => _instance ??= new QuestManager();

        private readonly List<Quest> _quests = new List<Quest>();

        public IReadOnlyList<Quest> Quests => _quests.AsReadOnly();

        public void RegisterQuest(Quest quest)
        {
            if (quest != null && !_quests.Any(q => q.Id == quest.Id))
            {
                _quests.Add(quest);
            }
        }

        public bool StartQuest(string questId)
        {
            var quest = _quests.FirstOrDefault(q => q.Id == questId);
            if (quest != null && quest.State == QuestState.NotStarted)
            {
                quest.State = QuestState.Active;
                return true;
            }
            return false;
        }

        public void OnTargetProgress(string targetId, ObjectiveType type, int amount = 1)
        {
            foreach (var quest in _quests.Where(q => q.State == QuestState.Active))
            {
                foreach (var obj in quest.Objectives)
                {
                    if (obj.TargetId == targetId && obj.Type == type && !obj.IsCompleted)
                    {
                        obj.Progress(amount);
                    }
                }
            }
        }

        public bool CompleteQuest(string questId)
        {
            var quest = _quests.FirstOrDefault(q => q.Id == questId);
            if (quest != null && quest.State == QuestState.Active && quest.IsObjectivesComplete)
            {
                quest.State = QuestState.Completed;

                // Grant rewards
                InventoryManager.Instance.Gold += quest.RewardGold;

                foreach (var item in quest.RewardItems)
                {
                    InventoryManager.Instance.AddItem(item);
                }

                if (GameManager.Instance.Player != null)
                {
                    ClassEvolution.AddExperience(GameManager.Instance.Player, quest.RewardExp, out _, out _);
                }

                return true;
            }
            return false;
        }

        public void Clear()
        {
            _quests.Clear();
        }
    }
}
