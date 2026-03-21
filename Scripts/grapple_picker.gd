extends MenuButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var menu = $".".get_popup()
	menu.clear()
	var options = [
		String("Hold"),
		String("Toggle")
		]
	for o in options:
		menu.add_item(o)
		
	menu.connect("id_pressed", _on_selected)
	
	var current = GameData.grappleHoldMode
	if current == true:
		menu.set_item_checked(0, true)
	else:
		menu.set_item_checked(1, true)
var options = [
		String("Hold"),
		String("Toggle")
		]
func _on_selected(id: int) -> void:
	var menu = $".".get_popup()
	for i in options.size():
		menu.set_item_checked(i, false)
	menu.set_item_checked(id, true)
	if id == 0:
		GameData.grappleHoldMode = true
	else:
		GameData.grappleHoldMode = false
