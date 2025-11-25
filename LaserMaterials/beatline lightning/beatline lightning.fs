/*{
  "RESOURCE_TYPE": "Laser Material For MadMapper",
  "CREDIT": "Beatline",
  "DESCRIPTION": "Continuous, flowing lightning creates a plasma-like motion with no flicker or bursts.",
  "TAGS": "laser,lightning,noise,continuous,line",
  "VSN": "1.3",
  "INPUTS": [
    { "LABEL": "Line Start", "NAME": "mat_lineStart", "TYPE": "point2D", "MIN": [-1,-1], "MAX": [1,1], "DEFAULT": [0.0,0.8] },
    { "LABEL": "Line End",   "NAME": "mat_lineEnd",   "TYPE": "point2D", "MIN": [-1,-1], "MAX": [1,1], "DEFAULT": [0.0, -0.8] },
    { "LABEL": "Pin Start",  "NAME": "mat_lineStartPin","TYPE": "bool", "DEFAULT": true, "DESCRIPTION":"Holds the start of the lightning bolt in a fixed position."},
    { "LABEL": "Pin End",    "NAME": "mat_lineEndPin","TYPE": "bool", "DEFAULT": true, "DESCRIPTION":"Holds the end of the lightning bolt in a fixed position."},
    { "LABEL": "Amplitude",  "NAME": "mat_amplitude", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.4, "DESCRIPTION":"Displacement amplitude of the lightning wave" },
    { "LABEL": "Noise Scale","NAME": "mat_scale",     "TYPE": "float", "MIN": 0.1, "MAX": 10.0, "DEFAULT": 1.4, "DESCRIPTION":"Spatial frequency of the lightning noise" },
    { "LABEL": "Speed",      "NAME": "mat_speed",     "TYPE": "float", "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.2, "DESCRIPTION":"Animation speed of the lightning motion" },
    { "LABEL": "Core Width", "NAME": "mat_coreWidth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75, "DESCRIPTION":"How bright the center appears" },
    { "LABEL": "Color",      "NAME": "mat_color",     "TYPE": "color", "DEFAULT": [0.6,0.9,1.0,1], "FLAGS":["no_alpha"], "DESCRIPTION":"Lightning color" },
    { "LABEL": "Alpha",      "NAME": "mat_alpha",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "DESCRIPTION":"Overall opacity" },
    { "LABEL": "Flicker",    "NAME": "mat_flicker",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "DESCRIPTION": "Sets intensity variation over time" },
    { "LABEL": "Fades/Start Fade", "NAME": "mat_startFade", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.15, "DESCRIPTION": "Softens the start of the lightning. 0 = no fade, 0.5 = smooth fade-in from dark." },
    { "LABEL": "Fades/End Fade", "NAME": "mat_endFade", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.15, "DESCRIPTION": "Softens the end of the lightning. 0 = no fade, 0.5 = long fade-out to dark." },
  ],
  "GENERATORS": [
    { "NAME": "mat_time", "TYPE": "time_base", "PARAMS": { "speed": "mat_speed", "link_speed_to_global_bpm": false } }
  ]
}*/

#include "MadNoise.glsl"
#include "MadCommon.glsl"


void laserMaterialFunc(int pointNumber, int pointCount, out vec2 pos, out vec4 color, out int shapeNumber, out vec4 userData) {
  float progress = float(pointNumber) / float(max(1, pointCount - 1));
  float angle = 0;
  float angleOffset = 90;
  if (mat_lineStartPin==true){
    angle += 90;
    angleOffset = 0;
  }
  if (mat_lineEndPin==true){
    angle += 90;
    angleOffset = mat_lineStartPin==true? 0 : 90;
  }
  float pr = sin((angleOffset / 57.2958) + (angle / 57.2958 * progress));
  
  vec2 base = mix(mat_lineStart, mat_lineEnd, progress);

  vec2 dir = mat_lineEnd - mat_lineStart;
  float len = length(dir);
  vec2 perp = len > 0.0001 ? normalize(vec2(-dir.y, dir.x)) : vec2(0.0, 1.0);
  float mt = floor(mat_time / 0.07) * 0.07;
  float t = mt * 0.5;
  float x = progress * mat_scale * pr;

  // X Base displacement
  float xdisplacement = billowyTurbulence(vec2(x, t*16.0)) * 2.0 * pr; 
  float xfine = (billowyTurbulence(vec2(x * 1.5, t * 63.0))) * pr;
  float xoffset = (xdisplacement - xfine) * mat_amplitude;

  pos = base + perp * xoffset;

  // Continuous plasma effect — alpha stronger near center, soft falloff at ends
  float edgeFade = smoothstep(0.0, mat_startFade, progress) * (1.0 - smoothstep(1-mat_endFade, 1.0, progress));
  float coreFade = smoothstep(2.0, min(0.99, mat_coreWidth), xoffset / max(mat_amplitude, 0.0001));
  float alpha = abs(mat_alpha * edgeFade * coreFade);

  color = vec4(mat_color.rgb, clamp(vnoise(vec2(mat_time * 10.0, 2.0)),1.0 - mat_flicker, 1.0) * alpha);

  shapeNumber = 1;
}
