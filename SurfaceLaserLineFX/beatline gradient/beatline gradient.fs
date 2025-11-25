/*{
	"CREDIT": "Beatline",
	"TAGS": ["graphics"],
	"VSN": 1.1,
	"DESCRIPTION": "Creates a 3 color sequence whee the start and end colors are fixed. For the middle color you can define position along the line, width and distance to transition to the other color.",
	"INPUTS": [
    { "LABEL": "Start color", "NAME": "mat_start_color", "TYPE": "color", "DEFAULT": [1,0,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the start color of the gradient" },
    { "LABEL": "End color", "NAME": "mat_end_color", "TYPE": "color", "DEFAULT": [1,0,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the end color of the gradient" },	 
    { "LABEL": "Chained Lines", "NAME": "mat_chained", "TYPE": "bool", "DEFAULT": false, "FLAGS": ["button"], "DESCRIPTION":"Turn on to make position end where it started"},
    { "LABEL": "Color Segment/Color", "NAME": "mat_segment_color", "TYPE": "color", "DEFAULT": [1,0.6,0,1], "FLAGS": ["no_alpha"], "DESCRIPTION": "To choose the intermediate color of the gradient" },
	 { "LABEL": "Color Segment/Position", "NAME": "mat_segment_pos", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the position where this color is 100%"},
    { "LABEL": "Color Segment/Ping pong", "NAME": "mat_segment_pingpong", "TYPE": "bool", "DEFAULT": false, "FLAGS": ["button"], "DESCRIPTION":"Turn on to make position end where it started"},
    { "LABEL": "Color Segment/Width", "NAME": "mat_segment_width", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the width where color remains the same"},
    { "LABEL": "Color Segment/Feather", "NAME": "mat_segment_feather", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1., "DESCRIPTION":"To choose the width where color transition takes place"},
    
	],
	"GENERATORS": [
		{"NAME": "fx_time", "TYPE": "time_base", "PARAMS": {"speed": "fx_speed", "speed_curve": 2, "link_speed_to_global_bpm":true}},
	]
}*/

out vec4 mm_OutColor;

void main()
{
	if (mm_RenderingWhat == LINE_FOR_USER_DATA) {
		mm_OutColor = vec4(0);
		return;
	}

	vec4 mm_DataOnLine1 = getDataForThisPoint(0);
	vec4 mm_DataOnLine2 = getDataForThisPoint(1);

	vec2 pointPosition = mm_DataOnLine1.xy;
	// posData.xy is from (-1,-1) to (1,1), rescale it from (0,0) to (1,1)
	vec2 mm_SurfaceCoord = (vec2(1,1)+pointPosition)/2;

	// Animation Position (from 0  to 1) of this vertex in current line segment
	float mm_LineAnimationPos = mm_DataOnLine2.x;

	// Global Animation Position (from 0  to 1) of this vertex in the whole line surface (taking all line segments into account)
	float mm_GlobalAnimationPos = mm_DataOnLine2.y;

	// Line segment number (order in the stack)
	float mm_LineNumber = mm_DataOnLine2.z;

	if (mm_RenderingWhat == LINE_FOR_POSITION_AND_SHAPE) {
		// Write XY + shape number to a 32 bits floating point RGB FBO
		// If shape number is negative, this point will be ignored
		mm_OutColor.rg = pointPosition;
		mm_OutColor.b = mm_LineNumber; // This one can be changed in FX to multiply pathes
		mm_OutColor.a = mm_LineNumber; // This one should NOT be changed in FX if you want to preserve specific line settings (ie Min Ilda Points)
	}  else { // RENDERING_COLOR


	float progress = mat_chained==true? mm_DataOnLine2.y : mm_DataOnLine2.x;
   float gpos = mat_segment_pos;
	float hw = mat_segment_width / 2.0;

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
	mm_OutColor = progress > p4? vec4(mat_end_color.rgb, 1.) : vec4(mat_start_color.rgb, 1.);
	if (progress > p1 && progress <= p2){
      float cpos = (progress - p1) / (p2 - p1); 
      mm_OutColor = vec4(mix(mat_start_color.rgb,mat_segment_color.rgb,vec3(cpos)), 1.);
   } else if (progress > p3 && progress <= p4){
      float ccpos = (progress - p3) / (p4 - p3); 
      mm_OutColor = vec4(mix(mat_segment_color.rgb,mat_end_color.rgb,vec3(ccpos)), 1.);
	} else if (progress > p2 && progress <= p3){
		mm_OutColor = vec4(mat_segment_color.rgb, 1.);
   }



		// Write RGB Color to a 32 bits floating point RGB FBO

        // Get texture color for this point (handling texturing mode / input geometry etc.) 
        // Don't use getColorForPoint() in this case because the FX will do an animated shift of texture position
        // So reproduce code to sample the color depending on texturing mode & use fx_time to animate
        //mm_OutColor = getColorForPoint();
		  //mm_OutColor = vec4(vec3(mm_GlobalAnimationPos),1.);
        // Handle modulation color / highlight selection / active segment / clamping
        fx_ColorPostProcessing(mm_OutColor);
	}
}
