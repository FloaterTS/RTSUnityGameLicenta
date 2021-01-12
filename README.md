# 3D Strategy Game Final Project
Unity 3D RTS Game
--Work In Progress--
This is a 3D RTS game made in Unity for my final project.

It features Object Oriented Programming written in C#, vector math and AI algorithms in order to create my own version of popular RTS games like Age of Empires/ Starcraft.

All written scripts can be found in Assets/Scripts/ folder.

What is implemented:
- Fully controllable camera, by keys or by mouse, with zoom in and out to a certain degree and fully rotative with a possibility for snap rotations to certain degrees (rotation relative to screen center for better visualization and ease of use). Bounded by map area and height coordinates.

- Unit movement with the help of Unity Navigation Mesh implementation, each unit can move to any valid place on map, using obstacle avoidance, on the shortest path.

- Multiple unit selection with coordinated group movement oriented by current and former mouse position using vector math and quaternion calculations.

- Resource interaction with integrated animations and complete AI route to resource storage camp and back to resource field after resource storage.

- Other helpful implementations like double click multi-selection or partial-selection.

- UI for Main Menu, Pause Menu and Interaction Panel.

- Partially finished map using Unity 3D Terrain Tools.

It is still in progress and the final version will have the following:
- Enemy AI - Base building and coordinated attacking

- More base building options - buildings and upgrades

- Finalized map with level design and resource field placement.

- Complete game design with level start, finish and goal accomplishment.
