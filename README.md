# FalconBMS SimHub Plugin

This plugin connects [Falcon 4 BMS](https://www.falcon-bms.com/)'s telemetry output to [SimHub](https://www.simhubdash.com/), allowing the use of SimHub to control bass shakers, LEDs, wind simulators, etc., based on Falcon 4's telemetry. 

This project relies on lightingtools' [F4SharedMem tool](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMem) to read the telemetry out of the BMS shared memory.

# Installation
1) Download the FalconBMS plugin [.dll file](https://github.com/RobGeada/falcon-bms-simhub-plugin/raw/refs/heads/main/builds/RGeada.FalconBMS.dll)
2) Move the downloaded `RGeada.FalconBMS.dll` to your SimHub folder in `$DRIVE:\\Program Files (x86)\SimHub`


# Some sample bass shaker configs:
SimHub's built-in bass shaker effects are pretty car-focused, so we'll have to write our own custom effects. Here are a few custom effects that I use as examples:

To set up a custom bass shaker effect in SimHub, go to `ShakeIt Bass Shakers`, select `Add Effect`, then scroll to the bottom to find `Custom Effect`.

### G-Force Rumbling
In the effect settings, hit edit, then toggle `Use Javascript`, then paste the following into the `Javascript` field:

```javascript
if ($prop("FalconBMS.Utility.stopEffects")){
	return 0
}


gs = $prop("FalconBMS.ownship.gs")
gsFromOne = Math.abs(gs - 1)

bump = Math.random() > .5 

return bump * (gsFromOne * 100/8)
```
