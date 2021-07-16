# Match-3-Linker
Quick match 3 linker game. 
Works on mobile. 

### Made with Unity 2021.1.7f1

### Game Description
The board is represented as a grid, and could be of various sizes. The size of the grid is determined by the ScriptableObject LevelData.

There are 5 different colors of tiles, and there's an extra one (the black one), which is an obstacle. The obstacle falls down and behaves like a tile, but it cannot be merged. 

Tiles can be liked veritically, diagonally and horizontally. 

Three or more tiles make a match. The match is made when the cursor (or touch) is lifted. 

When tiles are matched, they are removed from the board. The board pieces then fall down on the empty spots, and new tiles arrive from the top. 

The game is over when X points are made (it's 3,500 points for now, but can be changed inside the inspector).

When either the Replay or Play Again buttons are pressed, the score is erased and a new match begins. 

#### Game View

Here's a screenshot of the game view. 
It was originally made with only Unity built-in tools. However, this branch has been updated to use some packages, like TextMeshPro. 

![GameView](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/GameView.JPG)

And a GIF with gameplay:

![GameplayView](https://github.com/rsofia/Match-3-Linker/blob/master/ReadMeContent/Match3Gameplay.gif)

Easy to create and modify levels.
-------------
Using ScriptableObjects, you can have a board of any size by simply modifying the GridSize variable for rows and columns. 
You can also insert the type of tile in the array, which for the moment can be either Playable or Obstacle (the obstacle is like a tile, 
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
