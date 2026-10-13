namespace JrpgGame.Combat
{
    public enum CombatStateType
    {
        Init,
        PlayerTurn,
        EnemyTurn,
        Victory,
        Defeat
    }

    public abstract class CombatState
    {
        public CombatStateType Type { get; protected set; }
        public CombatStateMachine Machine { get; set; } = null!;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
