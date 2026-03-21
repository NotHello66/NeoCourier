extends Label
var character: CharacterBody3D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	character = get_parent().get_parent()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
func _physics_process(delta: float) -> void:
	if GameData.hasPackage:
		if GameData.activePackageDestination != null:
			text = "%002d m" % (character.global_position.distance_to(GameData.activePackageDestination.global_position))
	else:
		if GameData.activePackageSource != null:
			text = "%002d m" % (character.global_position.distance_to(GameData.activePackageSource.global_position))
