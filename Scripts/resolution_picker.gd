extends MenuButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var menu = $".".get_popup()
	menu.clear()
	var resolutions = [
		Vector2i(1280, 720),
		Vector2i(1920, 1080),
		Vector2i(2560, 1440),
		Vector2i(3840, 2160)
		]
	for res in resolutions:
		menu.add_item("%d x %d" % [res.x, res.y])
		
	menu.connect("id_pressed", _on_resolution_selected)
	
	var current = DisplayServer.window_get_size()
	for i in resolutions.size():
		if resolutions[i] == current:
			menu.set_item_checked(i, true)

var resolutions = [
	Vector2i(1280, 720),
	Vector2i(1920, 1080),
	Vector2i(2560, 1440),
	Vector2i(3840, 2160)
]

func _on_resolution_selected(id: int) -> void:
	var menu = $".".get_popup()
	for i in resolutions.size():
		menu.set_item_checked(i, false)
	menu.set_item_checked(id, true)
	DisplayServer.window_set_size(resolutions[id])
	get_window().move_to_center()
