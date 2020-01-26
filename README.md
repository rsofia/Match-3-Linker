# Match-3-Linker
Quick match 3 linker game. 
Works on mobile. 

### Made with Unity 2019.2.17f

### Game View

Here's a screenshot of the game view. 
Everything is made with Unity built-in tools. No plugins were used.

![GameView](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/GameView.JPG)

Easy to create and modify levels.
-------------
Using ScriptableObjects, you can have a board of any size by simply modifying the GridSize variable for rows and columns. 
You can also insert the type of tile in the array, which for the moment can be either Playabler or Obstacle (the obstacle is like a tile, 
but you can't merge it).
Each level can also have different types of tiles, and a different obstacle. Just assign the prefab to the corresponding slot. 

![LevelEditor with ScriptableObjects](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/LevelEditor.JPG)

Prefabs
-------------

Take advantage of prefab variants and inheritance to create your own tiles. 
To create new tile behaviours, just be sure to inherit from TileBase.

![Prefab variants](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/Variants.JPG)

UI
-------------

Modify the UI animations with animation curves. 

![UI](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/Animations.JPG)
