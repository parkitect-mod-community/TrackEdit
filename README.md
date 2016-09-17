**Description**

This mod allows the user to modify tracks.

**Planned Features**

- Method to attach segment back onto the grid
- re-implement segments and allow them to be built under the editor

**Controls**

- left-click drag - will move a track piece in the horizontal plane
- left-click left-ctrl drag - will move the node in the vertical position only 
- right click - will select a dot and make it active
- alt + 1-9 - grid for moving nodes

In the mod folder there is a config.json that can modified to change the key bindings for the vertical drag.

Consider donating if you want to see more: https://www.paypal.me/michaelpollind

**Changelog**

#### V1.3.6
- Implemented way to change the rotation of segments
- removed segment manager
- Changed the way deltas are calculated to the next segment to be more general

#### V1.3.7
- implemented a better way to approximate the end track after extrusion from an end segment
- track segments behind the extruded segment keep their shape
- added loop step to calculating bi-normal to avoid segment breakage

#### V1.3.8
- fix a bug with stuck rotation of end segment
- added grid to nodes

#### V1.4.0
- fixed shift bug with vertical drag and click
- redid track regeneration to run faster and also account for supports better


#### V1.4.1
- removed unity project
- added height mark helper

#### V1.5
- Removed special segment restrictions
- Updated ghost when moving a segment around
- Removed special UI
- Chain lift is applied on extrude when selected from the track builder

**Notes**

If you have any nice pictures using the mod I would like to exchange them with the default set of images I have at the moment.

Please submit issues with the mod to this issue tracker: [here](https://github.com/pollend/Parkitect_Track_Modify/issues)
