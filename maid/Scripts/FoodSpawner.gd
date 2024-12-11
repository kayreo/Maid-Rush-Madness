extends Node2D

# Export the food scene to be set in the editor
@export var Food : PackedScene

var SpawnTimer
var Random = RandomNumberGenerator.new()

var Parent

# Called when the node enters the scene tree for the first time
func _ready():
    Random.randomize()
    SpawnTimer = $Timer
    Parent = get_parent().get_parent()
    SpawnTimer.wait_time = Random.randi_range(1, 7)
    SpawnTimer.start()

# Called every frame, 'delta' is the elapsed time since the previous frame
func _process(delta):
    pass

# Called when the timer times out
func _OnTimerTimeout():
    if not Parent.paused:
        var new_food = Food.instantiate()
        var new_food_vis = new_food.get_node("Visual")
        add_child(new_food)
        get_parent().get_parent().emit_signal("PickRandomFood", new_food)
        SpawnTimer.wait_time = Random.randi_range(1, 7)
