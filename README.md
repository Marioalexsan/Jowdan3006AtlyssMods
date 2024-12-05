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
  &bullet; Grey,  0
  &bullet; Blue,  1
  &bullet; Green, 2
  &bullet; Red,   3
  &bullet; None, 
</pre>

## MoreBankTabs
Adds 3 new bank tabs to spike. These work in the exact same way as the original 3 tabs.

This mod creates 3 new bank files in the same way as the base game does. The specific implementation seems quite fragile and I wouldn't be surprised if the developer makes large changes to the storage systems that break the mod. So back up your files. Although it shouldn't affect the original bank tabs, it's better safe than sorry.

### Known\Possible Issues
- Have seen some ui flickering and mouse hover conditions not working as intended, functionality should be unaffected.
- This is not tested in multiplayer and I'd assume it doesn't work. I also likely don't have the knowledge to fix it if it doesn't.
