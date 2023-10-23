# tavli
Tavli games include portes (backgammon), plakoto, fevga, and asso-dio. 
This console application demonstrates functional programming in C#. 
The application uses a command-line interface.

# How to Play
## Chose a Game
```
TAVLI 2.1 Copyright © Pawel Welsberg 2023

Choose one of (Portes, Plakoto, Fevga, AssoDio)
GameType>
```
Enter one of the four names listed in the parentheses and press ENTER.
## Chose the Type of Black Player
```
Choose black player type (Human, Computer)
BlackPlayerType>
```
Enter `Human` if you wish to control this player's moves.
## Chose the Type of White Player
The same principles apply as for the black player.
## Game Board
The screen will display the current state of the game.
## Roll for Order
You'll be prompted to roll dice to determine the order of play:
```
Rolling players: White, Black
Roll>
```
Both players roll one die simultaneously. Enter two digits without spaces representing the rolls of the white and black players. For example, entering `16` means the white player rolled a 1 and the black player rolled a 6. Alternatively, enter `r` for a random roll. The player with the higher number goes first. If there's a tie, players roll again.
## Player Rolls Dice to Move 
A player will roll dice to move their checkers:
```
Rolling players: White
Roll>
```
Provide two digits representing the result of each die. For instance, `23` means a roll of 2 and 3. Again, you can enter `r` for a random roll. Note that the prompt for rolling appears for both human and computer players. 
## Player Chooses Move
If the player is of the `Human` type, they can choose their move from the available options. 
For example:
```
 11  10   9   8   7   6       5   4   3   2   1   0
╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗
║ ☻ │   │   │   │ ☺ │   ║   ║ ☺ │   │   │   │   │ ☻ ║   ║
║ ☻ │   │   │   │ ☺ │   ║   ║ ☺ │   │   │   │   │ ☻ ║   ║
║ ☻ │   │   │   │ ☺ │   ║   ║ ☺ │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║ ☺ │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║ ☺ │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
╠═══╬═══╬═══╬═══╬═══╬═══╬BAR╬═══╬═══╬═══╬═══╬═══╬═══╬OFF╣
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║ ☺ │   │   │   │   │   ║   ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │   │   ║   ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │   │ ☻ │   │ ☺ ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │   │ ☻ │   │ ☺ ║   ║
╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝
 12  13  14  15  16  17      18  19  20  21  22  23

Rolling players: Black
Roll>55
Rolled:(5,5)
Possible plays:
1 - 4 moves: Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 12 to pos 7 with no beating
2 - 4 moves: Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating
3 - 4 moves: Moves 5 from pos 7 to pos 2 with no beating, Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating
4 - 4 moves: Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating, Moves 5 from pos 12 to pos 7 with no beating
Play>
```
The player rolled two 5s, which entitles them to move four times. All possible moves are listed. The player should enter their choice, which in this case ranges from `1` to `4`.
## Computer chooses move
The computer selects its move automatically, as shown below. However, the user still controls the rolling:
```
Rolling players: White
Roll>r
Rolled:(6,3)
Playing:
2 moves: Moves 6 from pos 11 to pos 17 with beating, Moves 3 from pos 17 to pos 20 with no beating
State:PlayerMovedOrTriedOrBearedOffOrBoarded Player:White White player pips:158 Black player pips:163
 11  10   9   8   7   6       5   4   3   2   1   0
╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗
║ ☻ │   │   │   │ ☺ │   ║   ║ ☺ │   │   │   │ ☺ │ ☻ ║   ║
║ ☻ │   │   │   │ ☺ │   ║   ║ ☺ │   │   │   │   │ ☻ ║   ║
║ ☻ │   │   │   │   │   ║   ║ ☺ │   │   │   │   │   ║   ║
║ ☻ │   │   │   │   │   ║   ║ ☺ │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║ ☺ │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
╠═══╬═══╬═══╬═══╬═══╬═══╬BAR╬═══╬═══╬═══╬═══╬═══╬═══╬OFF╣
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║ ☺ │   │   │   │   │   ║ 1 ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │   │   ║ ☺ ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │   │   │   │   ║   ║
║ ☺ │   │   │   │ ☻ │   ║   ║ ☻ │   │ ☻ │   │   │ ☺ ║   ║
╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝
 12  13  14  15  16  17      18  19  20  21  22  23
```
## End of the game
If the game ends, the `State` will indicate either `PlayerWonSingle` or `PlayerWonDouble`. 
The `Player` field will show which player won. 
The following example displays the board at the end of a game:
```
State:PlayerWonSingle Player:White White player pips:0 Black player pips:9
 11  10   9   8   7   6       5   4   3   2   1   0
╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗
║   │   │   │   │   │   ║   ║   │   │   │   │ ☺ │ ☺ ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║ 7 ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║ ☺ ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │ ☺ ║   ║
╠═══╬═══╬═══╬═══╬═══╬═══╬BAR╬═══╬═══╬═══╬═══╬═══╬═══╬OFF╣
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║ ☻ ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║15 ║
║   │   │   │   │   │   ║   ║   │   │   │   │   │   ║   ║
╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝
 12  13  14  15  16  17      18  19  20  21  22  23
```
Press ENTER to exit the program.
# Notes

The application follows this versioning scheme:
- The major version number increases with any functional changes. 
- The minor version number indicates a bug fix for the version specified by the major number. 
Pure bug fixes or non-functional changes always result in a minor version increment.

The application showcases functional programming within an object-oriented paradigm. 
Key concepts include:
- Immutability: Once variables, fields, or properties are assigned, they never change.
- Distinction between Actions and Pure functions. Actions include `Main()`, `PickTurnRoll()`, `ChooseTurnPlay()` (for Ask* players), and configuration-related methods. All other methods are Pure functions.  

# TODO
- Provide a board description in the documentation.
- Allow to configure how player can choose the next move.
- Offer the option to select a computer algorithm.
- Provide hints for valid inputs when gathering keyboard input.
- Handle invalid input gracefully by prompting the user to retry.
- Implement game state saving/loading.

