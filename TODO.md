# Project Auto RPG — Development TODO

This list turns the MVP in [GAME_DESIGN.md](GAME_DESIGN.md) into small, reviewable implementation steps. Items should generally be completed in order.

## Phase 1 — Project Foundation

- [ ] Create the initial main scene and a minimal hub screen.
- [ ] Add a simple navigation flow between the hub and a placeholder dungeon screen.
- [ ] Establish the initial project folder structure for scenes, scripts, data, and UI.
- [ ] Add a lightweight game state model for the party, inventory, resources, and current progression.
- [ ] Decide and document the first provisional combat constants and data conventions.

## Phase 2 — Combat Prototype

- [ ] Define data for characters, enemies, tags, skills, and formations.
- [ ] Build a test combat scene with two opposing lines and visible combatants.
- [ ] Implement health, basic attacks, damage, defeat, and encounter completion.
- [ ] Implement automatic combat timing and a readable combat log or status display.
- [ ] Add target restrictions based on position and basic attack range.
- [ ] Allow the player to configure a party of up to four characters before combat.
- [ ] Add a simple dungeon sequence containing multiple encounters and a final boss.

## Phase 3 — Skills and Character Collection

- [ ] Add cooldown-based automatic skill usage.
- [ ] Add default skills and a small set of additional skill templates.
- [ ] Enforce tag requirements when selecting or equipping skills.
- [ ] Add soul shard drops from defeated enemies.
- [ ] Add a collection screen that combines enough shards to clone a character.
- [ ] Make at least one defeated enemy obtainable as a playable character.

## Phase 4 — Equipment and Hub Progression

- [ ] Add equipment slots for one weapon and one armor item.
- [ ] Add equipment tag requirements and validation when equipping items.
- [ ] Add resource drops and a minimal forging flow in the hub.
- [ ] Add a rest action that restores the party between dungeon runs.
- [ ] Add a boss reward that unlocks one new hub feature.

## Phase 5 — MVP Review and Expansion Hooks

- [ ] Play through the complete loop: prepare, enter, fight, collect rewards, improve, and retry.
- [ ] Add basic feedback for victories, defeats, rewards, cooldowns, and invalid equipment or skill choices.
- [ ] Verify that the core systems are data-driven enough to add enemies, skills, tags, and equipment without rewriting combat.
- [ ] Record balance observations and unresolved design decisions in GAME_DESIGN.md.
- [ ] Mark the MVP complete only after a clean end-to-end playtest.

## Future Ideas

- [ ] Character merging and stronger hybrid variants.
- [ ] Additional dungeons, enemy families, bosses, and formations.
- [ ] More tags, skill interactions, equipment types, and crafting recipes.
- [ ] Research trees, unique skills, and additional hub facilities.
- [ ] Status effects and more advanced enemy behavior.
- [ ] Offline idle progression and save/load systems.
