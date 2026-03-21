extends Node
var isCompetitive = false;
var activePackageSource : Node3D = null
var activePackageDestination : Node3D = null
var packageActive: bool = false
var timer : float
var gameOver :bool = false;
var hasPackage:bool = false;
var grappleHoldMode: = false;
var deliveriesComplete: int = 0;
var distanceCovered:float = 0;
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
