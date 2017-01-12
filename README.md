# CrewLight

## An automatic light manager


### What does it do ?

### It automatize light managment !

#### Every part that currently hold crew gets its lights turning on

![GIF of the crew transfer](http://i.imgur.com/QUqylip.gif)


#### Lights react to the sunshine

![GIF of Sunlight](http://i.imgur.com/hw9wEd8.gif)


#### When approaching a distant vessel its lights will blink, sending you a welcoming message in Morse code

![GIF of MorseLight](http://i.imgur.com/YlwWKMr.gif)


#### Kerbal on EVA can toggle light

![GIF of EVAToggleLight](http://i.imgur.com/DO9GwbO.gif)



### How does it works ?

Vessel's lights are divided into 3 groups : 
* **Lights of crewable part :**
  * They will remain off until a kerbal gets on-board
* **Lights NOT in the light action group : _(and kerbal's helmet light)_**
  * They'll go on when the sun's fall, off when it rises
* **Other Lights :**
  * They will work as usual, toggling by the light action group
  
  
### How to modify how it works ?
  
There is a file `Settings.cfg` in `Kerbal Space Program/GameData/CrewLight/PluginData/` with some variables to tweak, names and comment should be explicit enough. Most usefull one are :
* `morse_code` : change the morse message sent by distant vessel, [here](https://commons.wikimedia.org/wiki/File:International_Morse_Code.svg) you can find a Morse alphabet
* `distance` : set the distance at wich the Morse message begins
* `only_light_not_in_AG` : if `False` every light will toggle according to sunlight
* `always_on_in_space` : if `True` kerbal who un-board will always turn their lights on while on orbit
  
  
### What is needed ?
  
[ModuleManager](http://forum.kerbalspaceprogram.com/index.php?/topic/50533-121-module-manager-275-november-29th-2016-better-late-than-never/) is the only dependency, all credit go to ialdabaoth and sarbian


### What to do if it doesn't work ?

Report it to [Github](https://github.com/Li0n-0/CrewLight) or the [KSP forum]()



*This is my first mod for KSP and one of my first working C# code, I've tested it on every situation I could think of but it may have bugs left.*

A big thanks to all the modders/users of the KSP forum who have hepled me, direcly and by answering question before I posted them.

License is MIT
