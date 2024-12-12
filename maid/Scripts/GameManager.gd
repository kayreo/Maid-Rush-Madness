extends Node2D

# Signals declaration
signal GetChallenge(name : String)
signal BeginGame()
signal EndGame(won : int)
signal RestartGame()
signal ExitGame()
signal ContinueGame()
signal ReturnToMenu()
signal PlayClick()
signal ReturnComplete()

# Exported variables
@export var GameWorld : PackedScene
@export var GameOver : PackedScene
@export var Dialogue : PackedScene

# Node instances
var WorldInst
var GamInst
var DiaInst
var Credits
var MainMenu 
var Challenges
var BGMusic
var Fade 
var Game
var Click

# Default challenge and screen
var tgtChallenge : String = "ChallengeDoll"
var tgtScreen : String = "MainMenu"

# Stores order of challenges
var Progress : Array = []

# Initialize the challenge progression
func init_progress_dict():
	Progress = [
		"ChallengeSera",
		"ChallengeLetti",
		"ChallengeGob",
		"ChallengeDoll"
		# "ChallengeAnnieAlex"
	]

# Called when the node enters the scene tree for the first time
func _ready():
	MainMenu = $MainMenu
	Credits = $Credits
	Challenges = $Challenges
	BGMusic = $BGMusic
	Fade = $AnimationPlayer
	Game = $Game
	Click = $Click

	GetChallenge.connect(_on_get_challenge)
	BeginGame.connect(_on_begin_game)
	EndGame.connect(_on_end_game)
	RestartGame.connect(_on_restart_game)
	ExitGame.connect(_OnBackGameOverButtonPressed)
	ContinueGame.connect(_on_continue_game)
	ReturnToMenu.connect(_on_return)
	PlayClick.connect(_on_play_click)
	ReturnComplete.connect(return_complete)

	BGMusic.play(1.0)  # Start background music
	init_progress_dict()

# Called every frame. 'delta' is the elapsed time since the previous frame
func _process(delta):
	pass

# Play click sound
func _on_play_click():
	Click.play()

# Handle start button press
func _OnStartButtonPressed():
	Click.play()
	tgtChallenge = "ChallengeSera"
	tgtScreen = "Game"
	Fade.play("FadeIn")

# Handle modes button press
func _OnModesPressed():
	Click.play()
	tgtScreen = "Modes"
	Fade.play("FadeIn")

# Handle credits button press
func _OnCreditsPressed():
	Click.play()
	tgtScreen = "Credits"
	Fade.play("FadeIn")

# Handle exit button press
func _OnExitPressed():
	Click.play()
	get_tree().quit()

# Handle back button from credits
func _OnBackCreditsButtonPressed():
	Click.play()
	tgtScreen = "MainMenu"
	Fade.play("FadeIn")

# Handle back button from game over
func _OnBackGameOverButtonPressed():
	Click.play()
	tgtScreen = "Restart"
	Fade.play("FadeIn")

# Handle challenge selection
func _on_get_challenge(name : String):
	Click.play()
	tgtScreen = "Challenge"
	tgtChallenge = name
	Fade.play("FadeIn")

# Start the game
func _on_begin_game():
	WorldInst = GameWorld.instantiate()
	WorldInst.OnSetScenario(tgtChallenge)
	add_child(WorldInst)
	Game.get_node("DialogueHUD").queue_free()

# End the game
func _on_end_game(won : int):
	$GameWorld.queue_free()
	GamInst = GameOver.instantiate()
	GamInst._OnGetScenario(tgtChallenge, won)
	Game.add_child(GamInst)

# Restart the game
func _on_restart_game():
	Click.play()
	Game.get_node("GameOver").queue_free()
	WorldInst = GameWorld.instantiate() 
	WorldInst.OnSetScenario(tgtChallenge)
	add_child(WorldInst)

# Continue the game
func _on_continue_game():
	Click.play()
	tgtScreen = "Continue"
	var chalInd = Progress.find(tgtChallenge) + 1
	if chalInd < Progress.size():
		tgtChallenge = Progress[chalInd]
	else:
		complete_game()
	Fade.play("FadeIn")

# Return to menu
func _on_return():
	Click.play()
	tgtScreen = "Return"
	Fade.play("FadeIn")

# Complete the game
func complete_game():
#print("Reached end of scenario")
	tgtChallenge = "ChallengeEnd"

# Start the game
func return_complete():
	tgtScreen = "GoBack"
	Fade.play("FadeIn")

# Handle animation end event
func _OnAnimationPlayerAnimationFinished(anim_name : String):
	match anim_name:
		"FadeIn":
			match tgtScreen:
				"Game":
					MainMenu.hide()
					DiaInst = Dialogue.instantiate()
					DiaInst._on_set_scenario(tgtChallenge)
					Game.add_child(DiaInst)
				"Modes":
					MainMenu.hide()
					Challenges.show()
				"Credits":
					MainMenu.hide()
					Credits.show()
				"Challenge":
					Challenges.hide()
					if tgtChallenge != "ChallengeSphene":
						DiaInst = Dialogue.instantiate()
						DiaInst._on_set_scenario(tgtChallenge)
						Game.add_child(DiaInst)
					else:
						WorldInst = GameWorld.instantiate()
						WorldInst.OnSetScenario(tgtChallenge)
						add_child(WorldInst)
				"Restart":
					Game.get_node("GameOver").queue_free()
					MainMenu.show()
				"Continue":
					Game.get_node("GameOver").queue_free()
					DiaInst = Dialogue.instantiate()
					DiaInst._on_set_scenario(tgtChallenge)
					Game.add_child(DiaInst)
				"Return":
					$GameWorld.queue_free()
					MainMenu.show()
				"GoBack":
					Game.get_node("DialogueHUD").queue_free()
					MainMenu.show()
				_:
					Credits.hide()
					Challenges.hide()
					MainMenu.show()
			Fade.play("FadeOut")
