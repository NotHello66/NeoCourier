extends MeshInstance3D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

var time := 0.0

func _physics_process(delta: float) -> void:
	time += delta
	rotate_y(sin(time * 1.1) * delta * 1.5)
	rotate_x(sin(time * 0.6) * delta * 1.2)
	rotate_z(cos(time * 0.9) * delta * 1.3)
