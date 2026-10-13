using System;
using System.Collections.Generic;
using JrpgGame.Character;
using JrpgGame.Combat;
using JrpgGame.Core;
using JrpgGame.Dialogue;
using JrpgGame.Items;
using JrpgGame.Quests;

namespace JrpgGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("==============================================");
            Console.WriteLine("    ДОБРО ПОЖАЛОВАТЬ В JRPG (Консольная)     ");
            Console.WriteLine("==============================================\n");

            CreateCharacter();
            SetupQuestsAndDialogue();

            bool isRunning = true;
            while (isRunning)
            {
                var player = GameManager.Instance.Player;
                if (player == null || player.CurrentHp <= 0)
                {
                    Console.WriteLine("\n[ИГРА ОКОНЧЕНА] Ваш герой пал в бою.");
                    break;
                }

                Console.WriteLine("\n----------------------------------------------");
                Console.WriteLine($"[ГЕРОЙ: {player.Name} | Уровень: {player.Level} ({player.Archetype} {(string.IsNullOrEmpty(player.Subclass) ? "" : "/ " + player.Subclass)}) | HP: {player.CurrentHp}/{player.GetTotalMaxHp()} | MP: {player.CurrentMp}/{player.GetTotalMaxMp()} | Золото: {InventoryManager.Instance.Gold} | Репутация: {GameManager.Instance.Reputation}]");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("1. Перейти в Таверну (Разговор с NPC)");
                Console.WriteLine("2. Лавка Торговца (Покупка/Продажа)");
                Console.WriteLine("3. Журнал Квестов");
                Console.WriteLine("4. Отправиться в Подземелье (Пошаговый Бой)");
                Console.WriteLine("5. Инвентарь и Навыки");
                Console.WriteLine("6. Выход из игры");
                Console.Write("Выберите действие (1-6): ");

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
                        Console.WriteLine("Спасибо за игру!");
                        break;
                    default:
                        Console.WriteLine("Неверный ввод, попробуйте снова.");
                        break;
                }
            }
        }

        private static void CreateCharacter()
        {
            Console.WriteLine("=== СОЗДАНИЕ ПЕРСОНАЖА ===");
            Console.Write("Введите имя героя: ");
            string name = Console.ReadLine() ?? "Герой";

            Console.WriteLine("\nВыберите расу:");
            var races = RaceData.GetAvailableRaces();
            for (int i = 0; i < races.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {races[i].Name} - {races[i].Description}");
            }
            Console.Write("Выбор расы (1-3): ");
            int raceIdx = 0;
            int.TryParse(Console.ReadLine(), out raceIdx);
            if (raceIdx < 1 || raceIdx > races.Count) raceIdx = 1;
            var selectedRace = races[raceIdx - 1];

            Console.WriteLine("\nВыберите черту (Trait):");
            var traits = TraitData.GetAvailableTraits();
            for (int i = 0; i < traits.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {traits[i].Name} - {traits[i].Description}");
            }
            Console.Write("Выбор черты (1-3): ");
            int traitIdx = 0;
            int.TryParse(Console.ReadLine(), out traitIdx);
            if (traitIdx < 1 || traitIdx > traits.Count) traitIdx = 1;
            var selectedTrait = traits[traitIdx - 1];

            Console.WriteLine("\nВыберите класс:");
            Console.WriteLine("1. Воин");
            Console.WriteLine("2. Маг");
            Console.WriteLine("3. Разбойник");
            Console.Write("Выбор класса (1-3): ");
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
            Console.WriteLine($"\n Персонаж '{player.Name}' ({player.Race.Name}, {player.Archetype}) успешно создан!");
        }

        private static void SetupQuestsAndDialogue()
        {
            // Setup Quest
            var qm = QuestManager.Instance;
            var goblinQuest = new Quest("q_goblin", "Угроза Гоблинов", "Убейте 2 гоблинов в подземелье.", rewardGold: 80, rewardExp: 150);
            goblinQuest.Objectives.Add(new QuestObjective("goblin", "Убить гоблинов", ObjectiveType.Kill, 2));
            qm.RegisterQuest(goblinQuest);

            // Setup Dialogue
            var dm = DialogueManager.Instance;
            var node1 = new DialogueNode("elder_start", "Старейшина", "Здравствуй, путник! Гоблины из подземелья совсем одолели нашу деревню.");
            node1.Options.Add(new DialogueOption("Я помогу вам расправиться с ними! (+Репутация)", "elder_quest", () =>
            {
                GameManager.Instance.ChangeReputation(10);
                QuestManager.Instance.StartQuest("q_goblin");
            }));
            node1.Options.Add(new DialogueOption("Мне это не интересно.", "elder_decline", () =>
            {
                GameManager.Instance.ChangeReputation(-5);
            }));

            var node2 = new DialogueNode("elder_quest", "Старейшина", "Спасибо тебе! Квест 'Угроза Гоблинов' добавлен в твой журнал!");
            var node3 = new DialogueNode("elder_decline", "Старейшина", "Жаль... Надеюсь, ты передумаешь.");

            dm.RegisterNode(node1);
            dm.RegisterNode(node2);
            dm.RegisterNode(node3);
        }

        private static void VisitTavern()
        {
            Console.WriteLine("\n=== ТАВЕРНА И РАЗГОВОР С NPC ===");
            var dm = DialogueManager.Instance;
            dm.StartDialogue("elder_start");

            while (dm.CurrentNode != null)
            {
                var node = dm.CurrentNode;
                Console.WriteLine($"\n[{node.SpeakerName}]: {node.Text}");

                if (node.Options.Count == 0)
                {
                    dm.EndDialogue();
                    break;
                }

                for (int i = 0; i < node.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {node.Options[i].Text}");
                }
                Console.Write("Выберите ответ: ");
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
                Console.WriteLine($"\n=== ЛАВКА ТОРГОВЦА (Ваше золото: {inv.Gold}) ===");
                Console.WriteLine("1. Купить товары");
                Console.WriteLine("2. Продать товары");
                Console.WriteLine("3. Выйти из лавки");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "1")
                {
                    Console.WriteLine("\n-- ТОВАРЫ НА ПРОДАЖУ --");
                    for (int i = 0; i < shop.ShopCatalog.Count; i++)
                    {
                        var item = shop.ShopCatalog[i];
                        Console.WriteLine($"{i + 1}. {item.Name} - {item.Description} (Цена: {item.BuyPrice} зол.)");
                    }
                    Console.Write("Выберите предмет для покупки (0 для отмены): ");
                    int idx = 0;
                    int.TryParse(Console.ReadLine(), out idx);
                    if (idx > 0 && idx <= shop.ShopCatalog.Count)
                    {
                        var target = shop.ShopCatalog[idx - 1];
                        if (shop.BuyItem(inv, target))
                        {
                            Console.WriteLine($"Вы успешно купили '{target.Name}'!");
                        }
                        else
                        {
                            Console.WriteLine("Недостаточно золота!");
                        }
                    }
                }
                else if (choice == "2")
                {
                    Console.WriteLine("\n-- ВАШ ИНВЕНТАРЬ ДЛЯ ПРОДАЖИ --");
                    if (inv.Items.Count == 0)
                    {
                        Console.WriteLine("Ваш инвентарь пуст.");
                        continue;
                    }

                    for (int i = 0; i < inv.Items.Count; i++)
                    {
                        var item = inv.Items[i];
                        Console.WriteLine($"{i + 1}. {item.Name} (Цена продажи: {item.SellPrice} зол.)");
                    }
                    Console.Write("Выберите предмет для продажи (0 для отмены): ");
                    int idx = 0;
                    int.TryParse(Console.ReadLine(), out idx);
                    if (idx > 0 && idx <= inv.Items.Count)
                    {
                        var target = inv.Items[idx - 1];
                        shop.SellItem(inv, target);
                        Console.WriteLine($"Вы продали '{target.Name}'!");
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
            Console.WriteLine("\n=== ЖУРНАЛ КВЕСТОВ ===");
            var qm = QuestManager.Instance;
            if (qm.Quests.Count == 0)
            {
                Console.WriteLine("У вас нет активных или взятых квестов.");
                return;
            }

            foreach (var q in qm.Quests)
            {
                Console.WriteLine($"\nКвест: {q.Title} [{q.State}]");
                Console.WriteLine($"Описание: {q.Description}");
                foreach (var obj in q.Objectives)
                {
                    Console.WriteLine($" - Цель: {obj.Description} ({obj.CurrentCount}/{obj.RequiredCount})");
                }

                if (q.State == QuestState.Active && q.IsObjectivesComplete)
                {
                    Console.WriteLine(" Квест выполнен! Забрать награду? (y/n)");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        qm.CompleteQuest(q.Id);
                        Console.WriteLine($"Награда получена! (+{q.RewardGold} золота, +{q.RewardExp} опыта)");
                    }
                }
            }
        }

        private static void StartDungeonBattle()
        {
            var player = GameManager.Instance.Player;
            if (player == null) return;

            Console.WriteLine("\n=== ПОДЗЕМЕЛЬЕ: ПОШАГОВЫЙ БОЙ ===");
            var enemy = new CombatEntity("Дикий Гоблин", false, hp: 40, mp: 0, speed: 12, attack: 14, defense: 3);
            var cm = CombatManager.Instance;

            cm.OnCombatLog -= Console.WriteLine;
            cm.OnCombatLog += Console.WriteLine;

            cm.StartCombat(player, enemy, monsterId: "goblin", expReward: 100, goldReward: 40);

            while (cm.StateMachine.CurrentState != null &&
                   cm.StateMachine.CurrentState.Type != CombatStateType.Victory &&
                   cm.StateMachine.CurrentState.Type != CombatStateType.Defeat)
            {
                if (cm.StateMachine.CurrentState.Type == CombatStateType.PlayerTurn)
                {
                    var pEntity = cm.Participants.Find(p => p.IsPlayer)!;
                    var eEntity = cm.Participants.Find(p => !p.IsPlayer)!;

                    Console.WriteLine($"\n--- Ваш Ход ---");
                    Console.WriteLine($"Вы: HP {pEntity.Hp}/{pEntity.MaxHp} | MP {pEntity.Mp}/{pEntity.MaxMp}");
                    Console.WriteLine($"Противник: {eEntity.Name} | HP {eEntity.Hp}/{eEntity.MaxHp}");
                    Console.WriteLine("1. Базовая Атака");
                    Console.WriteLine("2. Использовать Навык");
                    Console.WriteLine("3. Защита");
                    Console.WriteLine("4. Побег");
                    Console.Write("Выберите действие: ");

                    string choice = Console.ReadLine() ?? "1";
                    if (choice == "1")
                    {
                        cm.ExecuteBasicAttack(pEntity, eEntity);
                        cm.AdvanceTurn();
                    }
                    else if (choice == "2")
                    {
                        var learnable = ClassEvolution.GetLearnableSkillsForCharacter(player);
                        Console.WriteLine("Доступные навыки:");
                        for (int i = 0; i < learnable.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {learnable[i].Name} (MP: {learnable[i].MpCost}, Урон/Лечение: {learnable[i].Power})");
                        }
                        Console.Write("Выберите навык (0 для отмены): ");
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

            // Subclass unlock notification check at Level 10
            if (player.Level >= 10 && string.IsNullOrEmpty(player.Subclass))
            {
                Console.WriteLine($"\n ВЫ ДОСТИГЛИ 10 УРОВНЯ! Открыт выбор подкласса!");
                var subclasses = ClassEvolution.GetAvailableSubclasses(player.Archetype);
                Console.WriteLine("Выберите Ваш Подкласс:");
                for (int i = 0; i < subclasses.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {subclasses[i]}");
                }
                Console.Write("Выбор подкласса: ");
                int subIdx = 0;
                int.TryParse(Console.ReadLine(), out subIdx);
                if (subIdx > 0 && subIdx <= subclasses.Count)
                {
                    ClassEvolution.SelectSubclass(player, subclasses[subIdx - 1]);
                    Console.WriteLine($"Поздравляем! Ваш подкласс теперь: {player.Subclass}");
                }
            }
        }

        private static void ShowInventoryAndSkills()
        {
            var player = GameManager.Instance.Player;
            if (player == null) return;

            Console.WriteLine("\n=== ИНВЕНТАРЬ И НАВЫКИ ===");
            Console.WriteLine($"Герой: {player.Name} | Класс: {player.Archetype} | Подкласс: {(string.IsNullOrEmpty(player.Subclass) ? "Отсутствует (доступно с 10 уровня)" : player.Subclass)}");
            Console.WriteLine($"Характеристики -> Сила: {player.GetTotalStats().Strength}, Ловкость: {player.GetTotalStats().Agility}, Интеллект: {player.GetTotalStats().Intelligence}, Живучесть: {player.GetTotalStats().Vitality}");

            Console.WriteLine("\nПредметы в инвентаре:");
            var items = InventoryManager.Instance.Items;
            if (items.Count == 0)
            {
                Console.WriteLine(" Инвентарь пуст.");
            }
            else
            {
                foreach (var item in items)
                {
                    Console.WriteLine($" - {item.Name}: {item.Description}");
                }
            }

            Console.WriteLine("\nИзученные и доступные навыки:");
            var skills = ClassEvolution.GetLearnableSkillsForCharacter(player);
            foreach (var s in skills)
            {
                Console.WriteLine($" - {s.Name} (MP: {s.MpCost}): {s.Description}");
            }
        }
    }
}
