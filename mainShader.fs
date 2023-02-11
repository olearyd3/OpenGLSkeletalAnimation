#version 330

precision mediump float;

// surface normal and vertex positions from VS
varying vec3 normal;  
varying vec3 vertPos;  

// texture coordinates
in vec2 TexCoords;
uniform sampler2D ourTexture;

// visibility value for fog
in float visibility;

// material struct
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

uniform Material material;

uniform vec3 viewPos;

// light struct
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float constant;
    float linear;
    float quadratic;
};

#define NUM_POINT_LIGHTS 3

uniform Light pointLights[NUM_POINT_LIGHTS];

const vec3 fogColor = vec3(0.5, 0.5, 0.5);

// function for point lights
vec3 calculatePointLights(Light pointLights, vec3 norm, vec3 fragPos, vec3 viewVec);

void main() {
    vec3 N = normalize(normal);
    vec3 V = normalize(viewPos - vertPos);
    vec3 result = vec3(0.0f, 0.0f, 0.0f);
    for(int i = 0; i < NUM_POINT_LIGHTS; i++) {
        result = result + calculatePointLights(pointLights[i], N, vertPos, V);
    }
   //vec4 texColor = texture(ourTexture, TexCoords);
    gl_FragColor = mix(vec4(fogColor, 1.0), vec4(result, 1.0), visibility);
}

vec3 calculatePointLights(Light pointLights, vec3 norm, vec3 fragPos, vec3 viewVec) {
    vec3 lightVec = normalize(pointLights.position - fragPos);
    // diffuse
    float lambertian = max(dot(norm, lightVec), 0.0);
    // specular
    vec3 R = reflect(-lightVec, norm);
    // calculate the specular angle by getting the dot of the reflection and viewer vectors
    float specAngle = max(dot(viewVec, R), 0.0);
    // specular lighting is the angle to the power of the shininess value 
    float spec = pow(specAngle, material.shininess);
    float dist = length(pointLights.position - fragPos);
    float attenuation = 1.0 / (pointLights.constant + pointLights.linear * dist + pointLights.quadratic * (dist * dist));
    vec3 ambient = pointLights.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = pointLights.diffuse * lambertian * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = pointLights.specular * spec * vec3(texture(material.specular, TexCoords));
    //ambient *= attenuation;
    //diffuse *= attenuation;
    //specular *= attenuation;
    vec3 res = ambient + diffuse + specular;
    return res;
}