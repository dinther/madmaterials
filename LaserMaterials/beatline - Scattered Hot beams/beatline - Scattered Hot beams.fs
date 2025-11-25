/*{
    "CREDIT": "Beatline",
    "DESCRIPTION": "10 Random beams in two colors for high energy effectys",
    "TAGS": "hot beam, random, scatter, point, points, laser, strobe",
    "VSN": "1.1",
    "INPUTS": [
        {"LABEL": "Point Repeat", "NAME": "mat_point_repeat", "TYPE": "int", "MIN": 1, "MAX": 50, "DEFAULT": 10, "DESCRIPTION": "How many samples of a single point are send to the laser before moving on to the next point." },
        {"LABEL": "Intensity", "NAME": "mat_intensity", "TYPE": "float", "MIN": 0.01, "MAX": 1.0, "DEFAULT": 0.4,"DESCRIPTION": "Brightness of the points" },
        {"LABEL": "Color 1", "NAME": "mat_color1", "TYPE": "color", "DEFAULT": [ 0.0, 1.0, 1.0, 1.0 ], "FLAGS": "no_alpha", "DESCRIPTION": "Color of the hot beam. Randomly selected"},
        {"LABEL": "Color 2", "NAME": "mat_color2", "TYPE": "color", "DEFAULT": [ 0.4, 0.3, 1.0, 1.0 ], "FLAGS": "no_alpha", "DESCRIPTION": "Color of the hot beam. Randomly selected"},
    ],

    "RENDER_SETTINGS": {
       "POINT_COUNT": "mat_point_repeat"
    }
}*/


#include "MadNoise.glsl"

vec2 points[10] = vec2[10](
	vec2(0.39046006441720027,0.1460730147637339),
	vec2(0.3141039716884355,0.10354750582741179),
	vec2(-0.9321345855922982,-0.5032718829086107),
	vec2(-0.2727467024901613,0.016876668361322844),
	vec2(-0.3324741029111389,-0.1929078846337966),
	vec2(0.3973244436934982,-0.7380902878234192),
	vec2(-0.39735081932273975,0.4956180638019658),
	vec2(0.87418558980737,-0.3013422396304579),
	vec2(0.6830421405810756,-0.6558295171200452),
	vec2(0.31664482919018151,-0.035668174489683846)
);


void laserMaterialFunc(int pointNumber, int pointCount, out vec2 pos, out vec4 color, out int shapeNumber, out vec4 userData)
{
   float offset = 0.001;
   bool odd = (mod(pointNumber, 2.0)==0.0);
   float random1 = vnoise(vec2(TIME * 142.5634, TIME * 82.373));
   float random2 = noise(vec2(TIME * 342.9563, TIME * 572.324));
   int index = int(floor(random1 * 10));
   vec3 beamColor = mat_color1.rgb;
	if (random2 > 0.2)
     beamColor = mat_color2.rgb;
	
	// each point is in the middle
   pos = points[index];

   // each point is a shape
   shapeNumber = pointNumber; 

   // color point
   color = vec4(beamColor, mat_intensity);
}
