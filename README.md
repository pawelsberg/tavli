# tavli
Tavli games: portes (backgammon), plakoto, fevga, aso-dio - console application demonstrating functional programming in C#.
Application uses command-line interface.

# How to play
## Chose game
```
TAVLI 2.1 Copyright © Pawel Welsberg 2023

Choose one of (Portes, Plakoto, Fevga, AssoDio)
GameType>
```
Type in one of the 4 names listed in the brackets and press ENTER.
## Chose type of black player
```
Choose black player type (Human, Computer)
BlackPlayerType>
```
Type in `Human` to be able to control this player's move.
## Chose type of white player
The same principles apply as to the black player.
## Game board
On the screen will be shown current state of the game at this point of time.
## Roll for order
Then the question to provide a roll for the order of who plays first.
```
Rolling players: White, Black
Roll>
```
Each of the player rolls one die at the same time.
Type in 2 digits without space representing roll of white and black player. For example if you type in `16` it will mean that white player rolled 1 and black player rolled 6. 
Alternatively type in `r` for a random roll.
Player with the greatest number starts first.
If there is a draw players roll again.
## Player rolls dice to move 
A player will roll dice to move checkers
```
Rolling players: White
Roll>
```
At this point need to provide two digits representing score of each of the die. For example `23` means 2 and 3.
Alternatively type in `r` for a random roll.
Note that question for rolling appears for human player AND computer. 
## Player chooses move
If the player is of `Human` type then is able to choose his move from one of the available moves. For example:
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
Player rolled 5 and 5. Which entitles player to move 4 times. All possible moves are listed. Player needs to type in choice. It is from `1` to `4` in this case.
## Computer chooses move
Computer chooses move automatically as shown below. Rolling is still controlled by user.
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
In case there is the end of the game `State` will indicate `PlayerWonSingle` or `PlayerWonDouble`.
Field `Player` will indicate which player won.
Following example shows the board at the end of the game:
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
Pressing enter will end the program.
# Notes

Application uses the following versioning:
- Major part of the version always increases if there are any functional changes to the application. 
- Minor part of the version indicate a bug fix to the version indicated by the major part.
Pure bug fixes or non functional changes will always be released as a minor version increment.

Application demonstrates how to do functional programming in object oriented programming.
Main ideas:
- Immutability of variables/fields/properties. Once assigned they never change.
- Separation of the Actions and Pure functions. Actions: `Main()`, `PickTurnRoll()`, `ChooseTurnPlay()` (for Ask* players) and configuration related methods. Pure functions: all the remaining methods.   

# TODO
Board description in the docs
Allow to configure how player can choose the next move.
Allow to chose Computer alghorithm.
Add hint about valid inputs when reading inputs from keyboard.
Do not break on invalid input. Re try instead.
Add possibility of loading/saving game state.
Read proof the docs.

