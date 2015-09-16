#version 130

uniform vec2 lightPosition;
uniform vec4 lightColor;
uniform float lightRadius;

void main() 
{
    vec2 distance = gl_TexCoord[0].xy - lightPosition.xy;

    float lightPower = 1 - length(distance) / lightRadius;
    
    if (lightPower <= 0) discard;
    gl_FragColor = lightColor * lightPower;
}