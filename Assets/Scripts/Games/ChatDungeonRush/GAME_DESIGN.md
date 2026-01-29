# Chat Dungeon Rush - Game Design Document

> A simple, chat-controlled dungeon crawler mini-game for Chaos League streams.
> **Design Philosophy: KISS (Keep It Simple, Stupid)**

---

## Game Overview

**Chat Dungeon Rush** is a lightweight mini-game where the Twitch chat collectively controls a single hero navigating through procedurally generated dungeon rooms. Players type simple commands (up/down/left/right/attack) and the most popular command each turn wins.

### Core Loop
1. Hero spawns in a dungeon room
2. Chat has 3-5 seconds to vote on an action
3. Most popular action is executed
4. Repeat until hero dies or clears the dungeon

---

## Game Mechanics

### Movement System
- **Grid-based movement** (8x8 grid per room)
- Hero moves one tile per turn
- Commands: `!up`, `!down`, `!left`, `!right`, `!wait`

### Combat System
- **Auto-attack**: Hero attacks adjacent enemies automatically
- **Special attack**: `!attack` performs a power attack (longer cooldown)
- Simple HP system for hero and enemies

### Voting System
- Each chat message counts as 1 vote
- Subscribers get 2 votes
- VIPs get 3 votes
- Tie-breaker: Random selection

### Scoring
- +10 points for killing an enemy
- +25 points for collecting treasure
- +100 bonus for clearing a room
- Points distributed to all participants who voted

---

## Room Types

1. **Combat Room** - Enemies to defeat
2. **Treasure Room** - Coins and chests to collect
3. **Trap Room** - Hazards to navigate
4. **Boss Room** - Mini-boss encounter (final room)

---

## Visual Style

- **Resolution**: 16x16 pixels per tile (base sprites)
- **Color Palette**: Limited 16-color palette (NES-style)
- **Grid**: 8x8 tiles visible = 128x128 pixel play area (scaled up for stream)
- **UI**: Simple pixel font, minimal HUD

### Color Palette Reference
```
#0f0f23 - Deep Black (background)
#1a1a2e - Dark Purple (shadows)
#16213e - Navy Blue (dungeon walls)
#4a5568 - Gray (stone)
#718096 - Light Gray (highlights)
#e53e3e - Red (damage/enemies)
#48bb78 - Green (health/healing)
#ecc94b - Gold (treasure/coins)
#f6ad55 - Orange (fire/torches)
#9f7aea - Purple (magic)
#63b3ed - Blue (water/ice)
#fc8181 - Pink (special effects)
#ffffff - White (highlights)
```

---

## COMPLETE ASSET LIST

### 1. HERO SPRITES (16x16 pixels each)

| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `hero_idle_down` | Hero facing down, breathing animation | 2 frames |
| `hero_idle_up` | Hero facing up, breathing animation | 2 frames |
| `hero_idle_left` | Hero facing left, breathing animation | 2 frames |
| `hero_idle_right` | Hero facing right, breathing animation | 2 frames |
| `hero_walk_down` | Hero walking downward | 4 frames |
| `hero_walk_up` | Hero walking upward | 4 frames |
| `hero_walk_left` | Hero walking left | 4 frames |
| `hero_walk_right` | Hero walking right | 4 frames |
| `hero_attack_down` | Hero sword slash facing down | 3 frames |
| `hero_attack_up` | Hero sword slash facing up | 3 frames |
| `hero_attack_left` | Hero sword slash facing left | 3 frames |
| `hero_attack_right` | Hero sword slash facing right | 3 frames |
| `hero_hurt` | Hero taking damage, flash red | 2 frames |
| `hero_death` | Hero death animation | 4 frames |

**Total Hero Frames: 42**

---

### 2. ENEMY SPRITES (16x16 pixels each)

#### Slime Enemy (Basic)
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `slime_idle` | Green blob bouncing in place | 2 frames |
| `slime_move` | Slime hopping movement | 4 frames |
| `slime_attack` | Slime lunging forward | 2 frames |
| `slime_hurt` | Slime squished/flashing | 2 frames |
| `slime_death` | Slime bursting into goo | 4 frames |

#### Skeleton Enemy (Medium)
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `skeleton_idle` | Skeleton standing, slight sway | 2 frames |
| `skeleton_walk` | Skeleton walking | 4 frames |
| `skeleton_attack` | Skeleton sword swing | 3 frames |
| `skeleton_hurt` | Skeleton rattling/flashing | 2 frames |
| `skeleton_death` | Skeleton crumbling to bones | 4 frames |

#### Bat Enemy (Flying)
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `bat_fly` | Bat flapping wings | 4 frames |
| `bat_attack` | Bat swooping down | 3 frames |
| `bat_hurt` | Bat tumbling | 2 frames |
| `bat_death` | Bat falling and poofing | 3 frames |

#### Boss: Giant Eye (Final)
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `boss_eye_idle` | Floating eye, pupil moving | 4 frames |
| `boss_eye_attack` | Eye shooting laser beam | 4 frames |
| `boss_eye_hurt` | Eye shaking/flashing | 3 frames |
| `boss_eye_death` | Eye exploding | 6 frames |

**Total Enemy Frames: 58**

---

### 3. TILE SPRITES (16x16 pixels each)

#### Floor Tiles
| Asset Name | Description |
|------------|-------------|
| `floor_stone_1` | Basic stone floor tile |
| `floor_stone_2` | Stone floor variation 2 (cracked) |
| `floor_stone_3` | Stone floor variation 3 (mossy) |
| `floor_dirt` | Dirt/earth floor tile |
| `floor_water` | Water puddle tile (animated 2 frames) |
| `floor_lava` | Lava tile (animated 4 frames) |
| `floor_grass` | Grass/moss floor tile |
| `floor_highlight` | Highlighted tile for movement preview |

#### Wall Tiles
| Asset Name | Description |
|------------|-------------|
| `wall_stone_top` | Stone wall top edge |
| `wall_stone_mid` | Stone wall middle section |
| `wall_stone_bottom` | Stone wall bottom edge |
| `wall_stone_corner_tl` | Top-left corner |
| `wall_stone_corner_tr` | Top-right corner |
| `wall_stone_corner_bl` | Bottom-left corner |
| `wall_stone_corner_br` | Bottom-right corner |
| `wall_brick_1` | Brick wall variation |
| `wall_brick_2` | Brick wall variation 2 |

#### Door Tiles
| Asset Name | Description |
|------------|-------------|
| `door_closed` | Closed wooden door |
| `door_open` | Open wooden door |
| `door_locked` | Locked door with keyhole |
| `door_boss` | Special boss room door (larger, ominous) |

**Total Tile Assets: 21 (+ 6 animated frames)**

---

### 4. INTERACTIVE OBJECTS (16x16 pixels each)

#### Collectibles
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `coin_gold` | Gold coin spinning | 4 frames |
| `coin_silver` | Silver coin spinning | 4 frames |
| `gem_red` | Red ruby gem sparkling | 2 frames |
| `gem_blue` | Blue sapphire gem sparkling | 2 frames |
| `gem_green` | Green emerald gem sparkling | 2 frames |
| `heart_pickup` | Health heart floating | 2 frames |
| `key_gold` | Golden key for locked doors | 1 frame |

#### Containers
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `chest_closed` | Wooden treasure chest closed | 1 frame |
| `chest_open` | Wooden treasure chest open | 1 frame |
| `chest_mimic` | Mimic chest (enemy!) | 4 frames |
| `barrel` | Wooden barrel (breakable) | 1 frame |
| `barrel_broken` | Broken barrel pieces | 1 frame |
| `crate` | Wooden crate (breakable) | 1 frame |
| `crate_broken` | Broken crate pieces | 1 frame |
| `pot` | Clay pot (breakable) | 1 frame |
| `pot_broken` | Broken pot shards | 1 frame |

#### Environment Props
| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `torch_wall` | Wall-mounted torch | 4 frames |
| `torch_standing` | Standing floor torch | 4 frames |
| `pillar` | Stone pillar decoration | 1 frame |
| `pillar_broken` | Broken/crumbled pillar | 1 frame |
| `bones_pile` | Pile of bones decoration | 1 frame |
| `skull` | Single skull decoration | 1 frame |
| `cobweb_corner` | Corner cobweb | 1 frame |
| `cobweb_large` | Large cobweb | 1 frame |

**Total Object Assets: 25 (+ 29 animated frames)**

---

### 5. TRAP SPRITES (16x16 pixels each)

| Asset Name | Description | Animation Frames |
|------------|-------------|------------------|
| `trap_spikes_up` | Spikes emerged from floor | 1 frame |
| `trap_spikes_down` | Spikes retracted in floor | 1 frame |
| `trap_spikes_activate` | Spikes emerging animation | 4 frames |
| `trap_arrow_hole` | Wall hole that shoots arrows | 1 frame |
| `trap_arrow_projectile` | Arrow flying | 2 frames |
| `trap_pit_open` | Open pit trap | 1 frame |
| `trap_pit_closed` | Hidden pit trap | 1 frame |
| `trap_fire_vent` | Fire vent in floor | 4 frames |

**Total Trap Assets: 8 (+ 7 animated frames)**

---

### 6. UI ELEMENTS

#### HUD Elements
| Asset Name | Description | Size |
|------------|-------------|------|
| `ui_heart_full` | Full health heart | 16x16 |
| `ui_heart_half` | Half health heart | 16x16 |
| `ui_heart_empty` | Empty health heart | 16x16 |
| `ui_coin_icon` | Small coin for score display | 8x8 |
| `ui_key_icon` | Small key for inventory | 8x8 |
| `ui_attack_ready` | Attack ability ready indicator | 16x16 |
| `ui_attack_cooldown` | Attack on cooldown indicator | 16x16 |
| `ui_turn_timer` | Timer bar background | 64x8 |
| `ui_turn_timer_fill` | Timer bar fill (stretchable) | 1x8 |

#### Vote Display
| Asset Name | Description | Size |
|------------|-------------|------|
| `ui_arrow_up` | Up vote indicator | 16x16 |
| `ui_arrow_down` | Down vote indicator | 16x16 |
| `ui_arrow_left` | Left vote indicator | 16x16 |
| `ui_arrow_right` | Right vote indicator | 16x16 |
| `ui_vote_bar_bg` | Vote percentage bar background | 80x12 |
| `ui_vote_bar_fill` | Vote percentage bar fill | 1x10 |
| `ui_command_panel` | Panel showing current votes | 96x48 |

#### Panels & Frames
| Asset Name | Description | Size |
|------------|-------------|------|
| `ui_panel_9slice` | 9-slice panel for dialogs | 24x24 |
| `ui_frame_gold` | Golden decorative frame | 32x32 |
| `ui_banner_victory` | "Victory!" banner | 96x32 |
| `ui_banner_defeat` | "Defeat!" banner | 96x32 |
| `ui_title_card` | Game title graphic | 128x64 |

**Total UI Assets: 21**

---

### 7. VISUAL EFFECTS (VFX)

| Asset Name | Description | Animation Frames | Size |
|------------|-------------|------------------|------|
| `vfx_slash` | Sword slash arc effect | 4 frames | 24x24 |
| `vfx_hit_spark` | Impact spark when hitting enemy | 4 frames | 16x16 |
| `vfx_damage_number` | Floating damage numbers | spritesheet | 8x8 per digit |
| `vfx_heal_sparkle` | Green healing particles | 4 frames | 16x16 |
| `vfx_coin_collect` | Coin pickup sparkle | 4 frames | 16x16 |
| `vfx_poof_small` | Small poof/smoke | 4 frames | 16x16 |
| `vfx_poof_large` | Large poof/smoke | 6 frames | 32x32 |
| `vfx_explosion` | Explosion effect | 6 frames | 32x32 |
| `vfx_laser_beam` | Boss eye laser | 3 frames | 8x48 |
| `vfx_dust_walk` | Small dust when walking | 3 frames | 8x8 |
| `vfx_glow_highlight` | Glowing tile highlight | 2 frames | 16x16 |
| `vfx_portal` | Room exit portal | 4 frames | 32x32 |

**Total VFX Assets: 12 (+ 44 animated frames)**

---

### 8. FONT & TEXT

| Asset Name | Description |
|------------|-------------|
| `font_pixel_8x8` | 8x8 pixel font (full ASCII set) |
| `font_pixel_numbers` | Large numbers for score display (16x16) |
| `text_commands` | Pre-rendered command text (!up, !down, etc.) |

**Total Font Assets: 3**

---

## ASSET SUMMARY

| Category | Static Assets | Animation Frames | Total Sprites |
|----------|---------------|------------------|---------------|
| Hero | 14 | 42 | 42 |
| Enemies | 19 | 58 | 58 |
| Tiles | 21 | 6 | 27 |
| Objects | 25 | 29 | 54 |
| Traps | 8 | 7 | 15 |
| UI | 21 | 0 | 21 |
| VFX | 12 | 44 | 56 |
| Font | 3 | 0 | 3+ |
| **TOTAL** | **123** | **186** | **276+** |

---

## AUDIO ASSETS (Suggested)

### Sound Effects
| Asset Name | Description |
|------------|-------------|
| `sfx_step_stone` | Footstep on stone |
| `sfx_sword_swing` | Sword swing whoosh |
| `sfx_sword_hit` | Sword hitting enemy |
| `sfx_enemy_hurt` | Enemy taking damage |
| `sfx_enemy_death` | Enemy death sound |
| `sfx_hero_hurt` | Hero taking damage |
| `sfx_hero_death` | Hero death sound |
| `sfx_coin_pickup` | Coin collection jingle |
| `sfx_chest_open` | Treasure chest opening |
| `sfx_door_open` | Door opening creak |
| `sfx_trap_trigger` | Trap activation sound |
| `sfx_vote_tick` | Vote registered tick |
| `sfx_turn_start` | Turn beginning chime |
| `sfx_turn_end` | Turn executed sound |
| `sfx_victory` | Victory fanfare |
| `sfx_defeat` | Defeat jingle |

### Music Tracks
| Asset Name | Description |
|------------|-------------|
| `music_dungeon_loop` | Main dungeon exploration music (loopable) |
| `music_boss_loop` | Intense boss battle music (loopable) |
| `music_victory_sting` | Short victory celebration |
| `music_defeat_sting` | Short defeat sound |

---

## SPRITE SHEET ORGANIZATION

All sprites should be organized into the following sprite sheets for optimal Unity importing:

1. `spritesheet_hero.png` - All hero animations (256x128)
2. `spritesheet_enemies.png` - All enemy animations (256x256)
3. `spritesheet_tiles.png` - All floor/wall tiles (128x128)
4. `spritesheet_objects.png` - Props, chests, collectibles (256x128)
5. `spritesheet_traps.png` - Trap sprites (128x64)
6. `spritesheet_ui.png` - UI elements (256x128)
7. `spritesheet_vfx.png` - Visual effects (256x128)
8. `font_pixel.png` - Pixel font spritesheet (128x64)

---

## IMPLEMENTATION NOTES

### Unity Setup
- Import as **Sprite (2D and UI)**
- Pixels Per Unit: **16**
- Filter Mode: **Point (no filter)**
- Compression: **None** (for crisp pixels)

### Scaling for Stream
- Render at native resolution (128x128 for game area)
- Scale up 4x-6x for stream visibility (512x512 to 768x768)
- Use Point filtering to maintain pixel crispness

---

## PHASE 1 MVP (Minimum Viable Product)

For the initial playable version, prioritize these assets:

### Essential (Must Have)
- [ ] Hero idle (1 direction) + walk (4 frames)
- [ ] Slime enemy (idle + death)
- [ ] Basic floor tiles (2 variations)
- [ ] Basic wall tiles (top, mid, bottom)
- [ ] Gold coin (animated)
- [ ] Heart UI icons
- [ ] Vote arrow indicators
- [ ] Hit spark VFX

### Nice to Have (Phase 2)
- [ ] Full hero directional sprites
- [ ] Additional enemies (skeleton, bat)
- [ ] Doors and chests
- [ ] Torches and props
- [ ] Full trap system

### Polish (Phase 3)
- [ ] Boss enemy
- [ ] All VFX
- [ ] UI panels and banners
- [ ] Additional room types

---

*Document Version: 1.0*
*Created for: Chaos League - Chat Dungeon Rush Mini-Game*
