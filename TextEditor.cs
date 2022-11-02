using Godot;
using godotlocalizationeditor;
using System;

public class TextEditor : Control
{
	public event EventHandler<TextChangedEventArgs> TextChanged;

	public event EventHandler<TextChangedEventArgs> FocusExited;

	TextEdit textEditor;

	Timer timer;

	Label label;

	int maxLength = 66;

	public bool FreeMode { get; set; }
	public string Text { get => textEditor.Text; set => textEditor.Text = value; }
	public override void _Ready()
	{
		textEditor = this.GetChild<TextEdit>();

		timer = this.GetChild<Timer>();

		label = this.GetChild<Label>();

		FreeMode = false;
	}

	public void SetLabelText(string text)
	{
		label.Text = text;
	}

	public void SetFont(string themePath)
	{
		textEditor.SetLangugeSpecificTheme(themePath);
	}

	public void RemoveFont()
	{
		label.AddFontOverride("font", null);
	}

	public void SetFont(string fontPath, int fontSize)
	{
		label.SetFont(fontPath, fontSize);
	}

	private void _on_TextEdit_breakpoint_toggled(int row)
	{
		DebugHelper.PrettyPrint($"breakpoint_toggled( {row}", ConsoleColor.Yellow);
	}

	private void _on_TextEdit_text_changed()
	{

		if (textEditor.GetLineCount()>1)
		{
			EventHandler<TextChangedEventArgs> handler = TextChanged;

			if (handler != null)
			{
				handler(this, new TextChangedEventArgs() { NewText = textEditor.GetLine(0).Truncate(maxLength)});
			}

			timer.Start();

			textEditor.Text = textEditor.GetLine(0);
		}


		if (textEditor.GetLine(0).Length >= maxLength - 1)
		{
			textEditor.Text = textEditor.GetLine(0).Truncate(maxLength);
		}

		textEditor.CursorSetColumn(textEditor.Text.Length);
	}
	
	private void _on_TextEdit_focus_entered()
	{

	}

	private void _on_TextEdit_focus_exited()
	{

		EventHandler<TextChangedEventArgs> handler = FocusExited;

		if (handler != null)
		{
			handler(this, new TextChangedEventArgs() { NewText = textEditor.GetLine(0).Truncate(maxLength) });
		}
	}

	private void _on_Timer_timeout()
	{
		timer.Stop();

		textEditor.ReleaseFocus();
	}
}

public class TextChangedEventArgs : EventArgs
{
	public string NewText { get; set; }
}


















