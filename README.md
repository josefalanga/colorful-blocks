# Colorful Blocks

## Purpose

This was a technical test for a Unity dev position. Had fun while building it, so I'm leaving it public.

## Task

I had 3 hours to complete 3 tasks. Tasks are tagged as requested. 

I added another `stretch-goals` tag with the work done outside the initial time budget (mostly DOTween animations and small corrections).

## Reflections

- I probably could have simplified the whole data structure on Blocks and BlockUI. The onclick forwarding ended up a little weird.
- I'm not using any object pooling for gameobjects, this kinda sucks as garbage collector pressure keeps piling. I guess is not too bad as the grid is small, but I don't really like it anyway.
- Block names/sprites ended up being entirely data driven. I think that was a good idea as new blocks can be added/removed without programmer intervention and the loop would work the same.
  - This implies all blocks are the same, it would need some updating if we add block types, like match-three games have (bomb blocks, stuff like that). I would probably add a new block type enum to handle them, with the current ones being the base ones.