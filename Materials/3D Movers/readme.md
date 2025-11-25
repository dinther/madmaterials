# 3D Movers

<img width="797" height="759" alt="image" src="https://github.com/user-attachments/assets/f093c31a-ad52-47fd-be1a-967f7f9bd33b" />

Aiming one oving heads (movers) at a target is hard. Especially when you are not at the venue and you can't afford Virtual stage software. Aiming multiple mmovers is even harder.
This poor mans solution takes the guesswork out of it. Rather than setting the pan and tilt to aim at a presenter that isn't there right now, we can use geometry and calculate the pan and tilt angles.

Here we are assuming that the movers are all equally spaced along a line and are of identical capability. 
By setting the 3D start and end point and the number of movers we defined where exactly they are in space. It doesn't matter what point you consider the origin 0,0,0

As a target we also have a line. It will be a point if start and end are the same.

## Some numbers

Lets say we have a truss across the venue that is 4 meters wide at a height of 3 meters and we put 8 movers on it. Use feet or whatever, it doesn't matter as long as you are consistent. The target on stage is 3 meters away.
We will use X as the axis gpoing accross the venue and Y as the axis running along the venue. We will assume the center of the truss is above the X,Y point of (0,0).
Z is up.

This is what that will look like with the properties set.

<img width="311" height="594" alt="image" src="https://github.com/user-attachments/assets/8610e1f5-4e4b-4d44-aa77-5ac615fe5d6b" />

For the fixture line the start X at minus 2 meters and end X at 2 meters this making it a length of 4 as intended. 
The Y is on both ends zero and the height set to 3 meters.

For the target we aim at a point 3 meters away at neck height of 1.6 meters above the ground. The target becomes a point because the target start and end are the same.
The target could be a group of people in which case you make it wider. Thanks to the Z property you can also light a vertical target even with a horizontal truss full of movers.

## Details

In order for the math to work the program needs to map the full range of the color channel (0.0 to 1.0) to the range of the Pan and Tilt of the movers. It is important that these values are entered correctly. The defaults are typical values.
However, for the fancy movers there might be a channel that controls the beam angle. This can be handled by the program as well and thus ensures all light circles are relativly the same size.

Although you can map any channel to any color in your Fixture Editor, the convention is:

- Red for Pan
- Green for Tilt
- Blue for zoom (beam angle) optional

## Todo
- Figure out the best way to define the coordinates
- Add a Point2D parameter for fine tracking in the XY plane
- Add a slider parameter for fine tracking in the Z axis


