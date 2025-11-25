# Beatline - Mover wave

![Thumbnail of the Beeatline - Mover wave material as it appears in MadMapper](./thumbnail.jpg)

This material is designed to coordinate all kinds of wave motions for moving heads. These commands are picked up via the theree color channels in the material.

<img width="291" height="824" alt="image" src="https://github.com/user-attachments/assets/f117fa10-032d-4c3d-ba26-95cc03d48d29" />

There are 3 independent occilators labeled Pan, Tilt and Intensit. They animate resp. the Red, Green and Blue color channels in the output material.

<img width="419" height="426" alt="image" src="https://github.com/user-attachments/assets/7577a14a-a7d6-4c71-b285-94b05a5d7891" />

## General properties
- Mover count. First you set the number of moving heads (movers) that you have lined up.
- Cycle offset. fine grained animation control. You can set the speed for the occilators to 0 and use this parameter with your own controller.
- Pan and Tilt fine adjust. Although most movers typically have a pan range of 540 deg. When you want to track a person on stage a much finer level of control is required. This 2D point input acts according the sensitivity sliders shown below it.

## Pan Tilt and Intensity occilators

The ossilators run a sine wave with a frequency equal to the beat if speed is set to 1.0 and BPM Sync is on. The value of the modulation is added/subtracted from the center value. The amplitude parameter defines the size of the modulation. The modulation is pinched to a smaller value if there is no headroom left at min or max values. 


