/*{
  "RESOURCE_TYPE": "Laser Material For MadMapper",
  "CREDIT": "Beatline",
  "DESCRIPTION": "Simple line gradient. Between the start color and end color is a movable color segment. You can set the width of this segment and how wide the transition is to it's neighboring color. The line uses a 2D start and end point so that it can easily be animated. turn Ping pong on to get a back and forth movement without having to define animation curves. The strobe is linked to global BPM. It flashes 1,2,3 or 4 times per beat with a flash duration as a fraction of the cycle time.",
  "TAGS": "laser,gradient,strobe,line, bpm",
  "VSN": "1.1",
  "INPUTS": [
    { "LABEL": "Line Position/Start", "NAME": "mat_lineStart", "TYPE": "point2D", "MIN": [-1.,-1.], "MAX": [1.,1.], "DEFAULT": [-1.,0.], "DESCRIPTION":"To adjust the start position of your line"},
    { "LABEL": "Line Position/End", "NAME": "mat_lineEnd", "TYPE": "point2D", "MIN": [-1.,-1.], "MAX": [1.,1.], "DEFAULT": [1.,0.], "DESCRIPTION":"To adjust the end position of your line"},
    { "LABEL": "Main Colors/Start", "NAME": "mat_start_color", "TYPE": "color", "DEFAULT": [1,0,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the start color of the gradient" },
    { "LABEL": "Main Colors/End", "NAME": "mat_end_color", "TYPE": "color", "DEFAULT": [1,0,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the end color of the gradient" },	 
    { "LABEL": "Color Segment/Color", "NAME": "mat_segment_color", "TYPE": "color", "DEFAULT": [1,0.6,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the intermediate color of the gradient" },
	 { "LABEL": "Color Segment/Position", "NAME": "mat_segment_pos", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the position where this color is 100%"},
    { "LABEL": "Color Segment/Ping pong", "NAME": "mat_segment_pingpong", "TYPE": "bool", "DEFAULT": false, "FLAGS": ["button"], "DESCRIPTION":"Turn on to make position end where it started"},
    { "LABEL": "Color Segment/Width", "NAME": "mat_segment_width", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the width where color remains the same"},
    { "LABEL": "Color Segment/Feather", "NAME": "mat_segment_feather", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the width where color transition takes place"},
	 { "LABEL": "Intensity/Intensity", "NAME": "mat_alpha", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the overall intensity of the entire line"},
    { "LABEL": "Strobe/Activate", "NAME": "mat_strobeActivated", "TYPE": "bool", "DEFAULT": false, "FLAGS": ["button"], "DESCRIPTION":"To activate the strobe"},
	 { "LABEL": "Strobe/Times per beat", "NAME": "mat_speedStrobe", "TYPE": "int", "MIN": 0, "MAX": 4, "DEFAULT":1.0, "DESCRIPTION":"To adjust how many times it flashes per beat"},
	 { "LABEL": "Strobe/Duration", "NAME": "mat_strobeDuration", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "DESCRIPTION":"To adjust the duration of the flash"},	
    { "LABEL": "Strobe/Minimum", "NAME": "mat_strobeMinimum", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "DESCRIPTION":"To adjust the minimum brightness level of the strobe"},
  ],
  "GENERATORS": [
		{"NAME": "mat_timeStrobe", "TYPE": "time_base", "PARAMS": {"speed": "mat_speedStrobe","speed_curve": 1,"link_speed_to_global_bpm":true}},
  ]
}*/


void laserMaterialFunc(int pointNumber, int pointCount, out vec2 pos, out vec4 color, out int shapeNumber, out vec4 userData) 
{
   float progress = float(pointNumber)/(pointCount-1);
   float gpos = mat_segment_pos;
	pos = vec2(mix(mat_lineStart, mat_lineEnd, progress));
	float hw = mat_segment_width / 2.0;
	shapeNumber = 1;

   if (mat_segment_pingpong == true){
   	gpos = gpos * 2.0;
		if (gpos > 1.0){
			gpos = 2.0 - gpos;
		}
   }

	float p1 = gpos - hw - mat_segment_feather;
	float p2 = gpos - hw;
   float p3 = gpos + hw;
   float p4 = gpos + hw + mat_segment_feather;
	color = progress > p4? vec4(mat_end_color.rgb, mat_alpha) : vec4(mat_start_color.rgb, mat_alpha);
	if (progress > p1 && progress <= p2){
      float cpos = (progress - p1) / (p2 - p1); 
      color = vec4(mix(mat_start_color.rgb,mat_segment_color.rgb,vec3(cpos)), mat_alpha);
   } else if (progress > p3 && progress <= p4){
      float ccpos = (progress - p3) / (p4 - p3); 
      color = vec4(mix(mat_segment_color.rgb,mat_end_color.rgb,vec3(ccpos)), mat_alpha);
	} else if (progress > p2 && progress <= p3){
		color = vec4(mat_segment_color.rgb, mat_alpha);
   }

   //  strobe
   if(mat_strobeActivated){color=vec4(color[0],color[1],color[2],max(mat_strobeMinimum, color[3]*(fract(mat_timeStrobe*2)<mat_strobeDuration?1.:0.)));}
   return;
}