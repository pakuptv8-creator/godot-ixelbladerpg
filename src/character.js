export class Stats {
  constructor(str = 10, agi = 10, intVal = 10, vit = 10) {
    this.strength = str;
    this.agility = agi;
    this.intelligence = intVal;
    this.vitality = vit;
  }

  get maxHp() { return this.vitality * 10 + 20; }
  get maxMp() { return this.intelligence * 8 + 10; }
  get speed() { return this.agility * 2; }
  get physicalAttack() { return this.strength * 2 + 5; }
  get magicAttack() { return this.intelligence * 2 + 5; }
  get defense() { return this.vitality + Math.floor(this.strength / 2); }
  get evasion() { return this.agility * 0.01; }

  add(otherStats) {
    return new Stats(
      this.strength + (otherStats.strength || 0),
      this.agility + (otherStats.agility || 0),
      this.intelligence + (otherStats.intelligence || 0),
      this.vitality + (otherStats.vitality || 0)
    );
  }
}

export const RACES = {
  human: { name: 'Человек Древних Семлян', stats: new Stats(2, 2, 2, 2) },
  elf: { name: 'Лунно-Лесной Эльф', stats: new Stats(0, 4, 4, 0) },
  dwarf: { name: 'Подземный Дварф', stats: new Stats(4, 0, 0, 4) }
};

export const TRAITS = {
  stealth: { name: 'Скрытный', stats: new Stats(0, 3, 0, 0), evasionBonus: 0.10, attackPenalty: 2 },
  blood: { name: 'Кровь Древних', stats: new Stats(4, 0, 0, 3), evasionBonus: 0, attackPenalty: 0 },
  gnosis: { name: 'Окультист', stats: new Stats(0, 0, 5, 1), evasionBonus: 0, attackPenalty: 0 }
};

export class CharacterData {
  constructor(name = 'Герой', raceKey = 'human', traitKey = 'stealth', archetype = 'Warrior') {
    this.name = name || 'Герой';
    this.raceKey = raceKey;
    this.traitKey = traitKey;
    this.archetype = archetype;
    this.subclass = '';

    this.level = 1;
    this.experience = 0;

    this.baseStats = new Stats(10, 10, 10, 10);

    this.totalStats = this.calculateTotalStats();
    this.currentHp = this.totalStats.maxHp;
    this.currentMp = this.totalStats.maxMp;
  }

  calculateTotalStats() {
    let total = new Stats(10, 10, 10, 10);
    const race = RACES[this.raceKey] || RACES.human;
    const trait = TRAITS[this.traitKey] || TRAITS.stealth;

    total = total.add(race.stats);
    total = total.add(trait.stats);
    return total;
  }

  getEvasion() {
    const trait = TRAITS[this.traitKey];
    return this.totalStats.evasion + (trait ? trait.evasionBonus : 0);
  }

  getAttack() {
    const trait = TRAITS[this.traitKey];
    const penalty = trait ? trait.attackPenalty : 0;
    return Math.max(1, this.totalStats.physicalAttack - penalty);
  }

  gainExperience(amount) {
    this.experience += amount;
    let req = this.level * 100;
    let leveledUp = false;
    let subclassUnlocked = false;

    while (this.experience >= req) {
      this.experience -= req;
      this.level++;
      leveledUp = true;
      this.baseStats.strength += 2;
      this.baseStats.agility += 2;
      this.baseStats.intelligence += 2;
      this.baseStats.vitality += 2;
      this.totalStats = this.calculateTotalStats();
      this.currentHp = this.totalStats.maxHp;
      this.currentMp = this.totalStats.maxMp;

      if (this.level >= 10 && !this.subclass) {
        subclassUnlocked = true;
      }
      req = this.level * 100;
    }

    return { leveledUp, subclassUnlocked };
  }
}
