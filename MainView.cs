using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;
using System.Text;
using System.Net;

public class MainView : Node2D
{
	Label CurrentKey;

	Label TargetTextLabel;

	Label ReferenceTextLabel;

	ItemList TargetLanguage;

	ItemList ReferenceLanguage;

	TextEdit ReferenceTextEdit;

	TextEdit TargetTextEdit;

	TextEdit APIkey;

	ItemList Keys;

	TextEditor TextEditor;

	FileDialog FileDialog;

	static string fontPath = "res://fonts/Chinese.tres";

	Dictionary<int, LocalizedTexts> localizations;

	int targetTextIndex = 1;

	int referenceTextIndex = 2;

	string selectedKey;

	HTTPRequest hTTPRequest;

	public override void _Ready()
	{
		FileDialog = this.GetChild<FileDialog>();

		TextEditor = this.GetChild<TextEditor>();

		TextEditor.TextChanged += TextEditor_TextChanged;

		TextEditor.SetLabelText("Search:");

		TextEditor.SetFont(fontPath);

		localizations = new Dictionary<int, LocalizedTexts>();

		APIkey = (TextEdit)this.GetChildByName(nameof(APIkey));

		ReferenceTextEdit = (TextEdit)this.GetChildByName(nameof(ReferenceTextEdit));

		TargetTextEdit = (TextEdit)this.GetChildByName(nameof(TargetTextEdit));

		TargetLanguage = (ItemList) this.GetChildByName(nameof(TargetLanguage));

		ReferenceLanguage = (ItemList)this.GetChildByName(nameof(ReferenceLanguage));

		Keys = (ItemList)this.GetChildByName(nameof(Keys));

		CurrentKey = (Label)this.GetChildByName(nameof(CurrentKey));

		TargetTextLabel = (Label)this.GetChildByName(nameof(TargetTextLabel));

		ReferenceTextLabel = (Label)this.GetChildByName(nameof(ReferenceTextLabel));

		CurrentKey.Visible = false;

		TargetTextLabel.Visible = false;

		ReferenceTextLabel.Visible = false;

		FileDialog.Filters = new string[] { "*.csv" };

		hTTPRequest = this.GetChild<HTTPRequest>();
	}

	private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
	{
		DebugHelper.PrettyPrintVerbose($"Searched text: {e.NewText}", ConsoleColor.Green);

	}

	private void _on_Button_button_up()
	{
		if (!FileDialog.Visible)
		{
			FileDialog.Popup_();
		}
	}
	
	private void _on_Keys_item_selected(int index)
	{
		var key = Keys.GetItemText(index);

		selectedKey = key;

		CurrentKey.Text = key;

		CurrentKey.Visible = true;

		UpateTextFields();
	}

	private void UpateTextFields()
	{
		if (selectedKey == null) return;

		ReferenceTextEdit.Text = localizations[referenceTextIndex].Texts[selectedKey];

		TargetTextEdit.Text = localizations[targetTextIndex].Texts[selectedKey];
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

			ReferenceLanguage.AddItem(languages[i]);

			localizations.Add(i, new LocalizedTexts() { Index = i, Locale = languages[i], Texts = new Dictionary<string, string>() });
		}

		TargetLanguage.DisableTooltips();

		ReferenceLanguage.DisableTooltips();

		for (int i = 1; i < lines.Count; i++)
		{
			var line = lines[i].Split(";");

			ProcessLine(line);
		}

		Keys.DisableTooltips();

	}
	
		
	private void _on_TargetLanguage_item_selected(int index)
	{
		targetTextIndex = index +1;

		TargetTextLabel.Text = localizations[targetTextIndex].Locale;

		TargetTextLabel.Visible = true;

		UpateTextFields();
	}
	
	private void _on_HTTPRequest_request_completed(int result, int response_code, String[] headers, byte[] body)
	{
		JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));

		if (response_code == 200)
		{
			DebugHelper.PrettyPrintVerbose(json.Result);

			return;
		}

		DebugHelper.PrettyPrintVerbose($"Http response code: {response_code}");
	}
	
	
	private void _on_Button2_button_up()
	{
		
		var auth = $"Authorization: DeepL-Auth-Key {APIkey.Text}";
		var contentTpye = "Content-Type: application/x-www-form-urlencoded";


		hTTPRequest.Request("https://api-free.deepl.com/v2/translate", new string[] { auth, contentTpye }, true, HTTPClient.Method.Post, "text=Hello%2C%20world!&target_lang=DE");

	}


	private void _on_ReferenceLanguage_item_selected(int index)
	{
		referenceTextIndex = index +1;

		ReferenceTextLabel.Text = localizations[referenceTextIndex].Locale;

		ReferenceTextLabel.Visible = true;

		UpateTextFields();
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














