# Project Auto RPG — Game Development Document

## 1. Game Overview

**Genre:** Idle RPG with automated, formation-based combat

**Core fantasy:** Build a party of unusual characters, prepare them in a city hub, and send them through increasingly dangerous dungeons where positioning, tags, skills, and equipment determine how far they can progress.

The player makes strategic decisions before and between encounters. Once a dungeon run begins, characters fight automatically, using their equipped skills whenever those skills are available.

## 2. Core Gameplay Loop

1. Assemble a party of up to four characters.
2. Equip characters with compatible skills and equipment.
3. Enter a dungeon and choose a formation.
4. Watch the party automatically fight one enemy group after another.
5. Collect soul shards and crafting resources from defeated enemies.
6. Combine enough soul shards to clone defeated enemies as new characters.
7. Craft skills and equipment, improve the party, and attempt harder content.
8. Defeat dungeon bosses to unlock new hub features and progression systems.

## 3. Combat

Both parties stand in lines, making position an important part of combat. A character’s position can affect which enemies they can target and which skills can reach them. Different enemy formations and skill requirements should encourage the player to experiment with party order and composition.

Combat is automated after preparation. Each character has health, attacks automatically, and uses equipped skills when their cooldowns expire. The player’s main combat decisions are party composition, formation, skill loadout, and equipment.

### Characters

- A party contains up to four characters.
- Characters are obtained by collecting and combining soul shards dropped by their corresponding enemies.
- Every character has a simple default skill, usually a basic attack.
- Characters have tags such as `melee`, `ranged`, `physical`, `magic`, or elemental tags.
- Tags determine which skills a character can use and which equipment they can wear.
- Characters may eventually be merged using their soul shards to create stronger variants.

### Skills

- A character can equip up to four skills, including the default skill.
- Skills are automatically used when their cooldowns expire.
- Skill templates are unlocked by defeating particular enemies enough times.
- Finished skills are created from templates using resources collected in dungeons.
- A skill may require one or more character tags. For example, a skill requiring both `melee` and `physical` can only be used by a character with both tags.
- Skills should vary by range, target selection, damage type, utility, and cooldown.

### Equipment

Equipment improves characters and is crafted from resources dropped by enemies.

Planned equipment categories include:

- Weapons: swords, bows, magic books, and future weapon types.
- Armor: gloves, boots, body armor, helmets, and cloaks.

Equipment has tag requirements. For example, heavy armor can only be worn by characters with the appropriate `heavy` tag. Equipment should complement the character’s tags and skill loadout rather than provide unconditional upgrades only.

## 4. Dungeons and Rewards

Dungeons contain a sequence of enemy encounters followed by a boss or other completion challenge. Each dungeon has its own enemy roster, formations, resource drops, and progression purpose.

Defeated enemies can provide:

- Soul shards for cloning characters.
- Resources used to create skills.
- Resources used to forge equipment.
- Progress toward unlocking enemy-specific skill templates.

Boss victories unlock new features in the hub and act as major progression milestones.

## 5. Hub

The hub is the player’s preparation and progression area. Planned activities include:

- **Forging:** Craft and improve equipment.
- **Resting:** Recover the party between dungeon runs.
- **Research:** Unlock new equipment options and recipes.
- **Unique skill development:** Discover and create special skills.
- **Feature unlocks:** Gain new hub functions by defeating dungeon bosses.

The hub should gradually grow from a small preparation area into the player’s main base of operations.

## 6. Initial MVP

The first playable version should prove the complete core loop with limited content:

- One hub area.
- One short dungeon with several encounters and one boss.
- A party of up to four characters.
- Basic line-based positioning and target restrictions.
- Automated combat with health, damage, cooldowns, victory, and defeat states.
- A small enemy roster with distinct tags or combat roles.
- Soul shard drops and the ability to clone at least one defeated enemy as a character.
- A small set of character tags.
- Default skills and a few craftable or unlockable skills.
- Basic equipment slots with at least one weapon and one armor crafting path.
- One boss reward that unlocks a hub feature.

The MVP should prioritize a satisfying preparation-to-dungeon-to-reward loop over content volume, complex balancing, or visual polish.

## 7. Future Expansion

The following systems are part of the long-term direction but are not required for the MVP:

- Character merging and stronger hybrid variants created by mixing soul shards.
- More dungeons, enemy families, bosses, and environmental themes.
- More tags, complex tag combinations, and specialized builds.
- Additional weapons, armor categories, rarity, and upgrade systems.
- Deeper research trees and more hub facilities.
- Unique skills and rare crafting materials.
- More advanced positioning rules, status effects, and enemy behaviors.
- Idle progression while the player is away from the game.

These systems should extend the existing loop without making the basic party-building and dungeon progression obsolete.

## 8. Design Principles

- **Preparation matters:** Party composition, formation, tags, skills, and equipment should all influence success.
- **Automation remains strategic:** The player configures meaningful decisions even though encounters resolve automatically.
- **Every victory advances the player:** Runs should provide characters, recipes, resources, or knowledge.
- **Content creates possibilities:** New enemies should introduce new party options, not only higher numbers.
- **The system can grow:** New tags, skills, equipment, and dungeons should fit the same clear rules.

## 9. Open Design Areas

The following details are intentionally provisional and can be defined as implementation progresses:

- Exact combat formulas, timing rules, and target-selection priorities.
- The number and shape of positions in a formation.
- Soul shard quantities and cloning costs.
- Skill and equipment rarity, upgrade rules, and resource costs.
- Save behavior and offline-progress rules.
- Final presentation, UI structure, art direction, and audio direction.
