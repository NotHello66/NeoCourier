extends Node3D

var sourceNodes: Array = []
var destinationNodes: Array = []
@export var sourceNodesRoot: Node3D
@export var destinationNodesRoot: Node3D
@export var isCompetitive : bool = false
@export var startingTime :float = 200
@export var timeShrink: float = 0.9
@export var timePerMeter :float = 0.12
var shrinkingTimer :float
var packageActive: bool = false
var activePackageSource : Node3D = null
var activePackageDestination: Node3D = null
var rng = RandomNumberGenerator.new()
var previousDistanceCovered : int = 0
@export var minimapMesh : MeshInstance3D = null

func _ready():
	shrinkingTimer = startingTime;
	if isCompetitive:
		GameData.isCompetitive = true;
	for node in sourceNodesRoot.get_children():
		var component = node.get_node("PackageComponent")
		sourceNodes.append(component)
	
	for node in destinationNodesRoot.get_children():
		destinationNodes.append(node)
	
	if not packageActive:
		assignNewPackage()
	if isCompetitive:
		GameData.timer = startingTime
		GameData.isCompetitive = isCompetitive
	else:
		GameData.isCompetitive = false
		
func _physics_process(delta):
	packageActive = GameData.packageActive
	if not packageActive:
		GameData.distanceCovered += previousDistanceCovered
		assignNewPackage()
	if isCompetitive:
		GameData.timer -= delta
	if packageActive and GameData.hasPackage:
		minimapMesh.global_position = activePackageDestination.global_position
		minimapMesh.global_position.y += 5
	if packageActive and !GameData.hasPackage:
		minimapMesh.global_position = activePackageSource.global_position
		minimapMesh.global_position.y += 5

func assignNewPackage():
	var i = rng.randi() % sourceNodes.size()
	var j = rng.randi() % destinationNodes.size()
	activePackageSource = sourceNodes[i]
	activePackageDestination = destinationNodes[j]
	activePackageSource.hasPackage = true
	GameData.activePackageDestination = activePackageDestination
	GameData.activePackageSource = activePackageSource
	packageActive = true
	GameData.packageActive = true
	shrinkingTimer = timePerMeter * timeShrink * activePackageSource.global_position.distance_to(activePackageDestination.global_position)
	previousDistanceCovered = activePackageSource.global_position.distance_to(activePackageDestination.global_position)
	timeShrink *= 0.9
	GameData.timer += shrinkingTimer
	print("Source: " + activePackageSource.get_parent().name + "; Destination: " + activePackageDestination.name)
