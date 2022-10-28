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

	ItemList TargetLanguage;

	ItemList ReferenceLanguage;

	TextEdit ReferenceTextEdit;

	TextEdit TargetTextEdit;

	TextEdit APIkey;

	ItemList Keys;

	TextEditor TextEditor;

	FileDialog FileDialog;

	CheckBox MockCheckBox;

	HTTPRequest hTTPRequest;

	static string fontPath = "res://fonts/Chinese.tres";

	public override void _Ready()
	{
		translationManager = new TranslationManager();

		FileDialog = this.GetChild<FileDialog>();

		TextEditor = this.GetChild<TextEditor>();

		TextEditor.TextChanged += TextEditor_TextChanged;

		TextEditor.SetLabelText("Search:");

		TextEditor.SetFont(fontPath);

		APIkey = (TextEdit)this.GetChildByName(nameof(APIkey));

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

		MockCheckBox = this.GetChild<CheckBox>();

		MockCheckBox.Pressed = true;

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
		translationManager.HandleAPIResponse(result, response_code, headers, body);
	}
	
	
	private void _on_Button2_button_up()
	{
		translationManager.CallAPI(hTTPRequest, APIkey.Text, MockCheckBox.Pressed);
	}
}














