import { CharacterData } from './character.js';
import { DialogueManager } from './dialogue.js';
import { ShopManager } from './shop.js';
import { QuestManager } from './quests.js';
import { CombatManager } from './combat.js';

class Game {
  constructor() {
    this.player = null;
    this.gold = 100;
    this.reputation = 0;
    this.inventory = [];

    this.dialogueManager = new DialogueManager(this);
    this.shopManager = new ShopManager(this);
    this.questManager = new QuestManager(this);
    this.combatManager = new CombatManager(this);

    this.selectedClass = 'Warrior';
    this.initUI();
  }

  initUI() {
    // Class button selection
    document.querySelectorAll('.class-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        document.querySelectorAll('.class-btn').forEach(b => b.classList.remove('selected'));
        e.target.classList.add('selected');
        this.selectedClass = e.target.dataset.class;
        this.updateCreationPreview();
      });
    });

    // Inputs update preview
    document.getElementById('input-hero-name')?.addEventListener('input', () => this.updateCreationPreview());
    document.getElementById('select-race')?.addEventListener('change', () => this.updateCreationPreview());
    document.getElementById('select-trait')?.addEventListener('change', () => this.updateCreationPreview());

    // Start Game
    document.getElementById('btn-start-game')?.addEventListener('click', () => this.startGame());

    // Nav Tabs
    document.querySelectorAll('.nav-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const tab = e.target.dataset.tab;
        this.switchTab(tab);
      });
    });

    // Combat buttons
    document.getElementById('btn-atk')?.addEventListener('click', () => {
      this.combatManager.playerAttack();
      this.updateCombatView();
      this.updateHUD();
    });

    document.getElementById('btn-defend')?.addEventListener('click', () => {
      this.combatManager.playerDefend();
      this.updateCombatView();
      this.updateHUD();
    });

    this.updateCreationPreview();
  }

  updateCreationPreview() {
    const name = document.getElementById('input-hero-name')?.value || 'Герой';
    const race = document.getElementById('select-race')?.value || 'human';
    const trait = document.getElementById('select-trait')?.value || 'stealth';

    const previewHero = new CharacterData(name, race, trait, this.selectedClass);

    document.getElementById('prev-str').innerText = previewHero.totalStats.strength;
    document.getElementById('prev-agi').innerText = previewHero.totalStats.agility;
    document.getElementById('prev-int').innerText = previewHero.totalStats.intelligence;
    document.getElementById('prev-vit').innerText = previewHero.totalStats.vitality;
    document.getElementById('prev-hp').innerText = previewHero.totalStats.maxHp;
    document.getElementById('prev-mp').innerText = previewHero.totalStats.maxMp;
    document.getElementById('prev-atk').innerText = previewHero.getAttack();
    document.getElementById('prev-evasion').innerText = Math.round(previewHero.getEvasion() * 100) + '%';
  }

  startGame() {
    const name = document.getElementById('input-hero-name')?.value || 'Герой';
    const race = document.getElementById('select-race')?.value || 'human';
    const trait = document.getElementById('select-trait')?.value || 'stealth';

    this.player = new CharacterData(name, race, trait, this.selectedClass);

    document.getElementById('view-creation').classList.remove('active-view');
    document.getElementById('main-hud').classList.remove('hidden');
    document.getElementById('main-nav').classList.remove('hidden');

    this.switchTab('tab-world');
    this.updateHUD();
  }

  switchTab(tabId) {
    document.querySelectorAll('.view-panel').forEach(p => p.classList.remove('active-view'));
    document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));

    const target = document.getElementById(tabId);
    if (target) target.classList.add('active-view');

    const navBtn = document.querySelector(`.nav-btn[data-tab="${tabId}"]`);
    if (navBtn) navBtn.classList.add('active');

    if (tabId === 'tab-dialogue') this.renderDialogue();
    if (tabId === 'tab-shop') this.renderShop();
    if (tabId === 'tab-quests') this.renderQuests();
    if (tabId === 'tab-character') this.renderCharacterSheet();
    if (tabId === 'tab-combat') this.initCombat();

    this.updateHUD();
  }

  updateHUD() {
    if (!this.player) return;
    document.getElementById('hero-name-title').innerText = this.player.name;
    document.getElementById('hero-class-badge').innerText = `${this.player.archetype} ${this.player.subclass ? '• ' + this.player.subclass : ''}`;
    document.getElementById('hud-level').innerText = this.player.level;
    document.getElementById('hud-exp').innerText = this.player.experience;
    document.getElementById('hud-next-exp').innerText = this.player.level * 100;

    const hpPercent = (this.player.currentHp / this.player.totalStats.maxHp) * 100;
    const mpPercent = (this.player.currentMp / this.player.totalStats.maxMp) * 100;

    document.getElementById('hp-fill').style.width = hpPercent + '%';
    document.getElementById('mp-fill').style.width = mpPercent + '%';

    document.getElementById('hp-text').innerText = `${this.player.currentHp} / ${this.player.totalStats.maxHp}`;
    document.getElementById('mp-text').innerText = `${this.player.currentMp} / ${this.player.totalStats.maxMp}`;

    document.getElementById('hud-gold').innerText = this.gold;
    document.getElementById('hud-reputation').innerText = this.reputation;
  }

  renderDialogue() {
    const node = this.dialogueManager.startDialogue('elder_start');
    if (!node) return;

    document.getElementById('npc-name').innerText = node.speaker;
    document.getElementById('dialogue-text').innerText = node.text;

    const choicesContainer = document.getElementById('dialogue-choices');
    choicesContainer.innerHTML = '';

    node.choices.forEach((choice, idx) => {
      const btn = document.createElement('button');
      btn.className = 'choice-btn';
      btn.innerText = `${idx + 1}. ${choice.text}`;
      btn.addEventListener('click', () => {
        const next = this.dialogueManager.selectChoice(idx);
        if (next) {
          document.getElementById('dialogue-text').innerText = next.text;
          choicesContainer.innerHTML = '';
        } else {
          document.getElementById('dialogue-text').innerText = 'Разговор завершен.';
          choicesContainer.innerHTML = '';
        }
        this.updateHUD();
      });
      choicesContainer.appendChild(btn);
    });
  }

  renderShop() {
    const shopList = document.getElementById('shop-items-list');
    shopList.innerHTML = '';

    this.shopManager.catalog.forEach(item => {
      const row = document.createElement('div');
      row.className = 'item-row';
      row.innerHTML = `
        <div>
          <strong>${item.name}</strong> - ${item.desc}
        </div>
        <button class="gothic-button" style="width:auto; padding:4px 8px;">Купить (${item.buyPrice} зол.)</button>
      `;
      row.querySelector('button').addEventListener('click', () => {
        if (this.shopManager.buyItem(item.id)) {
          this.renderShop();
          this.updateHUD();
        } else {
          alert('Недостаточно золота!');
        }
      });
      shopList.appendChild(row);
    });

    const playerList = document.getElementById('shop-player-items-list');
    playerList.innerHTML = '';

    this.inventory.forEach((item, idx) => {
      const row = document.createElement('div');
      row.className = 'item-row';
      row.innerHTML = `
        <div>
          <strong>${item.name}</strong>
        </div>
        <button class="gothic-button" style="width:auto; padding:4px 8px;">Продать (${item.sellPrice} зол.)</button>
      `;
      row.querySelector('button').addEventListener('click', () => {
        this.shopManager.sellItem(idx);
        this.renderShop();
        this.updateHUD();
      });
      playerList.appendChild(row);
    });
  }

  renderQuests() {
    const list = document.getElementById('quests-list');
    list.innerHTML = '';

    this.questManager.quests.forEach(q => {
      const card = document.createElement('div');
      card.className = 'quest-card';
      card.innerHTML = `
        <h3>${q.title} [${q.state}]</h3>
        <p>${q.desc}</p>
        <p>Прогресс: ${q.currentCount} / ${q.requiredCount}</p>
        ${q.state === 'Active' && q.isComplete ? `<button class="gothic-button claim-btn">Забрать Награду (+${q.rewardGold} Золота, +${q.rewardExp} EXP)</button>` : ''}
      `;

      card.querySelector('.claim-btn')?.addEventListener('click', () => {
        this.questManager.claimReward(q.id);
        this.renderQuests();
        this.updateHUD();
      });

      list.appendChild(card);
    });
  }

  renderCharacterSheet() {
    const p = this.player;
    if (!p) return;

    document.getElementById('char-full-stats').innerHTML = `
      <p>Имя: <strong>${p.name}</strong></p>
      <p>Класс: <strong>${p.archetype} ${p.subclass ? '• ' + p.subclass : ''}</strong></p>
      <p>Уровень: <strong>${p.level}</strong></p>
      <hr style="margin:10px 0; border-color:var(--border-purple);">
      <p>Сила: ${p.totalStats.strength}</p>
      <p>Ловкость: ${p.totalStats.agility}</p>
      <p>Интеллект: ${p.totalStats.intelligence}</p>
      <p>Живучесть: ${p.totalStats.vitality}</p>
      <p>Атака: ${p.getAttack()}</p>
      <p>Уклонение: ${Math.round(p.getEvasion() * 100)}%</p>
    `;

    const subBox = document.getElementById('subclass-selection-container');
    if (p.level >= 10 && !p.subclass) {
      subBox.classList.remove('hidden');
      const btnsBox = document.getElementById('subclass-buttons');
      btnsBox.innerHTML = '';

      const options = p.archetype === 'Warrior' ? ['Берсерк', 'Паладин'] :
                      p.archetype === 'Mage' ? ['Пиромант', 'Некромант'] : ['Тень', 'Ассасин'];

      options.forEach(sub => {
        const btn = document.createElement('button');
        btn.className = 'gothic-button';
        btn.innerText = sub;
        btn.addEventListener('click', () => {
          p.subclass = sub;
          subBox.classList.add('hidden');
          this.renderCharacterSheet();
          this.updateHUD();
        });
        btnsBox.appendChild(btn);
      });
    } else {
      subBox.classList.add('hidden');
    }
  }

  initCombat() {
    if (!this.combatManager.inCombat) {
      this.combatManager.startCombat('Щупальцевая Тварь Бездны', 50, 14, 3, 120, 50, 'tentacle');
    }
    this.updateCombatView();
  }

  updateCombatView() {
    const cm = this.combatManager;
    document.getElementById('combat-log').innerHTML = cm.combatLog.slice(-3).join('<br>');

    if (cm.enemy) {
      document.getElementById('combat-enemy-name').innerText = cm.enemy.name;
      const enemyHpPct = (cm.enemy.hp / cm.enemy.maxHp) * 100;
      document.getElementById('combat-enemy-hp').style.width = enemyHpPct + '%';
      document.getElementById('combat-enemy-hp-val').innerText = `${cm.enemy.hp}/${cm.enemy.maxHp}`;
    }

    if (this.player) {
      document.getElementById('combat-player-name').innerText = this.player.name;
      const playerHpPct = (this.player.currentHp / this.player.totalStats.maxHp) * 100;
      document.getElementById('combat-player-hp').style.width = playerHpPct + '%';
      document.getElementById('combat-player-hp-val').innerText = `${this.player.currentHp}/${this.player.totalStats.maxHp}`;
    }

    this.drawCombatCanvas();
  }

  drawCombatCanvas() {
    const canvas = document.getElementById('combat-canvas');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw background
    ctx.fillStyle = '#08060a';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Draw Player sprite
    ctx.fillStyle = '#8e44ad';
    ctx.fillRect(100, 160, 50, 80);
    ctx.fillStyle = '#fff';
    ctx.font = '14px Cinzel';
    ctx.fillText('🗡️ Герой', 90, 260);

    // Draw Enemy sprite
    ctx.fillStyle = '#c0392b';
    ctx.beginPath();
    ctx.arc(650, 180, 45, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = '#fff';
    ctx.fillText('👁️ Тварь Бездны', 600, 260);
  }
}

window.addEventListener('DOMContentLoaded', () => {
  window.game = new Game();
});
