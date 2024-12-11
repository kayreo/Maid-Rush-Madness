extends CanvasLayer

# Called when the node enters the scene tree for the first time.
func _ready():
    pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
    pass

# Level 1 button pressed, emits "GetChallenge" signal with "ChallengeSera"
func _OnLevel1Pressed():
    get_parent().emit_signal("GetChallenge", "ChallengeSera")

# Level 2 button pressed, emits "GetChallenge" signal with "ChallengeLetti"
func _OnLevel2Pressed():
    get_parent().emit_signal("GetChallenge", "ChallengeLetti")

# Level 3 button pressed, emits "GetChallenge" signal with "ChallengeGob"
func _OnLevel3Pressed():
    get_parent().emit_signal("GetChallenge", "ChallengeGob")

# Level 4 button pressed, emits "GetChallenge" signal with "ChallengeDoll"
func _OnLevel4Pressed():
    get_parent().emit_signal("GetChallenge", "ChallengeDoll")

# Level 5 button pressed, emits "GetChallenge" signal with "ChallengeSphene"
func _OnLevel5Pressed():
    get_parent().emit_signal("GetChallenge", "ChallengeSphene")
