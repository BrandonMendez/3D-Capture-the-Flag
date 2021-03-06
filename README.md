# 3D-Capture-the-Flag
Assignment 1 from the Advanced Game Development(COMP476) class offered at the University of Concordia.
At the start of the game, at random, one of the three teammates on each team will be selected to go capture the opposing flag. All players will randomly wander and will continue to do so unless specific actions and events take place. The two players not selected to capture the flag will attack any enemy who wanders into their home zone. More about attacking enemies wandering into a team's home explained later. The randomly selected player will only move out of its wandering cycle until a movement method has been selected. Movement can be selected as such:

•	-A: kinematic steering

•	-B: kinematic steering (different behaviour based off of distance)

•	-C: flee

•	-D: steering arrive

•	-E: steering arrive (different behaviour based off of distance)

Until one of these has been pressed, the selected player will wander like the rest. Once one is pressed, the selected players from each team will align with their target flag and begin its appropriate movement towards it, unless flee is selected where they will instead turn around to face the opposite direction and run away. Once either player crosses the center of the playing field, the two other opposing players who were wandering will turn around to face the intruder and begin to pursue him. If the opposing players make contact with the player trying to capture their flag, that player will be frozen. 

A player can only be frozen if touched by a pursuing enemy player in that enemies home zone. Once a player is frozen one of his teammates will be randomly selected to go assist the frozen player. The chosen player will turn and by using the same movement method will make its way over towards the frozen teammate. If successful in touching the frozen teammate, the teammate will unfreeze and continue with his duties. If he was assigned to capture the flag he will continue, if he has not been assigned to capture the flag he will either move in to attack an enemy in his home territory or continue wandering. If the player pursuing the flag is frozen, only once he is unfrozen can the team continue their attempt at capturing the flag.

The goal of the game is to bring the flag back to your home stand (the tiny platform holding up the flag) as fast as possible. That is the only way to achieve victory. If a player is holding the enemy's flag and successfully returns it to his home stand before the opposing team can do the same, they will win. 

Pertaining to the code, the scripts Team1 and Team2 control the two teams of players(red and green team). They are both very similar except for a few minor details. They control the movements and collisions for the players on each team. Team2 has been coded with more comments than Team1 therefore it would be the better one of the two to analyse if desired. The TeamController scripts are also very similar. They control the random assignment of players who will go capture the flag and who will go unfreeze and frozen teammate. Controller2 is also the one that has had extra comments added to it. 
