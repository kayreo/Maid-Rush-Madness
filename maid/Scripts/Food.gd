extends RigidBody2D

# Declare the name property
var foodName

# Called when the node enters the scene tree for the first time
func _ready():
    pass

# Called every frame, 'delta' is the elapsed time since the previous frame
func _process(delta):
    pass

# Called when an Area2D enters the Food object
func _OnArea2DEntered(area: Area2D):
    if area.name == "Plate":
        # Get the visual sprite of the food
        var vis = $Visual as Sprite2D
        var parent = area.get_parent()
        parent.emit_signal("FoodObtained", foodName, vis.texture)
        queue_free()

# Called when the object leaves the screen
func _OnVisibleOnScreenNotifier2D_screen_exited():
    queue_free()
