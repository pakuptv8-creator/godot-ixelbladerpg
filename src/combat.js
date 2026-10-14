export class CombatManager {
  constructor(game) {
    this.game = game;
    this.inCombat = false;
    this.enemy = null;
    this.turn = 'player';
    this.combatLog = [];
  }

  startCombat(monsterName, hp, attack, defense, expReward, goldReward, targetId = 'tentacle') {
    this.inCombat = true;
    this.enemy = {
      name: monsterName,
      hp: hp,
      maxHp: hp,
      attack: attack,
      defense: defense,
      expReward: expReward,
      goldReward: goldReward,
      targetId: targetId
    };
    this.turn = 'player';
    this.combatLog = [`Бой начался! Противник: ${monsterName}`];
  }

  playerAttack() {
    if (!this.inCombat || !this.enemy) return;
    const player = this.game.player;
    const dmg = Math.max(1, player.getAttack() - this.enemy.defense);
    this.enemy.hp = Math.max(0, this.enemy.hp - dmg);
    this.combatLog.push(`Вы атаковали ${this.enemy.name} и нанесли ${dmg} урона!`);

    if (this.enemy.hp <= 0) {
      this.victory();
      return;
    }

    this.enemyTurn();
  }

  playerDefend() {
    if (!this.inCombat || !this.enemy) return;
    this.combatLog.push(`Вы встали в защитную стойку!`);
    this.enemyTurn(true);
  }

  enemyTurn(playerIsDefending = false) {
    if (this.enemy.hp <= 0) return;
    const player = this.game.player;
    const def = playerIsDefending ? player.totalStats.defense * 2 : player.totalStats.defense;
    const dmg = Math.max(1, this.enemy.attack - def);

    player.currentHp = Math.max(0, player.currentHp - dmg);
    this.combatLog.push(`${this.enemy.name} атакует вас и наносит ${dmg} урона!`);

    if (player.currentHp <= 0) {
      this.defeat();
    }
  }

  victory() {
    this.inCombat = false;
    this.combatLog.push(`ПОБЕДА! Получено ${this.enemy.expReward} EXP и ${this.enemy.goldReward} Золота!`);
    this.game.gold += this.enemy.goldReward;

    if (this.game.player) {
      const res = this.game.player.gainExperience(this.enemy.expReward);
      if (res.leveledUp) this.combatLog.push(`Уровень повышен! Уровень: ${this.game.player.level}`);
      if (res.subclassUnlocked) this.combatLog.push(`Открыта эволюция класса (10 уровень)!`);
    }

    this.game.questManager.onKillTarget(this.enemy.targetId);
  }

  defeat() {
    this.inCombat = false;
    this.combatLog.push(`ПОРАЖЕНИЕ! Вы пали пред ликом Бездны.`);
  }
}
