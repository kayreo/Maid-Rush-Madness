extends CanvasLayer

# Exports for the sound effects
@export var WonSFX : AudioStream
@export var LostSFX : AudioStream

# Signal to handle scenario changes
signal GetScenario(scenario: String, won_game: int)

# Member variables
var Dialogue
var Speaker
var SFX

var InProgress : bool = true

var DialogueScenarios : Dictionary
var DiaLine : Array

var diaWonGame : int = 0
var diaScenario : String = "ChallengeSera"

# Called when the node enters the scene tree for the first time
func _ready():
	Dialogue = $DialogueMini/DialogueText
	Speaker = $Control/Speaker
	SFX = $SFX

	# Connect the signal
	GetScenario.connect(_OnGetScenario)

	var jsonLoader = JSON.new()
	var file = FileAccess.open("res://Data/GameOverData.json", 1)
	var content = file.get_as_text()
	file.close()

	jsonLoader.parse(content)
	DialogueScenarios = jsonLoader.data

	var Dia = DialogueScenarios[diaScenario]
	DiaLine = Dia[str(diaWonGame)]
	print("My line: ", DiaLine)
	Speaker.animation = str(DiaLine[0])
	Speaker.frame = int(DiaLine[1])
	Dialogue.text = str(DiaLine[2])

	# Set the sound effect based on win/lose state
	if diaWonGame != 1:
		var ContinueButton = $Buttons/Continue
		ContinueButton.hide()
		SFX.stream = LostSFX
	else:
		SFX.stream = WonSFX
	
	SFX.play()

# Called every frame, 'delta' is the elapsed time since the previous frame
func _process(delta):
	if InProgress:
		# Scroll text
		if Dialogue.visible_characters < Dialogue.text.length():
			Dialogue.visible_characters += 1
			if Input.is_action_just_released("click"):
				Dialogue.visible_characters = -1
				InProgress = false
		# Text done scrolling, set visible chars to all
		elif Dialogue.visible_characters >= Dialogue.text.length():
			Dialogue.visible_characters = -1
			InProgress = false

# Button press functions
func _OnTryAgainButtonPressed():
	print("Changing scene")
	get_parent().get_parent().emit_signal("RestartGame")

func _OnExitButtonPressed():
	get_parent().get_parent().emit_signal("ExitGame")

func _OnContinueButtonPressed():
	get_parent().get_parent().emit_signal("ContinueGame")

# Function to handle scenario changes
func _OnGetScenario(scenario: String, won_game: int):
	diaScenario = scenario
	diaWonGame = won_game
