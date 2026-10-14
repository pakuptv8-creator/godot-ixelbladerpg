import test from 'node:test';
import assert from 'node:assert';
import { CharacterData } from '../src/character.js';
import { ShopManager, Item } from '../src/shop.js';
import { QuestManager } from '../src/quests.js';
import { CombatManager } from '../src/combat.js';

test('Character stats calculation and traits', () => {
  const hero = new CharacterData('Герой', 'human', 'stealth', 'Warrior');
  assert.strictEqual(hero.name, 'Герой');
  assert.strictEqual(hero.totalStats.strength, 12); // 10 Base + 2 Human
  assert.strictEqual(hero.totalStats.agility, 15); // 10 Base + 2 Human + 3 Stealth
  assert.strictEqual(hero.getEvasion(), 0.25); // 0.15 + 0.10
});

test('Experience gain and level 10 subclass unlocking', () => {
  const hero = new CharacterData('Артур', 'human', 'blood', 'Warrior');
  assert.strictEqual(hero.level, 1);

  // Gain experience to reach level 10
  // Level 1: 100 EXP, Level 2: 200 EXP, ..., Level 9: 900 EXP => Total = 4500
  let res = hero.gainExperience(4500);
  assert.strictEqual(hero.level, 10);
  assert.strictEqual(res.subclassUnlocked, true);
});

test('Shop buy and sell logic', () => {
  const mockGame = { gold: 100, inventory: [] };
  const shop = new ShopManager(mockGame);

  const bought = shop.buyItem('hp_potion');
  assert.strictEqual(bought, true);
  assert.strictEqual(mockGame.gold, 70);
  assert.strictEqual(mockGame.inventory.length, 1);

  const sold = shop.sellItem(0);
  assert.strictEqual(sold, true);
  assert.strictEqual(mockGame.gold, 85);
  assert.strictEqual(mockGame.inventory.length, 0);
});

test('Quest progress and reward claim', () => {
  const hero = new CharacterData('Герой', 'human', 'blood', 'Warrior');
  const mockGame = { gold: 50, player: hero };
  const qm = new QuestManager(mockGame);

  qm.startQuest('q_goblin');
  qm.onKillTarget('tentacle');
  qm.onKillTarget('tentacle');

  const quest = qm.quests.find(q => q.id === 'q_goblin');
  assert.strictEqual(quest.isComplete, true);

  const claimed = qm.claimReward('q_goblin');
  assert.strictEqual(claimed, true);
  assert.strictEqual(mockGame.gold, 170); // 50 + 120
});

test('Turn-based combat damage and victory flow', () => {
  const hero = new CharacterData('Герой', 'human', 'blood', 'Warrior');
  const mockGame = {
    player: hero,
    gold: 0,
    questManager: { onKillTarget: () => {} }
  };

  const cm = new CombatManager(mockGame);
  cm.startCombat('Тварь Бездны', 20, 10, 2, 100, 50, 'tentacle');

  assert.strictEqual(cm.enemy.hp, 20);

  cm.playerAttack(); // Hero attacks enemy
  assert.strictEqual(cm.enemy.hp < 20, true);
});
