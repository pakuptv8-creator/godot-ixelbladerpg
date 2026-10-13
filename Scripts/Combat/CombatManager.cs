using System;
using System.Collections.Generic;
using System.Linq;
using JrpgGame.Character;
using JrpgGame.Core;
using JrpgGame.Items;
using JrpgGame.Quests;

namespace JrpgGame.Combat
{
    public class CombatManager
    {
        private static CombatManager? _instance;
        public static CombatManager Instance => _instance ??= new CombatManager();

        public CombatStateMachine StateMachine { get; private set; } = new CombatStateMachine();

        public List<CombatEntity> Participants { get; private set; } = new List<CombatEntity>();
        public List<CombatEntity> TurnOrder { get; private set; } = new List<CombatEntity>();
        public int TurnIndex { get; set; } = -1;

        public CombatEntity? CurrentActiveEntity => (TurnIndex >= 0 && TurnIndex < TurnOrder.Count) ? TurnOrder[TurnIndex] : null;

        public string EnemyMonsterId { get; set; } = "goblin";
        public int RewardExp { get; set; } = 50;
        public int RewardGold { get; set; } = 30;

        public event Action<string>? OnCombatLog;
        public event Action<CombatStateType>? OnStateChanged;

        public void Log(string message)
        {
            OnCombatLog?.Invoke(message);
        }

        public void StartCombat(CharacterData player, CombatEntity enemy, string monsterId = "goblin", int expReward = 50, int goldReward = 30)
        {
            EnemyMonsterId = monsterId;
            RewardExp = expReward;
            RewardGold = goldReward;

            Participants.Clear();
            var playerEntity = CombatEntity.FromCharacter(player);
            Participants.Add(playerEntity);
            Participants.Add(enemy);

            CalculateTurnOrder();

            StateMachine = new CombatStateMachine();
            StateMachine.RegisterState(new InitState(this));
            StateMachine.RegisterState(new PlayerTurnState(this));
            StateMachine.RegisterState(new EnemyTurnState(this));
            StateMachine.RegisterState(new VictoryState(this));
            StateMachine.RegisterState(new DefeatState(this));

            StateMachine.ChangeState(CombatStateType.Init);
        }

        public void CalculateTurnOrder()
        {
            // Speed initiative calculation
            TurnOrder = Participants.OrderByDescending(p => p.Speed).ToList();
            TurnIndex = -1;
        }

        public void AdvanceTurn()
        {
            var player = Participants.FirstOrDefault(p => p.IsPlayer);
            var enemy = Participants.FirstOrDefault(p => !p.IsPlayer);

            if (enemy == null || !enemy.IsAlive)
            {
                StateMachine.ChangeState(CombatStateType.Victory);
                return;
            }

            if (player == null || !player.IsAlive)
            {
                StateMachine.ChangeState(CombatStateType.Defeat);
                return;
            }

            TurnIndex = (TurnIndex + 1) % TurnOrder.Count;
            var active = CurrentActiveEntity;

            if (active != null)
            {
                active.IsDefending = false; // Reset defense stance on turn start
                if (active.IsPlayer)
                {
                    StateMachine.ChangeState(CombatStateType.PlayerTurn);
                }
                else
                {
                    StateMachine.ChangeState(CombatStateType.EnemyTurn);
                }
            }
        }

        // Combat Actions for Player
        public bool ExecuteBasicAttack(CombatEntity attacker, CombatEntity target)
        {
            Log($"{attacker.Name} атакует {target.Name}!");

            // Evasion check
            Random rng = new Random();
            if (rng.NextDouble() < target.Evasion)
            {
                Log($"{target.Name} уклонился от атаки!");
                return true;
            }

            target.TakeDamage(attacker.Attack);
            Log($"{target.Name} получил урон. Осталось HP: {target.Hp}/{target.MaxHp}");
            return true;
        }

        public bool ExecuteSkill(CombatEntity attacker, CombatEntity target, Skill skill)
        {
            if (!attacker.SpendMp(skill.MpCost))
            {
                Log("Недостаточно маны!");
                return false;
            }

            Log($"{attacker.Name} использует skill '{skill.Name}'!");

            if (skill.Type == SkillType.Damage)
            {
                int totalDmg = attacker.Attack + skill.Power;
                target.TakeDamage(totalDmg);
                Log($"{target.Name} получил {totalDmg} урона от навыка! Осталось HP: {target.Hp}/{target.MaxHp}");
            }
            else if (skill.Type == SkillType.Heal)
            {
                attacker.Heal(skill.Power);
                Log($"{attacker.Name} восстановил {skill.Power} HP! Здоровье: {attacker.Hp}/{attacker.MaxHp}");
            }

            return true;
        }

        public bool ExecuteUseItem(CombatEntity user, Item item)
        {
            if (item.Type != ItemType.Consumable) return false;

            if (item.Id.Contains("hp"))
            {
                user.Heal(item.ValueModifier);
                Log($"{user.Name} использовал {item.Name} и восстановил {item.ValueModifier} HP!");
            }
            else if (item.Id.Contains("mp"))
            {
                user.Mp = Math.Min(user.MaxMp, user.Mp + item.ValueModifier);
                Log($"{user.Name} использовал {item.Name} и восстановил {item.ValueModifier} MP!");
            }

            InventoryManager.Instance.RemoveItem(item);
            return true;
        }

        public void ExecuteDefend(CombatEntity entity)
        {
            entity.IsDefending = true;
            Log($"{entity.Name} встал в защитную стойку!");
        }

        public bool ExecuteFlee()
        {
            Log("Вы успешно сбежали из боя!");
            return true;
        }

        // States
        private class InitState : CombatState
        {
            private readonly CombatManager _mgr;
            public InitState(CombatManager mgr)
            {
                Type = CombatStateType.Init;
                _mgr = mgr;
            }

            public override void Enter()
            {
                _mgr.OnStateChanged?.Invoke(Type);
                _mgr.Log("Бой начался!");
                _mgr.AdvanceTurn();
            }
        }

        private class PlayerTurnState : CombatState
        {
            private readonly CombatManager _mgr;
            public PlayerTurnState(CombatManager mgr)
            {
                Type = CombatStateType.PlayerTurn;
                _mgr = mgr;
            }

            public override void Enter()
            {
                _mgr.OnStateChanged?.Invoke(Type);
                _mgr.Log("Ход игрока!");
            }
        }

        private class EnemyTurnState : CombatState
        {
            private readonly CombatManager _mgr;
            public EnemyTurnState(CombatManager mgr)
            {
                Type = CombatStateType.EnemyTurn;
                _mgr = mgr;
            }

            public override void Enter()
            {
                _mgr.OnStateChanged?.Invoke(Type);
                _mgr.Log("Ход противника!");

                var enemy = _mgr.CurrentActiveEntity;
                var player = _mgr.Participants.FirstOrDefault(p => p.IsPlayer);

                if (enemy != null && player != null && enemy.IsAlive)
                {
                    _mgr.ExecuteBasicAttack(enemy, player);
                }

                _mgr.AdvanceTurn();
            }
        }

        private class VictoryState : CombatState
        {
            private readonly CombatManager _mgr;
            public VictoryState(CombatManager mgr)
            {
                Type = CombatStateType.Victory;
                _mgr = mgr;
            }

            public override void Enter()
            {
                _mgr.OnStateChanged?.Invoke(Type);
                _mgr.Log("Победа!");

                // Sync HP/MP back to player character data
                var playerEntity = _mgr.Participants.FirstOrDefault(p => p.IsPlayer);
                if (playerEntity?.SourceCharacter != null)
                {
                    playerEntity.SourceCharacter.CurrentHp = playerEntity.Hp;
                    playerEntity.SourceCharacter.CurrentMp = playerEntity.Mp;

                    ClassEvolution.AddExperience(playerEntity.SourceCharacter, _mgr.RewardExp, out bool leveled, out bool subclassUnlocked);
                    _mgr.Log($"Получено {_mgr.RewardExp} опыта и {_mgr.RewardGold} золота!");
                    if (leveled) _mgr.Log("Уровень повышен!");
                    if (subclassUnlocked) _mgr.Log("Доступен выбор подкласса!");
                }

                InventoryManager.Instance.Gold += _mgr.RewardGold;
                QuestManager.Instance.OnTargetProgress(_mgr.EnemyMonsterId, ObjectiveType.Kill, 1);
            }
        }

        private class DefeatState : CombatState
        {
            private readonly CombatManager _mgr;
            public DefeatState(CombatManager mgr)
            {
                Type = CombatStateType.Defeat;
                _mgr = mgr;
            }

            public override void Enter()
            {
                _mgr.OnStateChanged?.Invoke(Type);
                _mgr.Log("Поражение! Игра окончена.");

                var playerEntity = _mgr.Participants.FirstOrDefault(p => p.IsPlayer);
                if (playerEntity?.SourceCharacter != null)
                {
                    playerEntity.SourceCharacter.CurrentHp = playerEntity.Hp;
                    playerEntity.SourceCharacter.CurrentMp = playerEntity.Mp;
                }
            }
        }
    }
}
