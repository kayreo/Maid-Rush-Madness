extends CharacterBody2D

const SPEED = 800.0

var health = 4
var madeDish = false
var heldDish : String = ""
var HeldFood = []
var currentRequests = []

@onready var AnimatedSprite = $AnimatedSprite2D
@onready var ParticleEffect = $CPUParticles2D
@onready var Plate = $Plate
var parent

signal FoodObtained(food, spriteTexture)
signal DishMerged(food, spriteTexture)
signal TakeDamage()
signal ChangeDisplayHealth()
signal ChangeDisplayOrder()

func _ready():
	AnimatedSprite.animation = "health"
	FoodObtained.connect(GetFood)
	DishMerged.connect(GetDish)
	TakeDamage.connect(OnTakeDamage)
	ChangeDisplayHealth.connect(SetDisplayHealth)
	ChangeDisplayOrder.connect(SetDisplayOrder)

	# Optionally hide the dish sprite
	var dishSprite = Plate.get_node("DishSprite")
	parent = get_parent()
	dishSprite.hide()

func _physics_process(delta):
	if not parent.paused:
		# Movement
		if Input.is_action_pressed("left"):
			velocity.x = -SPEED
		elif Input.is_action_pressed("right"):
			velocity.x = SPEED

		# Place the food/dish when "place" action is pressed
		if Input.is_action_pressed("place"):
			HeldFood.clear()
			ClearHolding()
			var dishSprite = Plate.get_node("DishSprite")
			if madeDish:
				madeDish = false
				parent.emit_signal("PlaceDish", dishSprite, heldDish)
				dishSprite.texture = null
				dishSprite.hide()
				heldDish = ""
		
		# Apply friction
		velocity.x *= 0.9
		self.velocity = velocity
		move_and_slide()

# Converts health (4-0) to respective frame number (0-3)
func convert_to_frame() -> int:
	return 4 - health

# Add food to held
func GetFood(food : String, spriteTexture : Texture):
	if HeldFood.size() < 3:
		# Play SFX
		parent.SFX.stream = parent.ItemGet
		parent.SFX.play()
		HeldFood.append(food)
		var newLen = HeldFood.size()
		var newInd = newLen - 1
		var placePos = "FoodSprite" + str(newInd)
		var foodSprite = Plate.get_node(placePos)
		foodSprite.texture = spriteTexture
		foodSprite.show()
		merge_food()
		ParticleEffect.texture = spriteTexture
		ParticleEffect.emitting = true
		parent.emit_signal("UpdateIngredients", food)

func ClearHolding():
	for i in range(3):
		var placePos = "FoodSprite" + str(i)
		var foodSprite = Plate.get_node(placePos)
		foodSprite.texture = null
		foodSprite.hide()

func GetDish(dish : String, spriteTexture : Texture):
	ClearHolding()
	var dishSprite = Plate.get_node("DishSprite")
	dishSprite.show()
	dishSprite.texture = spriteTexture
	heldDish = dish
	madeDish = true

# Check if any ingredients can be merged
func merge_food():
	if HeldFood.size() > 2:
		print(get_parent().name)
		parent.emit_signal("FoodObtained", HeldFood)

func OnTakeDamage():
	health -= 1
	if health <= 0:
		print("Game over!")
		parent.emit_signal("GameOver")
	else:
		AnimatedSprite.frame = convert_to_frame()

func SetDisplayHealth():
	AnimatedSprite.animation = "health"
	AnimatedSprite.frame = convert_to_frame()

func SetDisplayOrder():
	AnimatedSprite.animation = "newOrder"
