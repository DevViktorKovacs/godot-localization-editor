using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;

public class MainView : Node2D
{
	Label CurrentKey;

	ItemList TargetLanguage;

	TextEdit ReferenceTextEdit;

	ItemList Keys;

	TextEditor textEditor;

	FileDialog fileDialog;

	static string fontPath = "res://fonts/Chinese.tres";

	Dictionary<int, LocalizedTexts> localizations;

	public override void _Ready()
	{
		fileDialog = this.GetChild<FileDialog>();

		textEditor = this.GetChild<TextEditor>();

		textEditor.TextChanged += TextEditor_TextChanged;

		textEditor.SetLabelText("Search:");

		textEditor.SetFont(fontPath);

		localizations = new Dictionary<int, LocalizedTexts>();

		ReferenceTextEdit = (TextEdit)this.GetChildByName(nameof(ReferenceTextEdit));

		TargetLanguage = (ItemList) this.GetChildByName(nameof(TargetLanguage));

		Keys = (ItemList)this.GetChildByName(nameof(Keys));

		CurrentKey = (Label)this.GetChildByName(nameof(CurrentKey));

		CurrentKey.Visible = false;

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
	
	private void _on_Keys_item_selected(int index)
	{
		var key = Keys.GetItemText(index);

		CurrentKey.Text = key;

		CurrentKey.Visible = true;

		ReferenceTextEdit.Text = localizations[2].Texts[key];
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

			localizations.Add(i, new LocalizedTexts() { Index = i, Locale = languages[i], Texts = new Dictionary<string, string>() });
		}

		TargetLanguage.DisableTooltips();

		for (int i = 1; i < lines.Count; i++)
		{
			var line = lines[i].Split(";");

			ProcessLine(line);
		}

		Keys.DisableTooltips();

	}

	private void ProcessLine(string[] line)
	{
		Keys.AddItem(line.First());

		for (int c = 1; c < line.Length; c++)
		{
			if (localizations[c] == null) continue;
			
			if (!localizations[c].Texts.Any(t => t.Key == line.First()))
			{
				localizations[c].Texts.Add(line.First(), line[c]);
			}
		}
	}
}







