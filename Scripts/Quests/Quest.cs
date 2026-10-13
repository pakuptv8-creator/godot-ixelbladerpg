using System.Collections.Generic;
using System.Linq;
using JrpgGame.Items;

namespace JrpgGame.Quests
{
    public enum QuestState
    {
        NotStarted,
        Active,
        Completed,
        Failed
    }

    public class Quest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestState State { get; set; } = QuestState.NotStarted;

        public List<QuestObjective> Objectives { get; set; } = new List<QuestObjective>();

        public int RewardGold { get; set; }
        public int RewardExp { get; set; }
        public List<Item> RewardItems { get; set; } = new List<Item>();

        public bool IsObjectivesComplete => Objectives.All(o => o.IsCompleted);

        public Quest(string id, string title, string description, int rewardGold = 50, int rewardExp = 100)
        {
            Id = id;
            Title = title;
            Description = description;
            RewardGold = rewardGold;
            RewardExp = rewardExp;
        }
    }
}
