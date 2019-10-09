# Project Information
This document will contain any and all information pertaining to the unity VR project, Not Dodgeball(temp name).

**Project Description**

The project aims to make exercising fun with VR and Sensors. Body movements will be sensed with attached sensors (eg. vive tracker) and converted as input for the VR system. Users play the game by exercising. Appropriate virtual (in-game) rewards will be designed to motivate the players for winning.

## Admin Stuff:
**Tentative Timetable for Meetings**
- 3:30pm on Wednesdays with professor in his office
- Once the first prototype is out, ideally one playtest meeting in school every week or so.

**Projected Timeline**

**Milestones**

| Week  | Target                            | Status                                |
| ----- | --------------------------------- | ------------------------------------- |
| 6     | MVP Prototype                     | Complete for HTC Vive and Steam VR    |
| 7     | Online Multiplayer Functionality  | Lobby and Start Game Complete         |
| 8-9   | Migration to Oculus Quest         | Incomplete                            |


**Allocation of tasks**

---

# Not Dodgeball VR the game 2020

**Core mechanics:**
- Two player game with both players are put into a rectangular room at opposite sides.
- Each player has a goal which the aim is to score points into the opposing players goal.
    - Player's goal is attached to the player and will move as the player moves, following behind at a set distance.
    - The goal has two states - Stationary and Following.
        - A Stationary goal will have its position locked at where the goal's last Following position was.
        - A Following goal will be following the player.
    - When the player scores a goal, their own goal will be Following.
    - When the opposing player scores a goal, the player's goal will be Stationary.
- Game starts with each player owning 1 ball (TBC).
    - Players can spawn and throw as many balls as they own, with a short cooldown between spawning.
    - Either hand controller can spawn the ball.
    - Ball can bounce off every surface with physics matching ball material, and wall material.
    - Ball belongs to player who last touched it, and players can own-goal.
    - Every ball that is scored lets the scorer own 2 balls(TBC).
    - In case of the ball coming to a complete stop, destroy and spawn back in player's hand.
- The player can switch between spawning and a tool for each hand.
    - Tools have a wider area of collision and can be used to hit the ball.
    - If a player's hand remains stationary, the tool will become transparent and balls will not collide with it.
    - The tool will be active as long as the player is moving the controller.
- Players try to score points by getting balls into their respective goals.
    - Points are awarded for goals.
    - The more times the ball has been hit by both players, the more points it's worth. (TBC)
- The match continues until the set time for the match is over, and the player with the most points win the match.
- Players are allowed to host rooms for matches.
    - The maximum number of balls allowed in the game is decided by the game host.
    - The duration of a match, as well as the number of matches to count as a game, are also decided by the host.

**Design Sketches:**


**Fitness involvement:**
- Requires a lot of movement. ~~Kicking(TBC) and~~ smacking balls are allowed, hence it can be a full body workout.

**Projected behavioural impact:**
- It is a game that would be a logistical nightmare to implement in real life due to the number of balls to keep track of, so it is safe to assume there is no viable substitute for this game.
- With gimmicks such as daily rewards and in-game purchases implemented right, we can keep a significant number of players hooked on to the game and play it everyday, allowing them to get excerise through play.

---

**Notes:**

- It would be desirable to have a single player mode. We can use pure random rotation and flailing about to create a simple bot, but we should eventually turn to AI.
- Could require a tracker on each foot for kicking and/or dodging
- This is also hard to make fun/work as a PC game so it&#39;s very good for VR
- Should we curve the edges of the walls where they intersect to make the bouncing less predictable humanly? Again, more like air hockey style.
- This sem just make a simple playable game first, next sem then add the crazy shit
- Also add marketing?
- Need some learning value? Demonstration of new hardware? Pure game is ok but no A :( e.g. learn properties of diff materials (rubber ball vs styrofoam ball vs leather ball etc but by developing our own physics engine provided we can prove that existing one is not good enough)
- Possibly also change the shape and size of the ball
- Let players upload their own materials? Or at least customize existing ones
- Customize walls and tools too
- Do it for the KIDZ
- Individual contributions eg one do sensing hardware or smth
- Use machine learning to build our AI?
