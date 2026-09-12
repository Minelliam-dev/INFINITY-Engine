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
    outputColor = texColor;
}