extends Label


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _physics_process(delta: float) -> void:
	if GameData.isCompetitive:
		self.visible = true;
	else:
		self.visible = false;
	var total_seconds = int(GameData.timer)
	var minutes = total_seconds / 60
	var seconds = total_seconds % 60
	self.text = "%d:%02d" % [minutes, seconds]
