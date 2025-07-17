# Project: Orcaball


*A dynamic physics-based sports game where you play as a lone orca and must play football against a swarming pod of dolphins.*

## Gameplay Video
[![Watch gameplay demo on YouTube](http://img.youtube.com/vi/8aKeqWgA7sY/0.jpg)](https://youtu.be/8aKeqWgA7sY)

## The AI: The Brains of the Dolphin Opponents
### 1. Behavioral State Machine

The opponents operate on a Finite State Machine, allowing them to dynamically switch between behaviors based on the game's context.

- **`Attacking`**: Actively pursues the ball to score.
- **`Navigating`**: Intelligently repositions to get into an optimal attacking position.
- **`Defending`**: A specialized state for defender-type AI to guard a territory.

### 2. Strategic Positioning & Nuanced Pathfinding

A key feature is the AI's ability to navigate *around* the ball to get behind it, rather than colliding with it from the wrong direction. This is achieved by calculating an offset target point, creating a smooth, arcing path that demonstrates tactical awareness.

### 3. Predictive Targeting

The AI doesn't just chase the ball's last known position; it anticipates its movement. By analyzing the ball's velocity, the AI aims for where the ball *will be*, allowing it to intercept passes and block shots effectively. This creates a much more challenging and realistic opponent.

### 4. Hierarchical Roles & Specialization

- **The Defender**: This AI stays within a defined `localeRadius` of its starting position, acting like a goalie. It moves laterally to block the ball's path, only leaving its post if the ball is far out of its zone. This demonstrates a team-based intelligence even in this early stage.

### 5. Dynamic Movement

The AI uses a boost mechanic with a cooldown to mimic the effect of cetaceans flapping their tails, giving boosts of speed.

---

## Gameplay Concept

The player controls the powerful orca. The objective is to use the orca's strength and agility to maintain possession of the ball and score against the opponents. The primary challenge comes from the coordinated and intelligent pod of dolphins who will swarm, block, and attack.

## Current Status

This project is currently in the **prototype stage**. The core mechanics and the advanced AI framework are the primary focus.

## Technology Stack

- **Engine**: Unity 6
- **Language**: C#

## Future Plans

- [ ] Ball camera that keeps ball and player in shot at all times, as in Rocket League.
- [ ] Refined player controls.
- [ ] Sound design and visual effects.
