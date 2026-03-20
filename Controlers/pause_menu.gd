extends CanvasLayer

var isPaused :bool
var mouseCaptured:bool
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	mouseCaptured = true;
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass

func _unhandled_input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("pauseMenu") and GameData.gameOver == false:
		isPaused = !isPaused
		if isPaused:
			$MenuButtons.visible = true;
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			$Background.visible = true
			get_tree().paused = true
			$"../MusicPlayer".volume_linear *= 0.5
		else: 
			$"../MusicPlayer".volume_linear *= 2
			get_tree().paused = false
			$Background.visible = false;
			$MenuButtons.visible = false;
			$SettingsMenu.visible = false
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	if !isPaused and event is InputEventMouseButton and event.is_pressed():
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _on_resume_pressed() -> void:
	isPaused = false
	%MenuButtons.visible = false;
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	mouseCaptured = true;
	get_tree().paused = false
	$Background.visible = false;

func _on_options_pressed() -> void:
	$MenuButtons.visible = false;
	%SettingsMenu.visible = true;

func _on_exit_pressed() -> void:
	get_tree().paused = false
	get_tree().change_scene_to_file("res://MainMenu.tscn")
	
func _on_back_pressed() -> void:
	%MenuButtons.visible = true
	%SettingsMenu.visible = false
	
	
