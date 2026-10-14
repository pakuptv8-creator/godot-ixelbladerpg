export class Quest {
  constructor(id, title, desc, targetId, requiredCount, rewardGold, rewardExp) {
    this.id = id;
    this.title = title;
    this.desc = desc;
    this.targetId = targetId;
    this.requiredCount = requiredCount;
    this.currentCount = 0;
    this.rewardGold = rewardGold;
    this.rewardExp = rewardExp;
    this.state = 'NotStarted'; // NotStarted, Active, Completed
  }

  get isComplete() {
    return this.currentCount >= this.requiredCount;
  }
}

export class QuestManager {
  constructor(game) {
    this.game = game;
    this.quests = [
      new Quest('q_goblin', 'Осквернение Бездны', 'Истребите 2 Тварей Бездны в темных разломах.', 'tentacle', 2, 120, 200)
    ];
  }

  startQuest(id) {
    const q = this.quests.find(q => q.id === id);
    if (q && q.state === 'NotStarted') {
      q.state = 'Active';
      return true;
    }
    return false;
  }

  onKillTarget(targetId) {
    this.quests.forEach(q => {
      if (q.state === 'Active' && q.targetId === targetId) {
        q.currentCount = Math.min(q.requiredCount, q.currentCount + 1);
      }
    });
  }

  claimReward(id) {
    const q = this.quests.find(q => q.id === id);
    if (q && q.state === 'Active' && q.isComplete) {
      q.state = 'Completed';
      this.game.gold += q.rewardGold;
      if (this.game.player) {
        this.game.player.gainExperience(q.rewardExp);
      }
      return true;
    }
    return false;
  }
}
