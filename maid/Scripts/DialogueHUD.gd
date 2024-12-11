extends CanvasLayer

# Declare member variables
var DialogueBox : TextureRect
var jsonLoader
var DialogueScenarios : Dictionary
var DialogueData : Dictionary
var Dialogue : RichTextLabel
var GameManager
var Speaker : AnimatedSprite2D
var BG : TextureRect
var CurrentScenario : String
var VisibleCharacters : int = 0

# Exported variables
@export var CurSpeaker : String = "0"
@export var Scenario : String = "None"

# Local flags and indices
var InProgress : bool = false
var CurrentLineIndex : int = 0
var DialogueStarted : bool = true

# Signals
signal EndFirstDialogue
signal EndDialogue
signal TriggerDialogue

# Called when the node enters the scene tree for the first time
func _ready():
	# Listen for signals
	EndDialogue.connect(_on_end_dialogue)
	TriggerDialogue.connect(_on_dialogue_trigger)
	
	# Load the dialogue data from file
	jsonLoader = JSON.new()
	var file = FileAccess.open("res://Data/DialogueData.json", 1)
	var content = file.get_as_text()
	file.close()
	
	jsonLoader.parse(content)
	DialogueScenarios = jsonLoader.data
	Dialogue = $Dialogue/DialogueText
	Speaker = $Control/Speaker
	BG = $BG
	
	emit_signal("TriggerDialogue", Scenario)

# Called every frame. 'delta' is the elapsed time since the previous frame
func _process(delta):
	if DialogueStarted:
		# Printing out a line
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
		# Move to the next line
		else:
			if Input.is_action_just_released("click"):
				continue_dialogue()
	else:
		Dialogue.hide()
		DialogueStarted = false
		CurrentLineIndex = 0
		Speaker.frame = 0

# Trigger the dialogue and load the scenario
func _on_dialogue_trigger(scenario: String):
	DialogueData = DialogueScenarios[scenario]
	CurrentScenario = scenario
	BG.emit_signal("SetBG", CurrentScenario)
	continue_dialogue()

# Continue displaying dialogue
func continue_dialogue():
	if DialogueData.has(str(CurrentLineIndex)):
		var DialogueLine = DialogueData[str(CurrentLineIndex)]
		var Who = DialogueLine[0]
		var Frame = int(DialogueLine[1])
		
		# Update speaker animation and position
		if Who == "none":
			Speaker.hide()
		else:
			if Who == "Onboarding":
				Speaker.position = Vector2(17, -130)
			else:
				Speaker.position = Vector2(17, 62)
			Speaker.animation = Who
			Speaker.frame = Frame
		
		# Set dialogue text and begin scrolling
		Dialogue.text = DialogueLine[2]
		Dialogue.visible_characters = 0
		InProgress = true
		CurrentLineIndex += 1
	else:
		# Dialogue finished, emit signal to end
		if CurrentScenario.find("Start") == -1:
			emit_signal("EndDialogue")

# Called when dialogue ends
func _on_end_dialogue():
	hide()
	get_parent().get_parent().emit_signal("BeginGame")

# Set the scenario for the dialogue
func _on_set_scenario(name: String):
	print("Target challenge is now: ", name)
	Scenario = name

# Skip the dialogue
func _OnSkipButtonPressed():
	get_parent().get_parent().emit_signal("PlayClick")
	emit_signal("EndDialogue")
