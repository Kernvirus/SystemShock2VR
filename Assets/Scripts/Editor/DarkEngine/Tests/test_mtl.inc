# movie screen for SCP version of rec3
render_material_only 1
terrain_scale 128 64

# sweeping hand
render_pass {
	shaded 1
	ani_rate 50
	texture *_ 4 material/shtup/fam/movsw
	uv_mod SCALE 2 1
	blend ONE ZERO
}

# countdown numbers
render_pass {
	shaded 1
	ani_rate 1000
	ani_mode REVERSE
	texture { material/shtup/fam/movie "material/shtup/fam/movie1"   "material/shtup/fam/movie2" }
	uv_mod SCALE 2 1
	uv_mod uoffset_wave TURB -0.0025 0.005 0 1000
}

# screen mask
render_pass {
	shaded 1
	texture "material/shtup/fam/movie_m1"
}
