extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _physics_process(delta: float) -> void:
	if  GameData.timer <= 0:
		GameData.gameOver = true;
		$Background.visible = true
		$CenterContainer.visible = true
		get_tree().paused = true;
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		


func _on_restart_pressed() -> void:
	get_tree().paused = false;
	GameData.gameOver = false;
	get_tree().change_scene_to_file("res://testing scene timed.tscn")
	

func _on_quit_pressed() -> void:
	get_tree().paused = false;
	get_tree().change_scene_to_file("res://MainMenu.tscn")
