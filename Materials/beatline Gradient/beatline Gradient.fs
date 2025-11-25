/*{
    "CREDIT": "Beatline",
    "DESCRIPTION": "Gradient with color stops: each color has a width for solid color and interpolates to the next color in between.",
    "TAGS": "gradient,color,color stop, stop",
    "VSN": "1.2",
    "INPUTS": [
        { "LABEL": "Color Count", "NAME": "mat_color_count", "TYPE": "int", "DEFAULT": 6, "MIN": 2, "MAX": 6, "DESCRIPTION": "Number of active color stops (2–6)" },
		  { "LABEL": "Offset", "NAME": "mat_offset", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "DESCRIPTION": "Offset for the gradient (0–1)" },

        { "LABEL": "Color Stop 1/Color", "NAME": "mat_color1", "TYPE": "color", "DEFAULT": [1.0, 0.0, 0.0, 1.0], "DESCRIPTION": "Color of stop 1" },
        { "LABEL": "Color Stop 1/Position", "NAME": "mat_color1_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.075, "DESCRIPTION": "Position of stop 1 along the gradient (0–1)" },
        { "LABEL": "Color Stop 1/Width", "NAME": "mat_color1_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 1 (0–1)" },

        { "LABEL": "Color Stop 2/Color", "NAME": "mat_color2", "TYPE": "color", "DEFAULT": [1.0, 1.0, 0.0, 1.0], "DESCRIPTION": "Color of stop 2" },
        { "LABEL": "Color Stop 2/Position", "NAME": "mat_color2_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "DESCRIPTION": "Position of stop 2 along the gradient (0–1)"},
        { "LABEL": "Color Stop 2/Width", "NAME": "mat_color2_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 2 (0–1)" },

        { "LABEL": "Color Stop 3/Color", "NAME": "mat_color3", "TYPE": "color", "DEFAULT": [0.0, 1.0, 0.0, 1.0], "DESCRIPTION": "Color of stop 3" },
        { "LABEL": "Color Stop 3/Position", "NAME": "mat_color3_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.425, "DESCRIPTION": "Position of stop 3 along the gradient (0–1)" },
        { "LABEL": "Color Stop 3/Width", "NAME": "mat_color3_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 3 (0–1)" },

        { "LABEL": "Color Stop 4/Color", "NAME": "mat_color4", "TYPE": "color", "DEFAULT": [0.0, 1.0, 1.0, 1.0], "DESCRIPTION": "Color of stop 4" },
        { "LABEL": "Color Stop 4/Position", "NAME": "mat_color4_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "DESCRIPTION": "Position of stop 4 along the gradient (0–1)" },
        { "LABEL": "Color Stop 4/Width", "NAME": "mat_color4_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 4 (0–1)" },

        { "LABEL": "Color Stop 5/Color", "NAME": "mat_color5", "TYPE": "color", "DEFAULT": [0.0, 0.0, 1.0, 1.0], "DESCRIPTION": "Color of stop 5" },
        { "LABEL": "Color Stop 5/Position", "NAME": "mat_color5_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.775, "DESCRIPTION": "Position of stop 5 along the gradient (0–1)" },
        { "LABEL": "Color Stop 5/Width", "NAME": "mat_color5_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 5 (0–1)" },

        { "LABEL": "Color Stop 6/Color", "NAME": "mat_color6", "TYPE": "color", "DEFAULT": [1.0, 0.0, 1.0, 1.0], "DESCRIPTION": "Color of stop 6" },
        { "LABEL": "Color Stop 6/Position", "NAME": "mat_color6_pos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.925, "DESCRIPTION": "Position of stop 6 along the gradient (0–1)" },
        { "LABEL": "Color Stop 6/Width", "NAME": "mat_color6_width", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "DESCRIPTION": "Width of solid color region for stop 6 (0–1)" },
   ],
}*/

vec4 materialColorForPixel(vec2 texCoord)
{
    int count = clamp(mat_color_count, 2, 6);
    float x = clamp(fract(texCoord.x + mat_offset), 0.0, 1.0);

    vec4 colors[6] = vec4[6](mat_color1, mat_color2, mat_color3, mat_color4, mat_color5, mat_color6);
    float positions[6] = float[6](mat_color1_pos, mat_color2_pos, mat_color3_pos, mat_color4_pos, mat_color5_pos, mat_color6_pos);
    float widths[6] = float[6](mat_color1_width, mat_color2_width, mat_color3_width, mat_color4_width, mat_color5_width, mat_color6_width);

    // Sort positions
    for (int i = 0; i < count - 1; i++) {
        for (int j = 0; j < count - 1 - i; j++) {
            if (positions[j] > positions[j + 1]) {
                float tmpP = positions[j]; positions[j] = positions[j + 1]; positions[j + 1] = tmpP;
                float tmpW = widths[j]; widths[j] = widths[j + 1]; widths[j + 1] = tmpW;
                vec4 tmpC = colors[j]; colors[j] = colors[j + 1]; colors[j + 1] = tmpC;
            }
        }
    }

    // Compute effective left/right edges constrained by neighbors
    float leftEdges[6];
    float rightEdges[6];

    for (int i = 0; i < count; i++) {
        float left = positions[i] - widths[i] * 0.5;
        float right = positions[i] + widths[i] * 0.5;

        // Constrain to [0,1]
        left = max(left, 0.0);
        right = min(right, 1.0);

        // Clamp by previous/next stop to squash if overlap occurs
        if (i > 0) left = max(left, positions[i - 1]);
        if (i < count - 1) right = min(right, positions[i + 1]);

        leftEdges[i] = left;
        rightEdges[i] = right;
    }

    // Loop through stops
    for (int i = 0; i < count; i++) {
        float left = leftEdges[i];
        float right = rightEdges[i];

        // Solid color region
        if (x >= left && x <= right) return colors[i];

    // Apply offset to positions and wrap
    for (int i = 0; i < count; i++) {
        positions[i] = mod(positions[i] + mat_offset, 1.0);
    }

        // Blend to next color if not last
        if (i < count - 1 && x > right && x < leftEdges[i + 1]) {
            float t = (x - right) / (leftEdges[i + 1] - right);
            return mix(colors[i], colors[i + 1], t);
        } else {
				float rm = 1.0 - rightEdges[count-1];
				float d = rm + leftEdges[0];
				if (x < leftEdges[0]){	 
					 float t = (x + rm) / d ;
			       return mix(colors[count-1], colors[0], t);
				}
				if (x > rightEdges[count - 1]) {
					float t = (x - rightEdges[count-1]) / d ;
					return mix(colors[count-1], colors[0], t);
				}
        }
    }

    // Fallback
    if (x < leftEdges[0]) return colors[0];
    return colors[count - 1];
}
