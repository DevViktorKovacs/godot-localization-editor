using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;

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

	FileDialog FDMerge;

	FileDialog FDExport;

	CheckBox MockCheckBox;

	LineEdit LEFilePath;

	LineEdit LEAddKey;

	HTTPRequest hTTPRequest;

	List<string> foundKeys;

	int foundKeysIndex;

	static string fontPath = "res://fonts/Chinese.tres";

	public override void _Ready()
	{
		translationManager = new TranslationManager();

		APIkey = (TextEdit)this.GetChildByName(nameof(APIkey));

		DDOpen = (FileDialog)this.GetChildByName(nameof(DDOpen));

		FDSaveAs = (FileDialog)this.GetChildByName(nameof(FDSaveAs));

		FDMerge = (FileDialog)this.GetChildByName(nameof(FDMerge));

		FDExport = (FileDialog)this.GetChildByName(nameof(FDExport));

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

		LEFilePath = (LineEdit)this.GetChildByName(nameof(LEFilePath));

		LEAddKey = (LineEdit)this.GetChildByName(nameof(LEAddKey));

		TextEditor = this.GetChild<TextEditor>();

		TextEditor.TextChanged += TextEditor_TextChanged;

		TextEditor.SetLabelText("Search:");

		TextEditor.SetFont(fontPath);

		MockCheckBox = this.GetChild<CheckBox>();

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
		DDOpen.Popup_();
	}

	private void _on_Button4_button_up()
	{
		FDSaveAs.Popup_();
	}


	private void _on_Button8_button_up()
	{
		FDMerge.Popup_();
	}

	private void _on_Button9_button_up()
	{
		FDExport.Popup_();
	}
	
	private void _on_FileDialog_file_selected(String path)
	{
		translationManager.LoadData(path);

		translationManager.GetAllLanguages().ForEach(l => { TargetLanguage.AddItem(l); ReferenceLanguage.AddItem(l); });

		translationManager.GetAllKeys().ForEach(k => Keys.AddItem(k));

		LEFilePath.Text = path;

		TargetLanguage.DisableTooltips();

		ReferenceLanguage.DisableTooltips();

		Keys.DisableTooltips();
	}

	private void _on_FDMerge_file_selected(String path)
	{
		translationManager.MergeFiles(path);

		TargetLanguage.Clear();

		ReferenceLanguage.Clear();

		translationManager.GetAllLanguages().ForEach(l => { TargetLanguage.AddItem(l); ReferenceLanguage.AddItem(l); });

		Keys.Clear();

		translationManager.GetAllKeys().ForEach(k => Keys.AddItem(k));
	}

	private void _on_FDExport_file_selected(String path)
	{
		translationManager.ExportPartial(path);
	}


	private void _on_FDSaveAs_file_selected(String path)
	{
		translationManager.SaveData(path);
	}

	private void _on_Keys_item_selected(int index)
	{
		translationManager.SelectKeyByIndex(index);

		CurrentKey.Text = translationManager.GetCurrentKey();

		CurrentKey.Visible = true;

		UpateTextFields();
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

	private void UpateTextFields()
	{
		ReferenceTextEdit.Text = translationManager.GetReferenceText();

		TargetTextEdit.Text = translationManager.GetTargetText();
	}

	private void _on_HTTPRequest_request_completed(int result, int response_code, String[] headers, byte[] body)
	{
		TargetTextEdit.Text = translationManager.HandleAPIResponse(result, response_code, headers, body);
	}
	
	
	private void _on_Button2_button_up()
	{
		var trParams = new TranslationRequestParams()
		{
			APIKey = APIkey.Text,

			Mock = MockCheckBox.Pressed,

			Text = ReferenceTextEdit.Text,
		};

		translationManager.CallAPI(hTTPRequest, trParams);
	}
	
	
	private void _on_Button3_button_up()
	{
		translationManager.SaveData();
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
	
	private void _on_Button6_button_up()
	{
		translationManager.AddLanguage(LEAddKey.Text);

		TargetLanguage.Clear();

		ReferenceLanguage.Clear();

		translationManager.GetAllLanguages().ForEach(l => { TargetLanguage.AddItem(l); ReferenceLanguage.AddItem(l); });
	}

	private void _on_Button7_button_up()
	{
		translationManager.AddNewKey(LEAddKey.Text);

		Keys.Clear();

		translationManager.GetAllKeys().ForEach(k => Keys.AddItem(k));
	}

}







