using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;
using System.Text;
using System.Net;

public class MainView : Node2D
{
	ITranslationManager translationManager;

	Label CurrentKey;

	Label TargetTextLabel;

	Label ReferenceTextLabel;

	Label FoundLabel;

	ItemList TargetLanguage;

	ItemList ReferenceLanguage;

	TextEdit ReferenceTextEdit;

	TextEdit TargetTextEdit;

	TextEdit APIkey;

	ItemList Keys;

	TextEditor TextEditor;

	FileDialog DDOpen;

	FileDialog FDSaveAs;

	CheckBox MockCheckBox;

	LineEdit LineEdit;

	HTTPRequest hTTPRequest;

	List<string> foundKeys;

	int foundKeysIndex;

	static string fontPath = "res://fonts/Chinese.tres";

	public override void _Ready()
	{
		translationManager = new TranslationManager();

		TextEditor = this.GetChild<TextEditor>();

		TextEditor.TextChanged += TextEditor_TextChanged;

		TextEditor.SetLabelText("Search:");

		TextEditor.SetFont(fontPath);

		APIkey = (TextEdit)this.GetChildByName(nameof(APIkey));

		DDOpen = (FileDialog)this.GetChildByName(nameof(DDOpen));

		FDSaveAs = (FileDialog)this.GetChildByName(nameof(FDSaveAs));

		ReferenceTextEdit = (TextEdit)this.GetChildByName(nameof(ReferenceTextEdit));

		ReferenceTextEdit.SetLangugeSpecificTheme(fontPath);

		TargetTextEdit = (TextEdit)this.GetChildByName(nameof(TargetTextEdit));

		TargetTextEdit.SetLangugeSpecificTheme(fontPath);

		TargetLanguage = (ItemList) this.GetChildByName(nameof(TargetLanguage));

		ReferenceLanguage = (ItemList)this.GetChildByName(nameof(ReferenceLanguage));

		Keys = (ItemList)this.GetChildByName(nameof(Keys));

		CurrentKey = (Label)this.GetChildByName(nameof(CurrentKey));

		TargetTextLabel = (Label)this.GetChildByName(nameof(TargetTextLabel));

		ReferenceTextLabel = (Label)this.GetChildByName(nameof(ReferenceTextLabel));

		FoundLabel = (Label)this.GetChildByName(nameof(FoundLabel));

		MockCheckBox = this.GetChild<CheckBox>();

		LineEdit = this.GetChild<LineEdit>();

		MockCheckBox.Pressed = true;

		CurrentKey.Visible = false;

		TargetTextLabel.Visible = false;

		ReferenceTextLabel.Visible = false;

		DDOpen.Filters = new string[] { "*.csv" };

		foundKeys = new List<string>();

		hTTPRequest = this.GetChild<HTTPRequest>();
	}

	private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
	{
		DebugHelper.PrettyPrintVerbose($"Searched text: {e.NewText}", ConsoleColor.Green);

		var results = translationManager.GetKeysBySearchTerm(e.NewText);

		var count = results.Count();

		FoundLabel.Text = count.ToString();

		foundKeys = results;

		foundKeysIndex = 0;

		if (count > 0)
		{
			var indexOfFirstKey = translationManager.GetAllKeys().IndexOf(foundKeys[foundKeysIndex]);

			Keys.Select(indexOfFirstKey);

			_on_Keys_item_selected(indexOfFirstKey);
		}
	}

	private void _on_Button_button_up()
	{
		if (!DDOpen.Visible)
		{
			DDOpen.Popup_();
		}
	}
	
	private void _on_Keys_item_selected(int index)
	{
		translationManager.SelectKeyByIndex(index);

		CurrentKey.Text = translationManager.GetCurrentKey();

		CurrentKey.Visible = true;

		UpateTextFields();
	}

	private void UpateTextFields()
	{
		ReferenceTextEdit.Text = translationManager.GetReferenceText();

		TargetTextEdit.Text = translationManager.GetTargetText();
	}
	
	private void _on_FileDialog_file_selected(String path)
	{
		translationManager.LoadData(path);

		translationManager.GetAllLanguages().ForEach(l => { TargetLanguage.AddItem(l); ReferenceLanguage.AddItem(l); });

		translationManager.GetAllKeys().ForEach(k => Keys.AddItem(k));

		LineEdit.Text = path;

		TargetLanguage.DisableTooltips();

		ReferenceLanguage.DisableTooltips();

		Keys.DisableTooltips();
	}
	
		
	private void _on_TargetLanguage_item_selected(int index)
	{
		translationManager.SelectTargetLanguage(index);

		TargetTextLabel.Text = translationManager.GetTargetLanguage();

		TargetTextLabel.Visible = true;

		UpateTextFields();
	}

	private void _on_ReferenceLanguage_item_selected(int index)
	{
		translationManager.SelectReferenceLanguage(index);

		ReferenceTextLabel.Text = translationManager.GetReferenceLanguage();

		ReferenceTextLabel.Visible = true;

		UpateTextFields();
	}

	private void _on_HTTPRequest_request_completed(int result, int response_code, String[] headers, byte[] body)
	{
		TargetTextEdit.Text = translationManager.HandleAPIResponse(result, response_code, headers, body);
	}
	
	
	private void _on_Button2_button_up()
	{
		translationManager.CallAPI(hTTPRequest, APIkey.Text, MockCheckBox.Pressed);
	}
	
	
	private void _on_Button3_button_up()
	{
		translationManager.SaveData();
	}



	private void _on_Button4_button_up()
	{
		FDSaveAs.Popup_();
	}
	
	private void _on_FDSaveAs_file_selected(String path)
	{
		translationManager.SaveData(path);
	}


	private void _on_TargetTextEdit_text_changed()
	{
		translationManager.UpdateTargetLanguage(TargetTextEdit.Text);
	}
	
	
	private void _on_Button5_button_up()
	{
		if (foundKeys.Count == 0) return;

		foundKeysIndex = (foundKeysIndex + 1) % foundKeys.Count();

		var indexOfKey = translationManager.GetAllKeys().IndexOf(foundKeys[foundKeysIndex]);

		Keys.Select(indexOfKey);

		_on_Keys_item_selected(indexOfKey);

	}

}























