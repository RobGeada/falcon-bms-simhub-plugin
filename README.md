# FalconBMS SimHub Plugin

This plugin connects [Falcon 4 BMS](https://www.falcon-bms.com/)'s telemetry output to [SimHub](https://www.simhubdash.com/), allowing the use of SimHub to control bass shakers, LEDs, wind simulators, etc., based on Falcon BMS' telemetry. 

This project relies on lightingtools' [F4SharedMem tool](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMem) to read the telemetry out of the BMS shared memory.

# v0.2 Release Notes (June 6th, 2026)
* Standardizes variables names according to their underlying F4SharedMem names
* Automatic extraction of all telemetry variables, as opposed to old hardcoded system
* Automatic expansion of all telemetry bit objects (lightBits, bettyBits, etc) into individual boolean flags
   * e.g., Master Caution light status is now published directly to `FalconBMS.lights1.MasterCaution`, you no longer need to do byte conversion and bitwise flag parsing.

# Installation
1) Download the prebuilt FalconBMS plugin: [RGeada.FalconBMS.dll](https://github.com/RobGeada/falcon-bms-simhub-plugin/raw/refs/heads/main/builds/RGeada.FalconBMS.dll)
2) Download the F4SharedMem.dll: [F4SharedMem.dll](https://github.com/RobGeada/falcon-bms-simhub-plugin/blob/main/F4SharedMem.dll)
3) Move both downloaded `dll`s to your SimHub folder in `$DRIVE:\\Program Files (x86)\SimHub`
4) Restart SimHub

You can check that the plugin has installed succesfully by looking in the "Available Properties" tab of SimHub and searching for `FalconBMS`: you should see a bunch of available properties.

# Configuring BMS as a Custom Game in SimHub
From @gumby in the Falcon BMS forums:
1) From Simhub, go to Settings.
2) Select the “Custom games” tab
3) Create a new game called BMS and in Process Detection put Falcon BMS
4) You may also need to run SimHub as administrator - there’s an option for that in the General tab of the settings page
5) Run BMS, and at the top of the SimHub interface click Change active game > change it to BMS, then click activate.


# Build From Source
Alternatively, I've included the source code for the plugin, so if you're familiar with the SimHub plugin SDK, you can build and modify the plugin yourself!

# Provided Properties
After loading the plugin in Simhub, check "Available properties" and search for "Falcon BMS".

# My SimHub Bass Shaker Config
[Falcon BMS - rgeada.siprofile](https://raw.githubusercontent.com/RobGeada/falcon-bms-simhub-plugin/refs/heads/main/Falcon%20BMS%20-%20rgeada.siprofile)

# Some sample bass shaker configs:
SimHub's built-in bass shaker effects are pretty car-focused, so we'll have to write our own custom effects. Here are a few custom effects that I use as examples:

To set up a custom bass shaker effect in SimHub, go to `ShakeIt Bass Shakers`, select `Add Effect`, then scroll to the bottom to find `Custom Effect`.

### G-Force Rumbling
In the effect settings, hit edit, then toggle `Use Javascript`, then paste the following into the `Javascript` field:

```javascript
//turn off effects if not in plane
if ($prop("FalconBMS.Utility.stopEffects")){ 
	return 0
}


gs = $prop("FalconBMS.flightData.gs")
gsFromOne = Math.abs(gs - 1) ** 2
bump = Math.random() > .5 // waver the output volume randomly between 0 and 100, to add sensation of bumps

return bump*(gsFromOne*100/64) // scale effect by gs
```
I run this effect at 25hz.

### Cannon Fire
```javascript
//turn off effects if not in plane
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

gun = $prop("FalconBMS.IntelliVibe.IsFiringGun")
bump = (Math.random()<.5) * .2 + .8 // waver the output volume between 80% and 100%, to add organic fluctions to sound

return gun * bump * 100
```
The F16 cannon fires at 6,000 rpm, so run this effect at 100hz to exactly match the firing frequency. For added oomph, add a duplicate of this effect at 50hz.

### Taxi Bumps
Adds bumps when taxiing
```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

speed = Math.min(1, $prop('FalconBMS.flightData.kias') / 50)
bumpIntensity = $prop("FalconBMS.flightData.bumpIntensity") * 100 * speed 
bump = Math.random() < .5

return bump*bumpIntensity
```


### Touchdown Bumps
Adds bumps when wheels hit runway

In the "Run once javascript code" section of the custom effect editor:
```javascript
prev_ground = $prop("FalconBMS.IntelliVibe.IsOnGround")
bumps = 0	
```

In the "Javascript" section of the custom effect editor:
```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

ground = $prop("FalconBMS.IntelliVibe.IsOnGround")

if (!prev_ground && ground){
	if (bumps < 5){ // change "5" to a bigger/smaller number to lengthen or shorten the thump
		bumps += 1		
		return 100;
	} else {
		bumps = 0
		prev_ground = ground
		return 0;		
	}
}

bumps = 0
prev_ground = ground
return 0;
```

### Thump when AA ordinance releases
This will thump your bass shaker as a missile releases, providing feedback to let you know when it's fired.

In the "Run once javascript code" section of the custom effect editor:
```javascript
var oldAA = $prop("DataPluginDemo.FalconBMS.IntelliVibe.AAMissileFired")
var len = 0;
```

In the "Javascript" section of the custom effect editor:
```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

var newAA = $prop("FalconBMS.IntelliVibe.AAMissileFired")

if (newAA != oldAA){
	if (len < 5){ // change "5" to a bigger/smaller number to lengthen or shorten the thump
		len++;
		return 100;
	} else {
		len = 0
		oldAA = newAA;
	}
}
return 0;
```

### Thump when AG ordinance releases
In the "Run once javascript code" section of the custom effect editor:
```javascript
var oldAG = $prop("FalconBMS.IntelliVibe.AGMissileFired") + $prop("FalconBMS.IntelliVibe.BombDropped")
var len = 0;
```

In the "Javascript" section of the custom effect editor:
```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

var newAG = $prop("FalconBMS.IntelliVibe.AGMissileFired") + $prop("FalconBMS.IntelliVibe.BombDropped")

if (newAG != oldAG){
	if (len < 5){ // change "5" to a bigger/smaller number to lengthen or shorten the thump
		len++;
		return 100;
	} else {
		len = 0
		oldAG = newAG;
	}
}
return 0;
```

### Thump when chaff/flare release
In the "Run once javascript code" section of the custom effect editor:
```javascript
var oldCM = $prop("FalconBMS.IntelliVibe.ChaffDropped") + $prop("FalconBMS.IntelliVibe.FlareDropped")
var len = 0;
```

In the "Javascript" section of the custom effect editor:
```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

var newCM = $prop("FalconBMS.IntelliVibe.ChaffDropped") + $prop("FalconBMS.IntelliVibe.FlareDropped")

if (newCM != oldCM){
	if (len < 5){ // change "5" to a bigger/smaller number to lengthen or shorten the thump
		len++;
		return 100;
	} else {
		len = 0
		oldCM = newCM;
	}
}
return 0;
```

This is just a few of the effects I've set up for my rig, I've got others for things like airbrake turbulence and landing gear raising/lowering. Feel free to open an issue here, or message me the [Falcon Lounge](https://www.falcon-lounge.com/) or [SimHub](https://discord.gg/nBBMuX7) discords (I'm @robgeada) to talk more about the plugin or custom SimHub effects!

# Disclaimers
I've tested this with BMS 4.38.1 and (licensed) SimHub 9.10.13. While I'll do my best to maintain this in the future, there is no guarantee that it will continue to work with future BMS/SimHub updates. 
