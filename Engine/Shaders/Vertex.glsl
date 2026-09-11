#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 fragWorldPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 worldPos = vec4(aPos, 1.0) * model;

    fragWorldPos = worldPos.xyz;

    gl_Position = worldPos * view * projection;

    texCoord = aTexCoord;
}