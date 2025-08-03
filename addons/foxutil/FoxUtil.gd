@tool
extends EditorPlugin

var ExportCSG
func _enter_tree() -> void:
	ExportCSG = preload("res://addons/foxutil/ExportCSG.gd").new()
	add_inspector_plugin(ExportCSG)

func _exit_tree() -> void:
	remove_inspector_plugin(ExportCSG)
