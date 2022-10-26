using Godot;
using godotlocalizationeditor;
using System;
using System.Linq;

public class MainView : Node2D
{
	FileDialog fileDialog;

	ItemList TargetLanguage;

	ItemList Keys;

	TextEditor textEditor;
	public override void _Ready()
	{
		fileDialog = this.GetChild<FileDialog>();

		textEditor = this.GetChild<TextEditor>();

		textEditor.TextChanged += TextEditor_TextChanged;

		textEditor.SetLabelText("Search:");

		textEditor.SetFont("res://fonts/Chinese.tres");

		TargetLanguage = (ItemList) this.GetChildByName(nameof(TargetLanguage));

		Keys = (ItemList)this.GetChildByName(nameof(Keys));

		fileDialog.Filters = new string[] { "*.csv" };
	}

	private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
	{
		DebugHelper.PrettyPrintVerbose($"Searched text: {e.NewText}", ConsoleColor.Green);
	}

	private void _on_Button_button_up()
	{
		if (!fileDialog.Visible)
		{
			fileDialog.Popup_();
		}
	}
	
	private void _on_FileDialog_file_selected(String path)
	{
		DebugHelper.PrettyPrintVerbose($"Selected file: {path}", ConsoleColor.Green);

		var file = new File();

		if (!file.FileExists(path)) return;

		file.Open(path, File.ModeFlags.Read);

		var lines = file.GetLines();

		file.Close();

		var languages = lines.First().Split(";");

		for (int i = 1; i < languages.Length; i++)
		{
			TargetLanguage.AddItem(languages[i]);
		}

		TargetLanguage.DisableTooltips();

		for (int i = 1; i < lines.Count; i++)
		{
			Keys.AddItem(lines[i].Split(";").First());
		}

		Keys.DisableTooltips();

	}
}




