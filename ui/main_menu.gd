extends Node2D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_play_timed_pressed() -> void:
	GameData.gameOver = false;
	get_tree().change_scene_to_file("res://map1.tscn")
	GameData.isCompetitive = true;


func _on_play_freeroam_pressed() -> void:
	GameData.gameOver = false;
	get_tree().change_scene_to_file("res://map1.tscn")
	GameData.isCompetitive = false;


func _on_options_pressed() -> void:
	%MenuButtons.visible = false
	%SettingsMenu.visible = true


func _on_exit_pressed() -> void:
	get_tree().quit()


func _on_back_pressed() -> void:
	%MainButtons.visible = true
	%SettingsMenu.visible = false


func _on_fullscreen_toggled(toggled_on: bool) -> void:
	if(toggled_on):
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
		get_window().move_to_center()
		


func _on_main_vol_slider_value_changed(value: float) -> void:
	AudioServer.set_bus_volume_linear(AudioServer.get_bus_index("Master"), value)
