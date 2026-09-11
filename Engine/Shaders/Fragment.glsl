#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 fragWorldPos;

uniform sampler2D texture0;

#define MAX_LIGHTS 32

uniform vec3 lightPositions[MAX_LIGHTS];
uniform float lightStrengths[MAX_LIGHTS];
uniform int lightCount;

void main()
{
    vec4 texColor = texture(texture0, texCoord);

    // Minimum brightness so areas without lights aren't completely black.
    float brightness = 0.1;

    for (int i = 0; i < lightCount; i++)
    {
        vec3 difference = lightPositions[i] - fragWorldPos;

        // Squared distance. Avoids needing sqrt().
        float distanceSquared = dot(difference, difference);

        float light =
            lightStrengths[i] / (distanceSquared + 1.0);

        brightness += light;
    }

    brightness = clamp(brightness, 0.0, 1.0);

    outputColor = vec4(
        texColor.rgb * brightness,
        texColor.a
    );
}