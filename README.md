# Some Cool Hex Game
Unity Version: 2019.1.12f1

## Description

This is intended to be a turn based strategy game. It is a bit different from others I've played, I'll explain how:

A turn consists of all players on in the game planning (giving orders) to all of their units. The units do not follow these orders yet, they are just planned. When everyone is ready, they hit 'end turn', whereupon the units begin executing their orders.

The orders can consist of moving, shooting, providing overwatch, digging in, etc.

Units can be suppressed by enemy fire (losing accuracy to their own attacks), they gain defense when staying in place and lose defense when moving.

Different tiles provide different positives and negatives like extra defense, or a boost or suppression to movement.

Different units have different types of attacks, for example riflemen attack a unit, as opposed to a tile, whereas mortar teams attack tiles as opposed to units. Rifle men have their attacks obstructed by cliffs and trees, where mortar teams can fire over them.

If anyone has any additional ideas, feel free to add them here, or add them as tasks to the Project.

(On the Movement branch)
When you give orders to the units, they get added to a queue, They do not execute the orders yet.
When you hit space bar, you are 'ending your turn'. At this point the units will begin following their predetermined paths.
If a move order spans multiple turns, a command will be queued for each consecutive turn. Just keep hitting spacebar to end those turns.

(On the Attack branch)
You can use pageUp and pageDown to switch whith team a unit you spawn is. So, before placing a unit, pick the team then place them.
This will later be used to determine friends from foes.
