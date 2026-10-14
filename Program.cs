using System;
using System.Collections.Generic;
using JrpgGame.Character;
using JrpgGame.Combat;
using JrpgGame.Core;
using JrpgGame.Dialogue;
using JrpgGame.Items;
using JrpgGame.Quests;
using JrpgGame.UI;

namespace JrpgGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();
            ConsoleRenderer.PrintEldritchBanner();
            Console.WriteLine($"\n{ConsoleRenderer.Purple}Welcome to the Dark Eldritch Fantasy JRPG (Console Edition){ConsoleRenderer.Reset}\n");

            CreateCharacter();
            SetupQuestsAndDialogue();

            bool isRunning = true;
            while (isRunning)
            {
                var player = GameManager.Instance.Player;
                if (player == null || player.CurrentHp <= 0)
                {
                    Console.WriteLine($"\n{ConsoleRenderer.DarkRed}===================================================={ConsoleRenderer.Reset}");
                    Console.WriteLine($"{ConsoleRenderer.Red}   [ВАША ДУША ПОГЛОЩЕНА БЕЗДНОЙ - GAME OVER]   {ConsoleRenderer.Reset}");
                    Console.WriteLine($"{ConsoleRenderer.DarkRed}===================================================={ConsoleRenderer.Reset}");
                    break;
                }

                Console.WriteLine();
                ConsoleRenderer.DrawHeader($"ГЕРОЙ: {player.Name} | {player.Archetype} {(string.IsNullOrEmpty(player.Subclass) ? "" : "• " + player.Subclass)}");

                string hpBar = ConsoleRenderer.GetProgressBar(player.CurrentHp, player.GetTotalMaxHp(), 15);
                string mpBar = ConsoleRenderer.GetProgressBar(player.CurrentMp, player.GetTotalMaxMp(), 15);

                Console.WriteLine($" {ConsoleRenderer.Red}HP [{hpBar}] {player.CurrentHp}/{player.GetTotalMaxHp()}{ConsoleRenderer.Reset}  │  {ConsoleRenderer.Blue}MP [{mpBar}] {player.CurrentMp}/{player.GetTotalMaxMp()}{ConsoleRenderer.Reset}");
                Console.WriteLine($" {ConsoleRenderer.Gold}Золото: {InventoryManager.Instance.Gold}{ConsoleRenderer.Reset} │ {ConsoleRenderer.Purple}Репутация / Сакральность: {GameManager.Instance.Reputation}{ConsoleRenderer.Reset} │ {ConsoleRenderer.Cyan}Уровень: {player.Level}{ConsoleRenderer.Reset}");
                Console.WriteLine($"{ConsoleRenderer.DarkGray}─────────────────────────────────────────────────────────────────────────────{ConsoleRenderer.Reset}");

                Console.WriteLine($" 1. {ConsoleRenderer.Yellow}Окультный Трактир / Разговор с Культистом{ConsoleRenderer.Reset}");
                Console.WriteLine($" 2. {ConsoleRenderer.Gold}Теневая Лавка Торговца{ConsoleRenderer.Reset}");
                Console.WriteLine($" 3. {ConsoleRenderer.Cyan}Фолиант Задач и Гримуар Квестов{ConsoleRenderer.Reset}");
                Console.WriteLine($" 4. {ConsoleRenderer.Red}Спуститься в Оскверненное Подземелье (Пошаговый Бой){ConsoleRenderer.Reset}");
                Console.WriteLine($" 5. {ConsoleRenderer.Magenta}Инвентарь и Древо Заклинаний{ConsoleRenderer.Reset}");
                Console.WriteLine($" 6. {ConsoleRenderer.Gray}Покинуть этот мир (Выход){ConsoleRenderer.Reset}");
                Console.Write($"\n{ConsoleRenderer.Purple}Выберите путь (1-6): {ConsoleRenderer.Reset}");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        VisitTavern();
                        break;
                    case "2":
                        VisitShop();
                        break;
                    case "3":
                        ShowQuestJournal();
                        break;
                    case "4":
                        StartDungeonBattle();
                        break;
                    case "5":
                        ShowInventoryAndSkills();
                        break;
                    case "6":
                        isRunning = false;
                        Console.WriteLine($"\n{ConsoleRenderer.DarkPurple}Мрак смыкает свои объятия... До новых встреч.{ConsoleRenderer.Reset}");
                        break;
                    default:
                        Console.WriteLine("Неверное знамение, попробуйте снова.");
                        break;
                }
            }
        }

        private static void CreateCharacter()
        {
            ConsoleRenderer.DrawHeader("СОЗДАНИЕ ИЗБРАННОГО");
            Console.Write($"{ConsoleRenderer.Cyan}Введите имя смертного: {ConsoleRenderer.Reset}");
            string name = Console.ReadLine() ?? "Путник";

            Console.WriteLine($"\n{ConsoleRenderer.Purple}Выберите рождение и происхождение:{ConsoleRenderer.Reset}");
            var races = RaceData.GetAvailableRaces();
            for (int i = 0; i < races.Count; i++)
            {
                Console.WriteLine($" {ConsoleRenderer.Gold}{i + 1}.{ConsoleRenderer.Reset} {races[i].Name} - {races[i].Description}");
            }
            Console.Write($"\n{ConsoleRenderer.Cyan}Выбор расы (1-3): {ConsoleRenderer.Reset}");
            int raceIdx = 0;
            int.TryParse(Console.ReadLine(), out raceIdx);
            if (raceIdx < 1 || raceIdx > races.Count) raceIdx = 1;
            var selectedRace = races[raceIdx - 1];

            Console.WriteLine($"\n{ConsoleRenderer.Purple}Выберите дремлющую черту (Trait):{ConsoleRenderer.Reset}");
            var traits = TraitData.GetAvailableTraits();
            for (int i = 0; i < traits.Count; i++)
            {
                Console.WriteLine($" {ConsoleRenderer.Gold}{i + 1}.{ConsoleRenderer.Reset} {traits[i].Name} - {traits[i].Description}");
            }
            Console.Write($"\n{ConsoleRenderer.Cyan}Выбор черты (1-3): {ConsoleRenderer.Reset}");
            int traitIdx = 0;
            int.TryParse(Console.ReadLine(), out traitIdx);
            if (traitIdx < 1 || traitIdx > traits.Count) traitIdx = 1;
            var selectedTrait = traits[traitIdx - 1];

            Console.WriteLine($"\n{ConsoleRenderer.Purple}Выберите Путь Искусства:{ConsoleRenderer.Reset}");
            Console.WriteLine($" {ConsoleRenderer.Red}1. Воин Крови (Warrior){ConsoleRenderer.Reset}");
            Console.WriteLine($" {ConsoleRenderer.Blue}2. Маг Бездны (Mage){ConsoleRenderer.Reset}");
            Console.WriteLine($" {ConsoleRenderer.Green}3. Теневой Убийца (Rogue){ConsoleRenderer.Reset}");
            Console.Write($"\n{ConsoleRenderer.Cyan}Выбор класса (1-3): {ConsoleRenderer.Reset}");
            string classChoice = Console.ReadLine() ?? "1";
            string archetype = classChoice switch
            {
                "2" => ClassNames.Mage,
                "3" => ClassNames.Rogue,
                _ => ClassNames.Warrior
            };

            var player = new CharacterData(name, selectedRace, new List<TraitData> { selectedTrait })
            {
                Archetype = archetype
            };

            GameManager.Instance.Player = player;
            Console.WriteLine($"\n{ConsoleRenderer.Green}Персонаж '{player.Name}' ({player.Race.Name}, {player.Archetype}) успешно ступил на земли Мрака!{ConsoleRenderer.Reset}");
        }

        private static void SetupQuestsAndDialogue()
        {
            var qm = QuestManager.Instance;
            var goblinQuest = new Quest("q_goblin", "Осквернение Бездны", "Истребите 2 Щупальцевых Тварей в темных подземельях.", rewardGold: 120, rewardExp: 200);
            goblinQuest.Objectives.Add(new QuestObjective("tentacle", "Убить Тварей Бездны", ObjectiveType.Kill, 2));
            qm.RegisterQuest(goblinQuest);

            var dm = DialogueManager.Instance;
            var node1 = new DialogueNode("elder_start", "Жрец Древних", "Приветствую тебя, ищущий... Из глубин подземного разлома пробудились древние Щупальцевые Твари.");
            node1.Options.Add(new DialogueOption("Я очищу подземелье от этой скверны! (+Репутация)", "elder_quest", () =>
            {
                GameManager.Instance.ChangeReputation(15);
                QuestManager.Instance.StartQuest("q_goblin");
            }));
            node1.Options.Add(new DialogueOption("Моя душа не продается. (Отвергнуть)", "elder_decline", () =>
            {
                GameManager.Instance.ChangeReputation(-10);
            }));

            var node2 = new DialogueNode("elder_quest", "Жрец Древних", "Благословенны твои клинки! Квест 'Осквернение Бездны' занесен в твой Гримуар.");
            var node3 = new DialogueNode("elder_decline", "Жрец Древних", "Шепот Бездны всё равно настигнет тебя...");

            dm.RegisterNode(node1);
            dm.RegisterNode(node2);
            dm.RegisterNode(node3);
        }

        private static void VisitTavern()
        {
            ConsoleRenderer.DrawHeader("ОКУЛЬТНЫЙ ТРАКТИР 'ПЕПЕЛ И КРОВЬ'");
            var dm = DialogueManager.Instance;
            dm.StartDialogue("elder_start");

            while (dm.CurrentNode != null)
            {
                var node = dm.CurrentNode;
                Console.WriteLine($"\n{ConsoleRenderer.Purple}[{node.SpeakerName}]:{ConsoleRenderer.Reset} {node.Text}");

                if (node.Options.Count == 0)
                {
                    dm.EndDialogue();
                    break;
                }

                Console.WriteLine();
                for (int i = 0; i < node.Options.Count; i++)
                {
                    Console.WriteLine($" {ConsoleRenderer.Gold}{i + 1}.{ConsoleRenderer.Reset} {node.Options[i].Text}");
                }
                Console.Write($"\n{ConsoleRenderer.Cyan}Выберите слово: {ConsoleRenderer.Reset}");
                int option = 0;
                int.TryParse(Console.ReadLine(), out option);
                dm.SelectOption(option - 1);
            }
        }

        private static void VisitShop()
        {
            var shop = new ShopManager();
            var inv = InventoryManager.Instance;

            bool inShop = true;
            while (inShop)
            {
                ConsoleRenderer.DrawHeader($"ТЕНЕВАЯ ЛАВКА (Золото: {inv.Gold})");
                Console.WriteLine($" 1. {ConsoleRenderer.Gold}Купить артефакты и зелья{ConsoleRenderer.Reset}");
                Console.WriteLine($" 2. {ConsoleRenderer.Green}Продать трофеи{ConsoleRenderer.Reset}");
                Console.WriteLine($" 3. {ConsoleRenderer.Gray}Покинуть лавку{ConsoleRenderer.Reset}");
                Console.Write($"\n{ConsoleRenderer.Cyan}Выберите действие: {ConsoleRenderer.Reset}");

                string choice = Console.ReadLine() ?? "";
                if (choice == "1")
                {
                    Console.WriteLine($"\n{ConsoleRenderer.Gold}-- ТОВАРЫ НА ПРОДАЖУ --{ConsoleRenderer.Reset}");
                    for (int i = 0; i < shop.ShopCatalog.Count; i++)
                    {
                        var item = shop.ShopCatalog[i];
                        Console.WriteLine($" {i + 1}. {item.Name} - {item.Description} (Цена: {item.BuyPrice} зол.)");
                    }
                    Console.Write($"\n{ConsoleRenderer.Cyan}Купить предмет (0 для отмены): {ConsoleRenderer.Reset}");
                    int idx = 0;
                    int.TryParse(Console.ReadLine(), out idx);
                    if (idx > 0 && idx <= shop.ShopCatalog.Count)
                    {
                        var target = shop.ShopCatalog[idx - 1];
                        if (shop.BuyItem(inv, target))
                        {
                            Console.WriteLine($"\n{ConsoleRenderer.Green}Вы приобрели '{target.Name}'!{ConsoleRenderer.Reset}");
                        }
                        else
                        {
                            Console.WriteLine($"\n{ConsoleRenderer.Red}Недостаточно золота!{ConsoleRenderer.Reset}");
                        }
                    }
                }
                else if (choice == "2")
                {
                    Console.WriteLine($"\n{ConsoleRenderer.Green}-- ВАШИ ПРЕДМЕТЫ --{ConsoleRenderer.Reset}");
                    if (inv.Items.Count == 0)
                    {
                        Console.WriteLine("Ваш инвентарь пуст.");
                        continue;
                    }

                    for (int i = 0; i < inv.Items.Count; i++)
                    {
                        var item = inv.Items[i];
                        Console.WriteLine($" {i + 1}. {item.Name} (Выручка: {item.SellPrice} зол.)");
                    }
                    Console.Write($"\n{ConsoleRenderer.Cyan}Продать предмет (0 для отмены): {ConsoleRenderer.Reset}");
                    int idx = 0;
                    int.TryParse(Console.ReadLine(), out idx);
                    if (idx > 0 && idx <= inv.Items.Count)
                    {
                        var target = inv.Items[idx - 1];
                        shop.SellItem(inv, target);
                        Console.WriteLine($"\n{ConsoleRenderer.Gold}Вы продали '{target.Name}'!{ConsoleRenderer.Reset}");
                    }
                }
                else
                {
                    inShop = false;
                }
            }
        }

        private static void ShowQuestJournal()
        {
            ConsoleRenderer.DrawHeader("ГРИМУАР КВЕСТОВ И ЗАДАЧ");
            var qm = QuestManager.Instance;
            if (qm.Quests.Count == 0)
            {
                Console.WriteLine("Гримуар пуст. Вы еще не приняли предначертанных испытаний.");
                return;
            }

            foreach (var q in qm.Quests)
            {
                Console.WriteLine($"\n{ConsoleRenderer.Purple}Квест:{ConsoleRenderer.Reset} {q.Title} [{q.State}]");
                Console.WriteLine($"Описание: {q.Description}");
                foreach (var obj in q.Objectives)
                {
                    string progress = ConsoleRenderer.GetProgressBar(obj.CurrentCount, obj.RequiredCount, 10);
                    Console.WriteLine($" - Цель: {obj.Description} [{progress}] ({obj.CurrentCount}/{obj.RequiredCount})");
                }

                if (q.State == QuestState.Active && q.IsObjectivesComplete)
                {
                    Console.Write($"\n{ConsoleRenderer.Gold}Квест выполнен! Получить награду? (y/n): {ConsoleRenderer.Reset}");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        qm.CompleteQuest(q.Id);
                        Console.WriteLine($"\n{ConsoleRenderer.Green}Награда получена! (+{q.RewardGold} золота, +{q.RewardExp} опыта!){ConsoleRenderer.Reset}");
                    }
                }
            }
        }

        private static void StartDungeonBattle()
        {
            var player = GameManager.Instance.Player;
            if (player == null) return;

            ConsoleRenderer.DrawHeader("ОСКВЕРНЕННОЕ ПОДЗЕМЕЛЬЕ - БОЙ");
            Console.WriteLine(ConsoleRenderer.GetMonsterSprite("tentacle"));

            var enemy = new CombatEntity("Щупальцевая Тварь Бездны", false, hp: 45, maxHp: 45, mp: 0, maxMp: 0, speed: 12, attack: 15, defense: 3);
            var cm = CombatManager.Instance;

            cm.OnCombatLog -= LogCombat;
            cm.OnCombatLog += LogCombat;

            cm.StartCombat(player, enemy, monsterId: "tentacle", expReward: 120, goldReward: 50);

            while (cm.StateMachine.CurrentState != null &&
                   cm.StateMachine.CurrentState.Type != CombatStateType.Victory &&
                   cm.StateMachine.CurrentState.Type != CombatStateType.Defeat)
            {
                if (cm.StateMachine.CurrentState.Type == CombatStateType.PlayerTurn)
                {
                    var pEntity = cm.Participants.Find(p => p.IsPlayer)!;
                    var eEntity = cm.Participants.Find(p => !p.IsPlayer)!;

                    string pHpBar = ConsoleRenderer.GetProgressBar(pEntity.Hp, pEntity.MaxHp, 10);
                    string eHpBar = ConsoleRenderer.GetProgressBar(eEntity.Hp, eEntity.MaxHp, 10);

                    Console.WriteLine($"\n{ConsoleRenderer.DarkPurple}────────────── СТАТУС СРАЖЕНИЯ ──────────────{ConsoleRenderer.Reset}");
                    Console.WriteLine($" {ConsoleRenderer.Cyan}{pEntity.Name}:{ConsoleRenderer.Reset} HP [{pHpBar}] {pEntity.Hp}/{pEntity.MaxHp} | MP {pEntity.Mp}/{pEntity.MaxMp}");
                    Console.WriteLine($" {ConsoleRenderer.Red}{eEntity.Name}:{ConsoleRenderer.Reset} HP [{eHpBar}] {eEntity.Hp}/{eEntity.MaxHp}");
                    Console.WriteLine($"{ConsoleRenderer.DarkPurple}────────────────────────────────────────────{ConsoleRenderer.Reset}");

                    Console.WriteLine($" 1. {ConsoleRenderer.Red}Удар Клинком{ConsoleRenderer.Reset}");
                    Console.WriteLine($" 2. {ConsoleRenderer.Blue}Использовать Заклинание{ConsoleRenderer.Reset}");
                    Console.WriteLine($" 3. {ConsoleRenderer.Yellow}Защитная Стойка{ConsoleRenderer.Reset}");
                    Console.WriteLine($" 4. {ConsoleRenderer.Gray}Побег в Тень{ConsoleRenderer.Reset}");
                    Console.Write($"\n{ConsoleRenderer.Cyan}Ваше решение (1-4): {ConsoleRenderer.Reset}");

                    string choice = Console.ReadLine() ?? "1";
                    if (choice == "1")
                    {
                        cm.ExecuteBasicAttack(pEntity, eEntity);
                        cm.AdvanceTurn();
                    }
                    else if (choice == "2")
                    {
                        var learnable = ClassEvolution.GetLearnableSkillsForCharacter(player);
                        Console.WriteLine($"\n{ConsoleRenderer.Blue}Гримуар Заклинаний:{ConsoleRenderer.Reset}");
                        for (int i = 0; i < learnable.Count; i++)
                        {
                            Console.WriteLine($" {i + 1}. {learnable[i].Name} (MP: {learnable[i].MpCost}, Сила: {learnable[i].Power})");
                        }
                        Console.Write($"\n{ConsoleRenderer.Cyan}Выберите заклинание (0 для отмены): {ConsoleRenderer.Reset}");
                        int sIdx = 0;
                        int.TryParse(Console.ReadLine(), out sIdx);
                        if (sIdx > 0 && sIdx <= learnable.Count)
                        {
                            if (cm.ExecuteSkill(pEntity, eEntity, learnable[sIdx - 1]))
                            {
                                cm.AdvanceTurn();
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        cm.ExecuteDefend(pEntity);
                        cm.AdvanceTurn();
                    }
                    else if (choice == "4")
                    {
                        cm.ExecuteFlee();
                        break;
                    }
                }
            }

            if (player.Level >= 10 && string.IsNullOrEmpty(player.Subclass))
            {
                Console.WriteLine($"\n{ConsoleRenderer.Gold} ВЫ ДОСТИГЛИ 10 УРОВНЯ! Пробудился истинный Подкласс!{ConsoleRenderer.Reset}");
                var subclasses = ClassEvolution.GetAvailableSubclasses(player.Archetype);
                Console.WriteLine("Выберите Ваш Высший Подкласс:");
                for (int i = 0; i < subclasses.Count; i++)
                {
                    Console.WriteLine($" {i + 1}. {subclasses[i]}");
                }
                Console.Write($"\n{ConsoleRenderer.Cyan}Выбор подкласса: {ConsoleRenderer.Reset}");
                int subIdx = 0;
                int.TryParse(Console.ReadLine(), out subIdx);
                if (subIdx > 0 && subIdx <= subclasses.Count)
                {
                    ClassEvolution.SelectSubclass(player, subclasses[subIdx - 1]);
                    Console.WriteLine($"\n{ConsoleRenderer.Purple}Поздравляем! Ваш подкласс теперь: {player.Subclass}{ConsoleRenderer.Reset}");
                }
            }
        }

        private static void LogCombat(string msg)
        {
            Console.WriteLine($"  {ConsoleRenderer.Gray}» {msg}{ConsoleRenderer.Reset}");
        }

        private static void ShowInventoryAndSkills()
        {
            var player = GameManager.Instance.Player;
            if (player == null) return;

            ConsoleRenderer.DrawHeader("ИНВЕНТАРЬ И ГРИМУАР НАВЫКОВ");
            Console.WriteLine($"Избранный: {player.Name} | Класс: {player.Archetype} | Подкласс: {(string.IsNullOrEmpty(player.Subclass) ? "Дремлет (с 10 уровня)" : player.Subclass)}");
            Console.WriteLine($"Характеристики -> Сила: {player.GetTotalStats().Strength}, Ловкость: {player.GetTotalStats().Agility}, Интеллект: {player.GetTotalStats().Intelligence}, Живучесть: {player.GetTotalStats().Vitality}");

            Console.WriteLine($"\n{ConsoleRenderer.Gold}Предметы в сумке:{ConsoleRenderer.Reset}");
            var items = InventoryManager.Instance.Items;
            if (items.Count == 0)
            {
                Console.WriteLine(" Сумка пуста.");
            }
            else
            {
                foreach (var item in items)
                {
                    Console.WriteLine($" - {item.Name}: {item.Description}");
                }
            }

            Console.WriteLine($"\n{ConsoleRenderer.Purple}Изученные заклинания и приемы:{ConsoleRenderer.Reset}");
            var skills = ClassEvolution.GetLearnableSkillsForCharacter(player);
            foreach (var s in skills)
            {
                Console.WriteLine($" - {s.Name} (MP: {s.MpCost}): {s.Description}");
            }
        }
    }
}
