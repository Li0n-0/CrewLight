# CrewLight

## An automatic light manager


### What does it do ?

### It automatize lights managment !

#### Every part that currently hold crew gets its lights turning on

![GIF of the crew transfer](http://i.imgur.com/QUqylip.gif)


#### Lights react to the sunshine

![GIF of Sunlight](http://i.imgur.com/hw9wEd8.gif)


#### And to the depth

GIF incoming...


#### When approaching a distant vessel its lights will blink, sending you a welcoming message in Morse code

![GIF of MorseLight](http://i.imgur.com/YlwWKMr.gif)


#### Kerbal on EVA can toggle lights

![GIF of EVAToggleLight](http://i.imgur.com/DO9GwbO.gif)


### [Youtube review by KottabosGames](https://youtu.be/AE1pvzh2q1Y)


### How does it works ?

Vessel's lights are divided into 3 groups : 
* **Lights of crew-able part :**
  * They will remain off until a kerbal gets on-board
* **Lights NOT in the light action group : _(and kerbal's helmet light)_**
  * They'll go on when the sun's fall, off when it rises
* **Other Lights :**
  * They will work as usual, toggling by the light action group
  
This mod rely on the light action group to determine which light belongs to which group, in stock KSP you can change action group only in the editor, so no lights on all crafts already launched will react to the sun rising/falling. To change action group in flight you will need the mod [Action Groups Extended](http://forum.kerbalspaceprogram.com/index.php?/topic/67235-122dec1016-action-groups-extended-250-action-groups-in-flight-editing-now-kosremotetech/), by Diazo.
  
  
### How to modify how it works ?
  
There is a file `Settings.cfg` in `Kerbal Space Program/GameData/CrewLight/PluginData/` with some variables to tweak, names and comment should be explicit enough. Most useful one are :
* `morse_code` : change the morse message sent by distant vessel, [here](https://commons.wikimedia.org/wiki/File:International_Morse_Code.svg) you can find a Morse alphabet
* `distance` : set the distance at which the Morse message begins
* `only_light_not_in_AG` : if `False` every light will toggle according to sunlight
* `always_on_in_space` : if `True` kerbal who disembark will always turn their lights on while on orbit
  
  
### What is needed ?
  
[ModuleManager](http://forum.kerbalspaceprogram.com/index.php?/topic/50533-121-module-manager-275-november-29th-2016-better-late-than-never/) is the only dependency, all credits go to ialdabaoth and sarbian


### Know issues :

Lights on kerbal's helmet don't turn on/off as the sun rises/falls. Lights will be turned on if the kerbal disembark in the dark but status of the sunlight won't be checked after that.


### What to do if it doesn't work ?

Report it to [Github](https://github.com/Li0n-0/CrewLight) or the [KSP forum](http://forum.kerbalspaceprogram.com/index.php?/topic/154901-122-crew-light-an-automatic-light-manager-10-12-jan-2017/)



*This is my first mod for KSP and one of my first working C# code, I've tested it on every situation I could think of but it may have bugs left.*

**A big thanks to all the modders/users of the KSP forum who have helped me, directly and by answering question before I posted them** :wink:

License is MIT
