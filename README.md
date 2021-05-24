# Auto-Battle-Rogue-like-Dungeon-crawler
a Auto-Battle game with Rogue-like Dungeon crawler elements
> current total work time: ~230 hours (updated on May 11th 2021)
- current features:
  * A 9*8 hexagon tilemap with a mapping system to guide pawns to designated locations. The field will also show the next opponents for the player, those opponents are being save in a database prefab. This prefab contains the locations, IDs, levels, items, and selected skills of the opponent's pawns. (not fully implemented)
  * A shop system integrates with the pawn's database to generate random pawns with different chances of apparent depending on the player level, preference, etc... (not fully implemented).
  * A bench system where the player store all the pawns they have bought and adds/removes them from the map (have an upper limit of 7). The bench also checks if the player pawns(both on the field and on the bench) are upgradeable and do it automatically according to the rule set beforehand (pawns on the field can't be upgraded during the fight).
  * Pawn's prefab contains stats of each level, selectable skills of each level, ... These prefabs are stored in a database along with these prefabs IDs, the cost to buy, origin,  job, etc... This also applies to the item's prefabs. (not fully implemented)
  * A message pop-up that will show relevant information to the player when they clicked on the pawns on the field.
