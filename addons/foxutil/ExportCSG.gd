extends EditorInspectorPlugin

func _can_handle(object: Object) -> bool:
	return object.has_method('_add_inspector_buttons')

func _parse_begin(object: Object) -> void:
	var button = Button.new()
	button.text = "Meow"
	add_custom_control(button)
