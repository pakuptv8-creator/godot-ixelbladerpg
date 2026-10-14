using System;
using System.Text;

namespace JrpgGame.UI
{
    public static class ConsoleRenderer
    {
        // ANSI Escape Colors
        public static string Reset = "\u001b[0m";
        public static string Bold = "\u001b[1m";
        public static string Red = "\u001b[31m";
        public static string Green = "\u001b[32m";
        public static string Yellow = "\u001b[33m";
        public static string Blue = "\u001b[34m";
        public static string Magenta = "\u001b[35m";
        public static string Cyan = "\u001b[36m";
        public static string DarkRed = "\u001b[38;5;88m";
        public static string Purple = "\u001b[38;5;93m";
        public static string DarkPurple = "\u001b[38;5;54m";
        public static string Gold = "\u001b[38;5;214m";
        public static string Gray = "\u001b[38;5;242m";
        public static string DarkGray = "\u001b[38;5;236m";

        public static void DrawHeader(string title)
        {
            Console.WriteLine($"{DarkPurple}╔═════════════════════════════════════════════════════════════════════════════╗{Reset}");
            Console.WriteLine($"{DarkPurple}║{Reset} {Bold}{Purple}{title.PadLeft((75 + title.Length) / 2).PadRight(75)}{Reset} {DarkPurple}║{Reset}");
            Console.WriteLine($"{DarkPurple}╚═════════════════════════════════════════════════════════════════════════════╝{Reset}");
        }

        public static void DrawBox(string content, string color = "")
        {
            if (string.IsNullOrEmpty(color)) color = Gray;
            string[] lines = content.Split('\n');
            int width = 75;

            Console.WriteLine($"{color}┌{new string('─', width)}┐{Reset}");
            foreach (var line in lines)
            {
                string truncated = line.Length > width - 2 ? line.Substring(0, width - 2) : line;
                Console.WriteLine($"{color}│{Reset} {truncated.PadRight(width - 2)} {color}│{Reset}");
            }
            Console.WriteLine($"{color}└{new string('─', width)}┘{Reset}");
        }

        public static string GetProgressBar(int current, int max, int length = 20, string fillChar = "█", string emptyChar = "░")
        {
            if (max <= 0) max = 1;
            float percentage = Math.Clamp((float)current / max, 0f, 1f);
            int filled = (int)Math.Round(percentage * length);
            int empty = length - filled;

            return $"{new string(fillChar[0], filled)}{new string(emptyChar[0], empty)}";
        }

        public static void PrintEldritchBanner()
        {
            Console.WriteLine($@"{DarkRed}
       .---.          .---.
      /     \  __    /     \      ___     _                   _
     / /  \  \(  )  / /  \  \    / __|_ _| |_ |_  _ _| |_  _ _| |
    |  |  |  | ||  |  |  |  |  | (__| || |  _| ' \| || | ' \| |
    |  |  |  | ||  |  |  |  |   \___|\_,_|\__|_||_|\_,_|_||_|_|
     \ \__/  / ||   \ \__/  /
      \_____/  ''    \_____/   {Purple}~ CHRONICLES OF THE ELDRITCH VOID ~{Reset}
            {DarkPurple}(/  \)`--'(/  \){Reset}");
        }

        public static string GetMonsterSprite(string monsterType)
        {
            return monsterType.ToLower() switch
            {
                "tentacle" or "goblin" => $@"{DarkPurple}
                  ,---.
                 /     \
               /  (o)(o) \
              (      <    )
               \   '---' /   (;;;)
             (;;;)  ||||    (;;;;;)
            (;;;;;) ||||   (;;;;;)
            {Reset}",

                "cultist" => $@"{DarkRed}
                 .---.
                /  .  \
               /  / \  \
              |  | O |  |
              |  |---|  |
             /  /  |  \  \
            |__|___|___|__|
            {Reset}",

                "abomination" => $@"{Red}
               /\_/\
             =( °w° )=
               )   (   //  (;;;)
              (___.__) //  (;;;;;)
             /||||||||\
            {Reset}",

                _ => $@"{Gray}
                (??_??)
                {Reset}"
            };
        }
    }
}
