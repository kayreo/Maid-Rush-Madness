extends Node2D

@export var PlacedDish : PackedScene
@export var OrderDing : AudioStream
@export var ItemGet : AudioStream
@export var ItemPlace : AudioStream
@export var TimerUp : AudioStream

var foodTree
var dishes
var Recipes
var Sprites = Dictionary()
var player
var table
var requestUI 
var orderTimer
var appearTimer
var GameTimer
var TimerFill
var TimerPoint
var BG
var PauseScreen
var CurRecipes = []
var jsonLoader
var FoodDict = Dictionary()
var ChallengeDict = Dictionary()
var CurChallenge = ""
var CurIngredients = []
var RemainingIngredients = []
var RecipeKeys
var Random = RandomNumberGenerator.new()
var SFX 
var TimerSFX
var paused = false
var spriteFilePath = "res://Assets/items/"
var tgtRecipe = ""

signal FoodObtained(FoodToMerge)
signal SetScenario(name)
signal PlaceDish(DishToPlace, DishName)
signal PickRandomFood(newFood)
signal GameOver()
signal UpdateIngredients(food)

func _ready():
	var json = JSON.new()

	FoodObtained.connect(MergeFood)
	PlaceDish.connect(OnPlaceDish)
	PickRandomFood.connect(OnPickRandomFood)
	GameOver.connect(EndGame)
	UpdateIngredients.connect(UpdateRemainingIngredients)

	# Load food data
	var dataFile = FileAccess.open("res://Data/FoodData.json", 1)
	var foodData = dataFile.get_as_text()
	dataFile.close()

	Recipes = json.parse_string(foodData) 
	#print("Recipes: ", Recipes)
	RecipeKeys = Recipes.keys()
	foodTree = FoodTree.new(Recipes)

	# Load visual data
	var spriteFile = FileAccess.open("res://Data/SpriteData.json", 1)
	var spriteData = spriteFile.get_as_text()
	spriteFile.close()
	var tempDict = json.parse_string(spriteData)
	for line in tempDict.keys():
		var newTexture = load(spriteFilePath + tempDict[line])
		Sprites[line] = newTexture

	# Load challenge data
	var challengeFile =FileAccess.open("res://Data/ChallengeData.json", 1)
	var challengeData = challengeFile.get_as_text()
	challengeFile.close()
	
	ChallengeDict = json.parse_string(challengeData)
	player = get_node("Serafina")
	table = get_node("Table")
	dishes = get_node("Dishes")
	requestUI = get_node("RequestWindow") 
	orderTimer = get_node("OrderTimer")
	appearTimer = get_node("AppearTimer")
	BG = get_node("BG")
	GameTimer = get_node("Timer")
	TimerFill = get_node("Timer/TimerFill")
	TimerPoint = get_node("Timer/TimerPoint")
	PauseScreen = get_node("PauseScreen")
	SFX = get_node("SFX")
	TimerSFX = get_node("TimerSFX")

	# Pick a challenge
	BG.emit_signal("SetBG", CurChallenge)
	CurRecipes = ChallengeDict[CurChallenge]

	# Set time limit
	match CurChallenge:
		"ChallengeSera":
			appearTimer.wait_time = 5
			orderTimer.wait_time = 36
			TimerFill.max_value = 36
		"ChallengeLetti":
			orderTimer.wait_time = 32
			TimerFill.max_value = 32
		"ChallengeAnnieAlex":
			orderTimer.wait_time = 28
			TimerFill.max_value = 28
		"ChallengeGob":
			orderTimer.wait_time = 24
			TimerFill.max_value = 24
		"ChallengeDoll":
			appearTimer.wait_time = 1
			orderTimer.wait_time = 16
			TimerFill.max_value = 16
		"ChallengeSphene":
			orderTimer.wait_time = 20
			TimerFill.max_value = 20

	# Pick a random dish
	#print("Recipes for ", CurChallenge, ": ", CurRecipes)
	OnPickRandomDish()
	orderTimer.start()
	PopulateRandomFoodChoices()

func _process(delta):
	if not paused:
		appearTimer.paused = false
		orderTimer.paused = false
		TimerFill.value = orderTimer.wait_time - orderTimer.time_left
		TimerPoint.rotation_degrees = 360.0 * (TimerFill.value / TimerFill.max_value)
	else:
		appearTimer.paused = true
		orderTimer.paused = true

	# Timer sound effects
	if TimerFill.value / TimerFill.max_value >= 0.75 and not TimerSFX.playing:
		TimerSFX.play()
	elif TimerFill.value / TimerFill.max_value <= 0.0 and TimerSFX.playing:
		TimerSFX.stop()

	if Input.is_action_just_pressed("pause"):
		if PauseScreen.visible:
			paused = false
			PauseScreen.hide()
		else:
			paused = true
			PauseScreen.show()

func MergeFood(FoodToMerge):
	#print("Merging food: ", FoodToMerge)
	var recipe = foodTree.find_recipe(FoodToMerge)
	#print("My recipe: ", recipe);
	if recipe == "Null":
		recipe = "Slop"
	player.emit_signal("DishMerged", recipe, Sprites[recipe])

func UpdateRemainingIngredients(food):
	if RemainingIngredients.has(food):
		RemainingIngredients.erase(food)

func OnPlaceDish(dish, dishName):
	SFX.stream = ItemPlace
	SFX.play()
	var posX = int(dish.global_position.x)
	var newNode = PlacedDish.instantiate()
	var vis = newNode.get_node("Sprite2D")
	vis.texture = dish.texture
	vis.scale = Vector2(0.5, 0.5)
	newNode.position = Vector2(posX, table.position.y - 150)
	dishes.add_child(newNode)
	newNode.velocity = Vector2(0, 400)
	if dishName == tgtRecipe:
		#print("Correct Dish!")
		orderTimer.stop()
		orderTimer.start()
		if CurChallenge != "ChallengeSphene":
			CurRecipes.erase(dishName)
		if CurRecipes.is_empty():
			#print("No more recipes, game complete")
			get_parent().emit_signal("EndGame", 1)
		else:
			OnPickRandomDish()
	else:
		#print("Incorrect dish")
		RemainingIngredients.clear()
		var recipeIngredients = Recipes[tgtRecipe]
		for i in range(3):
			RemainingIngredients.append(recipeIngredients[i])

func OnPickRandomFood(newFood):
	Random.randomize()
	var tgtWeight = 5
	var randomWeight = int(Random.randi_range(0, 10))
	var randomKey = int(Random.randi_range(0, CurIngredients.size() - 1))
	var randomSprite = CurIngredients[randomKey]
	if RemainingIngredients.size() > 0 and randomWeight <= tgtWeight:
		randomKey = int(Random.randi_range(0, RemainingIngredients.size() - 1))
		randomSprite = RemainingIngredients[randomKey]
	var randomTexture = Sprites[randomSprite]
	var newFoodVis = newFood.get_node("Visual") as Sprite2D
	newFoodVis.texture = randomTexture
	newFood.foodName = randomSprite

func PopulateRandomFoodChoices():
	for recipe in RecipeKeys:
		var toLoadRecipe = Recipes[recipe]
		for ingredient in toLoadRecipe:
			if not CurIngredients.has(ingredient):
				CurIngredients.append(ingredient)

func OnPickRandomDish():
	RemainingIngredients.clear()
	Random.randomize()
	var randomKey = int(Random.randi_range(0, CurRecipes.size() - 1))
	var randomDish = CurRecipes[randomKey]
	var recipeIngredients = Recipes[randomDish]
	var recipeLabel = requestUI.get_node("Display/VBoxContainer/RecipeLabel")
	recipeLabel.text = randomDish
	var ingredientsList = requestUI.get_node("Display/VBoxContainer/Ingredients")
	for i in range(ingredientsList.get_child_count()):
		var castedI = ingredientsList.get_child(i) as TextureRect
		var textI = Sprites[recipeIngredients[i]]
		castedI.texture = textI
		RemainingIngredients.append(recipeIngredients[i])
	tgtRecipe = randomDish
	player.emit_signal("ChangeDisplayOrder")
	ShowRequest()

func _OnOrderTimeout():
	ShowRequest()
	player.emit_signal("TakeDamage")

func _OnAppearTimeout():
	var display = requestUI.get_node("Display")
	display.hide()
	player.emit_signal("ChangeDisplayHealth")

func ShowRequest():
	var display = requestUI.get_node("Display")
	SFX.stream = OrderDing
	SFX.play()
	display.show()
	appearTimer.start()

func EndGame():
	#print("Ending game")
	get_parent().emit_signal("EndGame", 0)

func OnSetScenario(name):
	#print("Tgt challenge is now: ", name)
	CurChallenge = name

func _OnPauseButtonPressed():
	get_parent().emit_signal("PlayClick")
	paused = true
	PauseScreen.show()

func _OnContinueButtonPressed():
	get_parent().emit_signal("PlayClick")
	paused = false
	PauseScreen.hide()

func _OnExitButtonPressed():
	get_parent().emit_signal("PlayClick")
	get_parent().emit_signal("ReturnToMenu")

# The FoodNode and FoodTree classes remain mostly the same except for the syntax and method calls.
class FoodNode:
	var ingredient : String
	var recipe : String
	var children : Array = []

	func _init(ingredient_name : String):
		ingredient = ingredient_name

	func set_recipe(recipe_name : String):
		recipe = recipe_name

class FoodTree:
	var root : FoodNode = FoodNode.new("Null")

	func _init(recipes : Dictionary):
		for recipe in recipes.keys():
			var recipe_name = recipe
			var recipe_to_check = recipes[recipe]
			recipe_to_check.sort()
			insert(root, recipe_to_check, recipe_name, 0)

	func insert(root : FoodNode, foods : Array, recipe_name : String, start : int):
		if start >= foods.size():
			root.recipe = recipe_name
			return
		var food_to_add = foods[start]
		for child in root.children:
			if child.ingredient == food_to_add:
				insert(child, foods, recipe_name, start + 1)
				return
		var new_node = FoodNode.new(food_to_add)
		root.children.append(new_node)
		insert(new_node, foods, recipe_name, start + 1)

	func find_recipe(ingredients : Array) -> String:
		ingredients.sort()
		return traverse_tree(root, ingredients, 0)

	func traverse_tree(root : FoodNode, ingredients : Array, start : int) -> String:
		if start >= ingredients.size():
			if root.recipe != null:
				return root.recipe
			return "Null"
		for child in root.children:
			if child.ingredient == ingredients[start]:
				return traverse_tree(child, ingredients, start + 1)
		return "Null"
