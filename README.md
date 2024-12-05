# AtlyssMods
 My personal mods for [Atlyss](https://store.steampowered.com/app/2768430/ATLYSS/), can be found on [Thunderstore](https://thunderstore.io/c/atlyss/p/16MB/).\
 I'm a complete novice when it comes to modding and game development so don't expect these to work without issue.
## DyeCommands
A crude implementation to dye armour pieces in game by using commands in chat. All dye colours work and you can even remove dye from armour altogether. Not all armours look great with no dye applied as their orignal texture is an odd green colour. The colour of dyes is also not consistent across armour pieces. This is likely due to the orignal system dyeing all pieces at once and so to not always look like a crayon, the dev added variance to each armour piece for each dye.

### Known\Possible Issues
- This mod enables dyeing on all armour pieces. I've observed this in game to not actually do anything but I'm unaware if this causes any actual issue.
- This is not tested in multiplayer and I'd assume it doesn't work. I also likely don't have the knowledge to fix it if it doesn't.

### Usage
<pre>
Type "/Dye &lt;Armour> &lt;Dye>"
&bullet; Armour:
  &bullet; Helm,  H, 0
  &bullet; Chest, C, 1
  &bullet; Legs,  L, 2
  &bullet; Cape,  K, 3
  &bullet; All,   A, 4
&bullet; Dye: 
  &bullet; Grey,  W, 0
  &bullet; Blue,  B, 1
  &bullet; Green, G, 2
  &bullet; Red,   R, 3
  &bullet; None,  N, 4
</pre>
