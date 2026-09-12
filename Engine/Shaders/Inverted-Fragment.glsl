#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 fragWorldPos;

uniform sampler2D texture0;

#define MAX_LIGHTS 64

uniform vec3 lightPositions[MAX_LIGHTS];
uniform vec3 lightColors[MAX_LIGHTS];
uniform float lightStrengths[MAX_LIGHTS];
uniform int lightCount;

void main()
{
    vec4 texColor = texture(texture0, texCoord);

    // Minimum brightness so areas without lights aren't completely black.
    vec4 brightness;

    for (int i = 0; i < lightCount; i++)
    {
        vec3 difference = lightPositions[i] - fragWorldPos;

        // Squared distance. Avoids needing sqrt().
        float distanceSquared = dot(difference, difference);

        float light =
            lightStrengths[i] / (distanceSquared + 1.0);

        brightness.rbg += (lightColors[i] * (light));
    }

    brightness = clamp(brightness, 0.0, 1.0);

    vec4 OutCol;


    OutCol.rgb = 1.0 - (texColor + brightness).rgb;
    OutCol.a = 1;
    
    outputColor = OutCol;
}