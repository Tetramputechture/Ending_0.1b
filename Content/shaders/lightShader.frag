#version 130

uniform vec2 lightPosition;
uniform vec4 lightColor;
uniform float lightRadius;
uniform vec4 ambientLightColor;

void main() 
{
    vec3 distance = vec3(gl_TexCoord[0].xy - lightPosition.xy, 5);

    float lightPower = 1 - length(distance) / lightRadius; // Light power between 0 and 1, where 0 is none and 1 is fullbright
    gl_FragColor = lightColor * lightPower;
}