/*{
  "CREDIT": "MadMapper Assistant",
  "DESCRIPTION": "Calculates Pan (Red) and Tilt (Green) DMX values for a line of moving heads to look at a target line. Blue channel scales based on distance.",
  "CATEGORIES": [
    "Geometry",
    "DMX"
  ],
  "INPUTS": [
    {
      "NAME": "NumMovers",
      "TYPE": "int",
      "DEFAULT": 10,
      "MIN": 1.0,
      "MAX": 100.0,
      "LABEL": "Number of Fixtures"
    },
    {
      "NAME": "PanRange",
      "TYPE": "float",
      "DEFAULT": 540.0,
      "MIN": 90.0,
      "MAX": 720.0,
      "LABEL": "Pan Range (Deg)"
    },
    {
      "NAME": "TiltRange",
      "TYPE": "float",
      "DEFAULT": 270.0,
      "MIN": 90.0,
      "MAX": 360.0,
      "LABEL": "Tilt Range (Deg)"
    },
    {
      "NAME": "ZoomDistRange",
      "LABEL": "Zoom Fade Dist",
      "TYPE": "floatRange",
      "DEFAULT": [2.0, 10.0],
      "MIN": 0.0,
      "MAX": 70.0
    },
    {
      "NAME": "FixtureStart_X",
      "TYPE": "float",
      "DEFAULT": -1.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line start/X"
    },
    {
      "NAME": "FixtureStart_Y",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line start/Y"
    },
    {
      "NAME": "FixtureStart_Z",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line start/Z"
    },
    {
      "NAME": "FixtureEnd_X",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line End/X"
    },
    {
      "NAME": "FixtureEnd_Y",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line End/Y"
    },
    {
      "NAME": "FixtureEnd_Z",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Fixture line End/Z"
    },
    {
      "NAME": "TargetStart_X",
      "TYPE": "float",
      "DEFAULT": -1.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line start/X"
    },
    {
      "NAME": "TargetStart_Y",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line start/Y"
    },
    {
      "NAME": "TargetStart_Z",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line start/Z"
    },
    {
      "NAME": "TargetEnd_X",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line end/X"
    },
    {
      "NAME": "TargetEnd_Y",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line end/Y"
    },
    {
      "NAME": "TargetEnd_Z",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": -20.0,
      "MAX": 20.0,
      "LABEL": "Target line end/Z"
    }
  ]
}*/

const float PI = 3.14159265359;

vec4 materialColorForPixel( vec2 texCoord )
{
	 // 1. Determine which fixture index corresponds to the current pixel X coordinate
    // We quantize the UV coordinate based on the number of movers
    float normalizedIndex = floor(texCoord.x * NumMovers) / max(1.0, (NumMovers - 1.0));
    
    // Clamp to ensure we stay within bounds
    normalizedIndex = clamp(normalizedIndex, 0.0, 1.0);

    // Reconstruct vectors from individual float inputs
    vec3 FixtureStart = vec3(FixtureStart_X, FixtureStart_Y, FixtureStart_Z);
    vec3 FixtureEnd = vec3(FixtureEnd_X, FixtureEnd_Y, FixtureEnd_Z);
    vec3 TargetStart = vec3(TargetStart_X, TargetStart_Y, TargetStart_Z);
    vec3 TargetEnd = vec3(TargetEnd_X, TargetEnd_Y, TargetEnd_Z);

    // 2. Calculate the Physical Position of the specific fixture in 3D space
    vec3 currentFixturePos = mix(FixtureStart, FixtureEnd, normalizedIndex);

    // 3. Calculate the Target Position in 3D space
    // We assume the 1st fixture looks at the 1st target point, last looks at last.
    vec3 currentTargetPos = mix(TargetStart, TargetEnd, normalizedIndex);

    // 4. Calculate the Look Vector (Vector from fixture to target)
    vec3 lookDir = currentTargetPos - currentFixturePos;
    
    // Normalize logic:
    // We assume Z is UP.
    // Fixture Default: Hanging Down (Pointing at 0, 0, -1).
    // Fixture Front: Facing Positive Y (0, 1, 0)
    // Here we assume Standard Math: Y+ is "Front" relative to the grid.

    // --- PAN CALCULATION ---
    // Project direction onto XY plane
    // atan(x, y) gives angle relative to Y axis.
    float panAngleRad = atan(lookDir.x, lookDir.y); 
    float panAngleDeg = degrees(panAngleRad);
    
    // Map -180/180 to normalized DMX (0.0 - 1.0)
    // 0.5 is Center (0 degrees).
    // range is +/- PanRange/2
    float panDMX = 0.5 + (panAngleDeg / PanRange);


    // --- TILT CALCULATION ---
    // Calculate distance on horizontal plane
    float distXY = length(lookDir.xy);
    
    // Calculate elevation angle (atan of Y height vs Horizontal distance)
    float tiltAngleRad = atan(lookDir.z, distXY);
    float tiltAngleDeg = degrees(tiltAngleRad); 
    
    // Logic for "Hanging Down is Center":
    // If pointing straight down, Z is negative, distXY is 0. Angle is -90.
    // We want -90 degrees to output 0.5 DMX.
    // Angle difference from -90:
    float angleFromDown = tiltAngleDeg - (-90.0);
    
    // Map to DMX
    float tiltDMX = 0.5 + (angleFromDown / TiltRange);

    // --- ZOOM CALCULATION (BLUE) ---
    float dist = length(lookDir);
    // Use floatRange input. x = min distance (brightest), y = max distance (darkest)
    // smoothstep(edge0, edge1, x): returns 0.0 if x <= edge0, 1.0 if x >= edge1.
    // We want the reverse: closer (Min) = 1.0, further (Max) = 0.0
    float zoomDMX = smoothstep(ZoomDistRange.x, ZoomDistRange.y, dist);

    // 5. Output Colors
    // Red = Pan, Green = Tilt, Blue = Dimmer (Scaled by distance)
    return vec4(clamp(panDMX, 0.0, 1.0), clamp(tiltDMX, 0.0, 1.0), clamp(zoomDMX, 0.0, 1.0), 1.0);
    //return vec4(0.5 +(panAngleDeg/PanRange));
}