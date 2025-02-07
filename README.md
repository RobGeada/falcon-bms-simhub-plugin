# FalconBMS SimHub Plugin

This plugin connects [Falcon 4 BMS](https://www.falcon-bms.com/)'s telemetry output to [SimHub](https://www.simhubdash.com/), allowing the use of SimHub to control bass shakers, LEDs, wind simulators, etc., based on Falcon 4's telemetry. 

This project relies on lightingtools' [F4SharedMem tool](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMem) to read the telemetry out of the BMS shared memory.

# Installation
1) Download the prebuilt FalconBMS plugin: [RGeada.FalconBMS.dll](https://github.com/RobGeada/falcon-bms-simhub-plugin/raw/refs/heads/main/builds/RGeada.FalconBMS.dll)
2) Download the F4SharedMem.dll: [F4SharedMem.dll](https://github.com/RobGeada/falcon-bms-simhub-plugin/blob/main/F4SharedMem.dll)
3) Move both downloaded `dll`s to your SimHub folder in `$DRIVE:\\Program Files (x86)\SimHub`
4) Restart SimHub

You can check that the plugin has installed succesfully by looking in the "Available Properties" tab of SimHub and searching for `FalconBMS`: you should see a bunch of available properties.

# Build From Source
Alternatively, I've included the source code for the plugin, so if you're familiar with the SimHub plugin SDK, you can build and modify the plugin yourself!

# Provided Properties
You can see the telemetry fields that the plugin exposes in the main [FalconBMS.cs file](https://github.com/RobGeada/falcon-bms-simhub-plugin/blob/68ecbddbb9fcb147693aed62593b85c1f4495f5c/FalconBMS.cs#L51). For better documentation, you can look at the original [F4SharedMemory FlightData class](https://github.com/lightningviper/lightningstools/blob/9bc1be08717982922fbb0705f500d93a2b36da0d/src/F4SharedMem/FlightData.cs#L230), which is the source of all of these fields. 

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


gs = $prop("FalconBMS.ownship.gs") // get current g-forces
gsFromOne = Math.abs(gs - 1) // how far from 1G are we?
bump = Math.random() > .5 // waver the output volume randomly between 0 and 100, to add sensation of bumps

return bump * (gsFromOne * 100/8) // scale effect by gs, maximum intensity at 8g
```
I run this effect at 44hz.

### Cannon Fire
```javascript
//turn off effects if not in plane
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

gun = $prop("FalconBMS.IntelliVibe.IsFiringGun")
bump = (Math.random()<.9) * .2 + .8 // waver the output volume between 80% and 100%, to add organic fluctions to sound

return gun * bump * 100
```
The F16 cannon fires at 6,000 rpm, so run this effect at 100hz to exactly match the firing frequency. For added oomph, add a duplicate of this effect at 50hz.

### Afterburner
I run this effect in stereo, with two copies the function below in the left and right channels. This adds some extra spatial noise to sensation, giving more of a 3D feel.

```javascript
// turn off effects if not in plane
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}

// turn off effects if the engine is off
if ($prop("FalconBMS.ownship.fuelFlow")<1){
	return 0;
}


nozzle = Math.max(0, $prop("FalconBMS.ownship.nozzlePos") * 100 - 30) // how open is the engine nozzle aperture? 
rpm = $prop("FalconBMS.ownship.rpm")/100 // scale effect by engine rpm
bump = Math.random() < .8  // add random bumping 
return bump * nozzle * rpm
```
For this effect, I also modulate the frequency, by selecting `Forced Frequencies` in the effect settings. My frequency computation is:
```javascript
return $prop("FalconBMS.ownship.nozzlePos") * 80 + 100 // adjust effect pitch according to nozzle opening
```

This is just a few of the effects I've set up for my rig, I've got others for things like missile release, airbrake turbulence, and landing gear raising/lowering, and I'll be trying to figure out how to add touchdown, crash, eject, and damage effects in the near future. Feel free to open an issue here, or message me the [Falcon Lounge](https://www.falcon-lounge.com/) or [SimHub](https://discord.gg/nBBMuX7) discords (I'm @robgeada) to talk more about the plugin or custom SimHub effects!

# Disclaimers
I've tested this with BMS 4.37.4 and (licensed) SimHub 9.4.9. While I'll do my best to maintain this in the future, there is no guarantee that it will continue to work with future BMS/SimHub updates. 
