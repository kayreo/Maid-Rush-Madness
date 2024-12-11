extends TextureRect

# Exported textures for the different challenges
@export var ChallengeSera : Texture2D
@export var ChallengeLetti : Texture2D
@export var ChallengeDoll : Texture2D
@export var ChallengeGob : Texture2D
@export var ChallengeAnnieAlex : Texture2D
@export var ChallengeSphene : Texture2D

# Dictionary to store the textures
var BGDict : Dictionary

# Signal to handle setting the background
signal SetBG()

# Initializes BG dictionary with corresponding textures
func init_bg_dict():
	BGDict = {
		"ChallengeSera": ChallengeSera,
		"ChallengeLetti": ChallengeLetti,
		"ChallengeDoll": ChallengeDoll,
		"ChallengeGob": ChallengeGob,
		"ChallengeAnnieAlex": ChallengeAnnieAlex,
		"ChallengeSphene": ChallengeSphene
	}

# Called when the node enters the scene tree for the first time
func _ready():
	init_bg_dict()
	# Connect the signal to the method
	SetBG.connect(_on_set_bg)

# Called every frame. 'delta' is the elapsed time since the previous frame
func _process(delta):
	pass

# Changes the background to the target scenario
func _on_set_bg(tgt : String):
	texture = BGDict.get(tgt, null)
