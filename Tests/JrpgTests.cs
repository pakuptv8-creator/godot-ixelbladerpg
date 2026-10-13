using System.Collections.Generic;
using NUnit.Framework;
using JrpgGame.Character;
using JrpgGame.Core;
using JrpgGame.Items;
using JrpgGame.Dialogue;
using JrpgGame.Quests;
using JrpgGame.Combat;

namespace JrpgGame.Tests
{
    [TestFixture]
    public class JrpgTests
    {
        [SetUp]
        public void Setup()
        {
            GameManager.Instance.ResetState();
            InventoryManager.Instance.Clear();
            QuestManager.Instance.Clear();
        }

        [Test]
        public void TestCharacterCreationAndStatCalculation()
        {
            var race = RaceData.GetAvailableRaces().Find(r => r.Type == RaceType.Elf)!;
            var trait = TraitData.GetAvailableTraits().Find(t => t.Name == "Скрытный")!;

            var hero = new CharacterData("Леголас", race, new List<TraitData> { trait });

            Assert.That(hero.Name, Is.EqualTo("Леголас"));
            Assert.That(hero.Race.Type, Is.EqualTo(RaceType.Elf));
            Assert.That(hero.GetTotalStats().Agility, Is.EqualTo(10 + 4 + 3)); // 10 Base + 4 Elf + 3 Trait
            Assert.That(hero.GetTotalEvasion(), Is.GreaterThan(0.15f));
            Assert.That(hero.CurrentHp, Is.EqualTo(hero.GetTotalMaxHp()));
        }

        [Test]
        public void TestClassEvolutionAndSubclassAtLevel10()
        {
            var race = RaceData.GetAvailableRaces()[0];
            var hero = new CharacterData("Артур", race, new List<TraitData>());
            hero.Archetype = ClassNames.Warrior;

            Assert.That(hero.Level, Is.EqualTo(1));
            Assert.That(hero.Subclass, Is.Empty);

            // Add EXP to reach Level 10
            // Level 1 -> 2: 100 EXP, 2 -> 3: 200, ..., 9 -> 10: 900. Total = 4500
            ClassEvolution.AddExperience(hero, 4500, out bool leveledUp, out bool unlockedSubclass);

            Assert.That(hero.Level, Is.EqualTo(10));
            Assert.That(unlockedSubclass, Is.True);

            // Select Berserker subclass
            bool selected = ClassEvolution.SelectSubclass(hero, ClassNames.Berserker);
            Assert.That(selected, Is.True);
            Assert.That(hero.Subclass, Is.EqualTo(ClassNames.Berserker));

            var learnable = ClassEvolution.GetLearnableSkillsForCharacter(hero);
            Assert.That(learnable.Exists(s => s.Name == "Ярость Берсерка"), Is.True);
        }

        [Test]
        public void TestInventoryAndShopEconomy()
        {
            var inventory = InventoryManager.Instance;
            var shop = new ShopManager();

            Assert.That(inventory.Gold, Is.EqualTo(100));

            var potion = shop.ShopCatalog.Find(i => i.Id == "potion_hp")!;
            bool bought = shop.BuyItem(inventory, potion);

            Assert.That(bought, Is.True);
            Assert.That(inventory.Gold, Is.EqualTo(80));
            Assert.That(inventory.HasItem("potion_hp"), Is.True);

            bool sold = shop.SellItem(inventory, potion);
            Assert.That(sold, Is.True);
            Assert.That(inventory.Gold, Is.EqualTo(90));
            Assert.That(inventory.HasItem("potion_hp"), Is.False);
        }

        [Test]
        public void TestDialogueAndChoiceSystem()
        {
            var dm = DialogueManager.Instance;

            var root = new DialogueNode("node_1", "Старейшина", "Приветствуем в нашей деревне!");
            root.Options.Add(new DialogueOption("Помочь деревне", "node_2", () => GameManager.Instance.ChangeReputation(10)));
            root.Options.Add(new DialogueOption("Уйти", "", () => GameManager.Instance.ChangeReputation(-5)));

            var node2 = new DialogueNode("node_2", "Старейшина", "Спасибо! Убей гоблина в пещере.");

            dm.RegisterNode(root);
            dm.RegisterNode(node2);

            dm.StartDialogue("node_1");
            Assert.That(dm.CurrentNode?.Id, Is.EqualTo("node_1"));

            dm.SelectOption(0);
            Assert.That(dm.CurrentNode?.Id, Is.EqualTo("node_2"));
            Assert.That(GameManager.Instance.Reputation, Is.EqualTo(10));
        }

        [Test]
        public void TestQuestSystemProgressAndReward()
        {
            var qm = QuestManager.Instance;
            var hero = new CharacterData("Герой", RaceData.GetAvailableRaces()[0], new List<TraitData>());
            GameManager.Instance.Player = hero;

            var quest = new Quest("q1", "Охота на Гоблина", "Убейте 1 гоблина", rewardGold: 50, rewardExp: 100);
            quest.Objectives.Add(new QuestObjective("goblin", "Убить гоблина", ObjectiveType.Kill, 1));
            qm.RegisterQuest(quest);

            qm.StartQuest("q1");
            Assert.That(quest.State, Is.EqualTo(QuestState.Active));

            qm.OnTargetProgress("goblin", ObjectiveType.Kill, 1);
            Assert.That(quest.IsObjectivesComplete, Is.True);

            bool completed = qm.CompleteQuest("q1");
            Assert.That(completed, Is.True);
            Assert.That(quest.State, Is.EqualTo(QuestState.Completed));
            Assert.That(InventoryManager.Instance.Gold, Is.EqualTo(150)); // 100 start + 50 reward
            Assert.That(hero.Level, Is.EqualTo(2)); // 100 EXP triggered level up to Level 2
        }

        [Test]
        public void TestTurnBasedCombatFlowAndActions()
        {
            var hero = new CharacterData("Герой", RaceData.GetAvailableRaces()[0], new List<TraitData>());
            var enemy = new CombatEntity("Гоблин", false, hp: 30, mp: 0, speed: 10, attack: 12, defense: 2);

            var cm = CombatManager.Instance;
            cm.StartCombat(hero, enemy, "goblin", expReward: 100, goldReward: 50);

            Assert.That(cm.TurnOrder.Count, Is.EqualTo(2));
            Assert.That(cm.StateMachine.CurrentState?.Type, Is.EqualTo(CombatStateType.PlayerTurn));

            var playerEntity = cm.Participants.Find(p => p.IsPlayer)!;
            var enemyEntity = cm.Participants.Find(p => !p.IsPlayer)!;

            // Player attacks enemy
            cm.ExecuteBasicAttack(playerEntity, enemyEntity);
            Assert.That(enemyEntity.Hp, Is.LessThan(30));

            // Defend stance
            cm.ExecuteDefend(playerEntity);
            Assert.That(playerEntity.IsDefending, Is.True);
        }
    }
}
