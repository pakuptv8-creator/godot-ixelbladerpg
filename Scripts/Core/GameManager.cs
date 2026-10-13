using System.Collections.Generic;
using JrpgGame.Character;

namespace JrpgGame.Core
{
    public class GameManager
    {
        private static GameManager? _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public CharacterData? Player { get; set; }
        public int Reputation { get; set; } = 0;
        public Dictionary<string, bool> StoryFlags { get; set; } = new Dictionary<string, bool>();

        public void SetFlag(string flagName, bool value)
        {
            StoryFlags[flagName] = value;
        }

        public bool GetFlag(string flagName)
        {
            return StoryFlags.TryGetValue(flagName, out bool val) && val;
        }

        public void ChangeReputation(int amount)
        {
            Reputation += amount;
        }

        public void ResetState()
        {
            Player = null;
            Reputation = 0;
            StoryFlags.Clear();
        }
    }
}
