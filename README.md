# Unity Dungeon Crawler

## Unity Version
**Unity 6000.6.6f1**

## About the Game
A 3D top-down isometric dungeon crawler where players explore interconnected rooms, defeat enemies, and solve puzzles to progress through the dungeon.

## Current Progress
The camera system is functional but still under development and subject to further improvements.

The camera currently supports:
- **Small rooms (`RoomS`):** Automatically centers and adjusts its height to fit the room.
- **Large rooms (`RoomB`):** Maintains the camera settings from the previous small room and follows the player when they move outside a defined area, while respecting room boundaries.
- **Entrances:** Detect when the player enters a different room and update the camera accordingly.
- **Transitions:** Smooth camera movement between rooms.

## Testing
The current camera functionality can be tested in:

`Assets/Scenes/TestScene`

**Note:** Player movement, room prefabs, and `TestScene` were created exclusively to test the camera system. They are temporary testing components and should not be considered finalized gameplay features.

## Development Status
The camera system is still being refined. Additional adjustments and improvements may be introduced as development progresses.

## Team Members
- Marta Gabriela Popescu
- Vlad Ogrodnic
- Miruna Alexandra Vișan
- Alexandru Morteanu
- Miruna Bianca Maxim
