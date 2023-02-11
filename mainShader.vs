#version 330

in vec3 vertex_position;
in vec3 vertex_normal;
in vec2 vertex_texture;

uniform mat4 proj;
uniform mat4 model;
uniform mat4 view;

varying vec3 normal;
varying vec3 vertPos;

out vec2 TexCoords;

out float visibility;
out float distance;

const float density = 0.05;
const float gradient = 1.1;

void main() {
	
	vertPos = vec3(model * vec4(vertex_position, 1.0));
	normal = mat3(transpose(inverse(model))) * vertex_normal;

	gl_Position = proj * view * vec4(vertPos, 1.0);

	mat4 ModelViewMatrix = view * model;
	mat3 NormalMatrix =  mat3(ModelViewMatrix);
    //gl_Position = proj * view * model * vec4(vertex_position, 1.0);

	vec4 positionToCam =  ModelViewMatrix * vec4(vertex_position, 1.0);
	distance = length(positionToCam.xyz);
	visibility = exp(-pow(distance*density, gradient));

	vec4 vertPos4 = view * vec4(vertex_position, 1.0);
	vertPos = vec3(vertPos4) / vertPos4.w;
	normal = NormalMatrix * vertex_normal;

	TexCoords = vertex_texture;
}
