// Part of the following texture sets:
//	Shared	GRAV002
terrain_scale 64
render_material_only 1
u_material standard_specular
render_pass
	{
	texture FAM\_GROSNUS\GRAV002
	shaded 1
	}
render_pass
	{
	u_type specular
	texture FAM\_GROSNUS\GRAV002_S
	uv_source ENVIRONMENT
	uv_mod SCALE 1.5 1.5
	}