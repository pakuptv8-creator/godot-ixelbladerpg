namespace JrpgGame.Quests
{
    public enum ObjectiveType
    {
        Kill,
        Collect
    }

    public class QuestObjective
    {
        public string TargetId { get; set; }
        public string Description { get; set; }
        public ObjectiveType Type { get; set; }
        public int RequiredCount { get; set; }
        public int CurrentCount { get; set; }

        public bool IsCompleted => CurrentCount >= RequiredCount;

        public QuestObjective(string targetId, string description, ObjectiveType type, int requiredCount)
        {
            TargetId = targetId;
            Description = description;
            Type = type;
            RequiredCount = requiredCount;
            CurrentCount = 0;
        }

        public void Progress(int amount = 1)
        {
            CurrentCount += amount;
            if (CurrentCount > RequiredCount)
            {
                CurrentCount = RequiredCount;
            }
        }
    }
}
