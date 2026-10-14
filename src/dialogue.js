export class DialogueManager {
  constructor(game) {
    this.game = game;
    this.nodes = {};
    this.currentNode = null;
    this.setupNodes();
  }

  setupNodes() {
    this.nodes = {
      elder_start: {
        speaker: 'Жрец Древних',
        text: 'Приветствую тебя, ищущий... Разлом Бездны ширится. Щупальцевые Твари угрожают землям.',
        choices: [
          {
            text: 'Я очищу подземелье от этой скверны! (+15 Репутации)',
            nextNode: 'elder_quest',
            action: () => {
              this.game.reputation += 15;
              this.game.questManager.startQuest('q_goblin');
            }
          },
          {
            text: 'Моя душа не продается. (-10 Репутации)',
            nextNode: 'elder_decline',
            action: () => {
              this.game.reputation -= 10;
            }
          }
        ]
      },
      elder_quest: {
        speaker: 'Жрец Древних',
        text: 'Благословенны твои клинки! Квест "Осквернение Бездны" занесен в твой Гримуар.',
        choices: []
      },
      elder_decline: {
        speaker: 'Жрец Древних',
        text: 'Шепот Бездны всё равно настигнет тебя в глубоких снах...',
        choices: []
      }
    };
  }

  startDialogue(nodeId) {
    if (this.nodes[nodeId]) {
      this.currentNode = this.nodes[nodeId];
      return this.currentNode;
    }
    return null;
  }

  selectChoice(index) {
    if (!this.currentNode || !this.currentNode.choices[index]) return null;
    const choice = this.currentNode.choices[index];
    if (choice.action) choice.action();
    if (choice.nextNode && this.nodes[choice.nextNode]) {
      this.currentNode = this.nodes[choice.nextNode];
    } else {
      this.currentNode = null;
    }
    return this.currentNode;
  }
}
