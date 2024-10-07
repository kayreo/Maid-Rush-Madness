shader_type canvas_item;
// Code adapted from: https://godotshaders.com/shader/panning/
// --- Uniforms --- //
uniform vec2 speed = vec2(0.001, 0.0);
uniform vec2 increment = vec2(1.0, 0.0);

void vertex() {
	// Called for every vertex the material is visible on.
}

// --- Functions --- //
void fragment() {
	COLOR = texture(TEXTURE, mod(UV + mod(TIME, 10.0) * speed, 1.0));
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
