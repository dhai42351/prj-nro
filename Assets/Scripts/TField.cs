using System;
using System.Threading;
using UnityEngine;

public class TField : IActionListener
{
	public bool isFocus;

	public int x;

	public int y;

	public int width;

	public int height;

	public bool lockArrow;

	public bool justReturnFromTextBox;

	public bool paintFocus = true;

	public const sbyte KEY_LEFT = 14;

	public const sbyte KEY_RIGHT = 15;

	public const sbyte KEY_CLEAR = 19;

	public static int typeXpeed = 2;

	private static readonly int[] MAX_TIME_TO_CONFIRM_KEY = new int[7] { 30, 14, 11, 9, 6, 4, 2 };

	private static int CARET_HEIGHT = 0;

	private static readonly int CARET_WIDTH = 1;

	private static readonly int CARET_SHOWING_TIME = 5;

	private static readonly int TEXT_GAP_X = 4;

	private static readonly int MAX_SHOW_CARET_COUNER = 10;

	public static readonly int INPUT_TYPE_ANY = 0;

	public static readonly int INPUT_TYPE_NUMERIC = 1;

	public static readonly int INPUT_TYPE_PASSWORD = 2;

	public static readonly int INPUT_ALPHA_NUMBER_ONLY = 3;

	private static string[] print = new string[12]
	{
		" 0", ".,@?!_1\"/$-():*+<=>;%&~#%^&*{}[];'/1", "abc2áàảãạâấầẩẫậăắằẳẵặ2", "def3đéèẻẽẹêếềểễệ3", "ghi4íìỉĩị4", "jkl5", "mno6óòỏõọôốồổỗộơớờởỡợ6", "pqrs7", "tuv8úùủũụưứừửữự8", "wxyz9ýỳỷỹỵ9",
		"*", "#"
	};

	private static string[] printA = new string[12]
	{
		"0", "1", "abc2", "def3", "ghi4", "jkl5", "mno6", "pqrs7", "tuv8", "wxyz9",
		"0", "0"
	};

	private static string[] printBB = new string[17]
	{
		" 0", "er1", "ty2", "ui3", "df4", "gh5", "jk6", "cv7", "bn8", "m9",
		"0", "0", "qw!", "as?", "zx", "op.", "l,"
	};

	private string text = string.Empty;

	private string passwordText = string.Empty;

	private string paintedText = string.Empty;

	private int caretPos;

	private int counter;

	private int maxTextLenght = 500;

	private int offsetX;

	private static int lastKey = -1984;

	private int keyInActiveState;

	private int indexOfActiveChar;

	private int showCaretCounter = MAX_SHOW_CARET_COUNER;

	private int inputType = INPUT_TYPE_ANY;

	public static bool isQwerty = true;

	public static int typingModeAreaWidth;

	public static int mode = 0;

	public static long timeChangeMode;

	public static readonly string[] modeNotify = new string[4] { "abc", "Abc", "ABC", "123" };

	public static readonly int NOKIA = 0;

	public static readonly int MOTO = 1;

	public static readonly int ORTHER = 2;

	public static readonly int BB = 3;

	public static int changeModeKey = 11;

	public static readonly sbyte abc = 0;

	public static readonly sbyte Abc = 1;

	public static readonly sbyte ABC = 2;

	public static readonly sbyte number123 = 3;

	public static TField currentTField;

	public bool isTfield;

	public bool isPaintMouse = true;

	public string name = string.Empty;

	public string title = string.Empty;

	public string strInfo;

	public Command cmdClear;

	public Command cmdDoneAction;

	private mScreen parentScr;

	private int timeDelayKyCode;

	private int holdCount;

	public static int changeDau;

	private int indexDau = -1;

	private int indexTemplate;

	private int indexCong;

	private long timeDau;

	private static string printDau = "aáàảãạâấầẩẫậăắằẳẵặeéèẻẽẹêếềểễệiíìỉĩịoóòỏõọôốồổỗộơớờởỡợuúùủũụưứừửữựyýỳỷỹỵ";

	public static Image imgTf;

	public int timePutKeyClearAll;

	public int timeClearFirt;

	public bool isPaintCarret;

	public bool showSubTextField = true;

	public static TouchScreenKeyboard kb;

	public static int[][] BBKEY = new int[17][]
	{
		new int[2] { 32, 48 },
		new int[2] { 49, 69 },
		new int[2] { 50, 84 },
		new int[2] { 51, 85 },
		new int[2] { 52, 68 },
		new int[2] { 53, 71 },
		new int[2] { 54, 74 },
		new int[2] { 55, 67 },
		new int[2] { 56, 66 },
		new int[2] { 57, 77 },
		new int[2] { 42, 128 },
		new int[2] { 35, 137 },
		new int[2] { 33, 113 },
		new int[2] { 63, 97 },
		new int[3] { 64, 121, 122 },
		new int[2] { 46, 111 },
		new int[2] { 44, 108 }
	};

	private static string[] telexMap = new string[13]
	{
		"aw|ă", "aa|â", "dd|đ", "ow|ơ", "oo|ô", "ee|ê", "uw|ư", "w|ư", "s|\u0301", "f|\u0300",
		"r|\u0309", "x|\u0303", "j|\u0323"
	};

	private string telexBuffer = "";

	private const int TELEX_BUFFER_LENGTH = 5;

	public TField()
	{
		text = string.Empty;
		init();
	}

	public void doChangeToTextBox()
	{
	}

	public void init()
	{
		CARET_HEIGHT = mScreen.ITEM_HEIGHT + 1;
		cmdClear = new Command(mResources.DELETE, this, 1000, null);
		if (Main.isPC)
		{
			typeXpeed = 0;
		}
		if (imgTf == null)
		{
			imgTf = GameCanvas.loadImage("/mainImage/myTexture2dtf.png");
		}
	}

	public void clearKeyWhenPutText(int keyCode)
	{
		if (keyCode == -8 && timeDelayKyCode <= 0)
		{
			if (timeDelayKyCode <= 0)
			{
				timeDelayKyCode = 1;
			}
			clear();
		}
	}

	public void clearAllText()
	{
		text = string.Empty;
		if (kb != null)
		{
			kb.text = string.Empty;
		}
		caretPos = 0;
		setOffset(0);
		setPasswordTest();
	}

	public void clear()
	{
		if (caretPos > 0 && text.Length > 0)
		{
			text = text.Substring(0, caretPos - 1);
			caretPos--;
			setOffset(0);
			setPasswordTest();
			if (kb != null)
			{
				kb.text = text;
			}
		}
	}

	public void clearAll()
	{
		if (caretPos > 0 && text.Length > 0)
		{
			text = text.Substring(0, text.Length - 1);
			caretPos--;
			setOffset();
			setPasswordTest();
			setFocusWithKb(isFocus: true);
			if (kb != null)
			{
				kb.text = string.Empty;
			}
		}
	}

	public void setOffset()
	{
		if (paintedText != null && mFont.tahoma_8b != null)
		{
			if (inputType == INPUT_TYPE_PASSWORD)
			{
				paintedText = passwordText;
			}
			else
			{
				paintedText = text;
			}
			if (offsetX < 0 && mFont.tahoma_8b.getWidth(paintedText) + offsetX < width - TEXT_GAP_X - 13 - typingModeAreaWidth)
			{
				offsetX = width - 10 - typingModeAreaWidth - mFont.tahoma_8b.getWidth(paintedText);
			}
			if (offsetX + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) <= 0)
			{
				offsetX = -mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos));
				offsetX += 40;
			}
			else if (offsetX + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) >= width - 12 - typingModeAreaWidth)
			{
				offsetX = width - 10 - typingModeAreaWidth - mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) - 2 * TEXT_GAP_X;
			}
			if (offsetX > 0)
			{
				offsetX = 0;
			}
		}
	}

	private void keyPressedAny(int keyCode)
	{
		string[] array = ((inputType != INPUT_TYPE_PASSWORD && inputType != INPUT_ALPHA_NUMBER_ONLY) ? print : printA);
		if (keyCode == lastKey)
		{
			indexOfActiveChar = (indexOfActiveChar + 1) % array[keyCode - 48].Length;
			char c = array[keyCode - 48][indexOfActiveChar];
			object obj = ((mode == 0) ? char.ToLower(c) : ((mode == 1) ? char.ToUpper(c) : ((mode != 2) ? array[keyCode - 48][array[keyCode - 48].Length - 1] : char.ToUpper(c))));
			string text = this.text.Substring(0, caretPos - 1) + obj;
			if (caretPos < this.text.Length)
			{
				text += this.text.Substring(caretPos, this.text.Length);
			}
			this.text = text;
			keyInActiveState = MAX_TIME_TO_CONFIRM_KEY[typeXpeed];
			setPasswordTest();
		}
		else if (this.text.Length < maxTextLenght)
		{
			if (mode == 1 && lastKey != -1984)
			{
				mode = 0;
			}
			indexOfActiveChar = 0;
			char c2 = array[keyCode - 48][indexOfActiveChar];
			object obj = ((mode == 0) ? char.ToLower(c2) : ((mode == 1) ? char.ToUpper(c2) : ((mode != 2) ? array[keyCode - 48][array[keyCode - 48].Length - 1] : char.ToUpper(c2))));
			string text2 = this.text.Substring(0, caretPos) + obj;
			if (caretPos < this.text.Length)
			{
				text2 += this.text.Substring(caretPos, this.text.Length);
			}
			this.text = text2;
			keyInActiveState = MAX_TIME_TO_CONFIRM_KEY[typeXpeed];
			caretPos++;
			setPasswordTest();
			setOffset();
		}
		lastKey = keyCode;
	}

	private void keyPressedAscii(int keyCode)
	{
		if ((keyCode >= 33 && keyCode <= 47) || (keyCode >= 58 && keyCode <= 64) || (keyCode >= 91 && keyCode <= 96) || (keyCode >= 123 && keyCode <= 126))
		{
			string text = this.text.Substring(0, caretPos) + (char)keyCode;
			if (caretPos < this.text.Length)
			{
				text += this.text.Substring(caretPos, this.text.Length - caretPos);
			}
			this.text = text;
			caretPos++;
			setPasswordTest();
			setOffset(0);
			if (kb != null)
			{
				kb.text = this.text;
			}
		}
		else
		{
			if ((inputType == INPUT_TYPE_PASSWORD || inputType == INPUT_ALPHA_NUMBER_ONLY) && (keyCode < 48 || keyCode > 57) && (keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 122))
			{
				return;
			}
			if (this.text.Length < maxTextLenght)
			{
				char c = (char)keyCode;
				string text2 = this.text.Substring(0, caretPos) + c;
				if (caretPos < this.text.Length)
				{
					text2 += this.text.Substring(caretPos, this.text.Length - caretPos);
				}
				this.text = text2;
				caretPos++;
				setPasswordTest();
				setOffset(0);
				if (ModFunc.isVietnamese && Main.isPC && inputType == INPUT_TYPE_ANY)
				{
					processTelex(c);
				}
			}
			if (kb != null)
			{
				kb.text = this.text;
			}
		}
	}

	private void setDau()
	{
		timeDau = Environment.TickCount / 100;
		if (indexDau == -1)
		{
			for (int num = caretPos; num > 0; num--)
			{
				char c = this.text[num - 1];
				for (int i = 0; i < printDau.Length; i++)
				{
					char c2 = printDau[i];
					if (c == c2)
					{
						indexTemplate = i;
						indexCong = 0;
						indexDau = num - 1;
						return;
					}
				}
			}
			indexDau = -1;
		}
		else
		{
			indexCong++;
			if (indexCong >= 6)
			{
				indexCong = 0;
			}
			string text = this.text.Substring(0, indexDau);
			string text2 = this.text.Substring(indexDau + 1);
			string text3 = printDau.Substring(indexTemplate + indexCong, 1);
			this.text = text + text3 + text2;
		}
	}

	public bool keyPressed(int keyCode)
	{
		if (Main.isPC && keyCode == -8)
		{
			clearKeyWhenPutText(-8);
			return true;
		}
		if (keyCode == 8 || keyCode == -8 || keyCode == 204)
		{
			clear();
			return true;
		}
		if (isQwerty && keyCode >= 32)
		{
			keyPressedAscii(keyCode);
			return false;
		}
		if (keyCode == changeDau && inputType == INPUT_TYPE_ANY)
		{
			setDau();
			return false;
		}
		if (keyCode == 42)
		{
			keyCode = 58;
		}
		if (keyCode == 35)
		{
			keyCode = 59;
		}
		if (keyCode >= 48 && keyCode <= 59)
		{
			if (inputType == INPUT_TYPE_ANY || inputType == INPUT_TYPE_PASSWORD || inputType == INPUT_ALPHA_NUMBER_ONLY)
			{
				keyPressedAny(keyCode);
			}
			else if (inputType == INPUT_TYPE_NUMERIC)
			{
				keyPressedAscii(keyCode);
				keyInActiveState = 1;
			}
		}
		else
		{
			indexOfActiveChar = 0;
			lastKey = -1984;
			if (keyCode == 14 && !lockArrow)
			{
				if (caretPos > 0)
				{
					caretPos--;
					setOffset(0);
					showCaretCounter = MAX_SHOW_CARET_COUNER;
					return false;
				}
			}
			else if (keyCode == 15 && !lockArrow)
			{
				if (caretPos < text.Length)
				{
					caretPos++;
					setOffset(0);
					showCaretCounter = MAX_SHOW_CARET_COUNER;
					return false;
				}
			}
			else
			{
				if (keyCode == 19)
				{
					clear();
					return false;
				}
				lastKey = keyCode;
			}
		}
		return true;
	}

	public void setOffset(int index)
	{
		if (inputType == INPUT_TYPE_PASSWORD)
		{
			paintedText = passwordText;
		}
		else
		{
			paintedText = text;
		}
		int num = mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos));
		switch (index)
		{
		case -1:
			if (num + offsetX < 15 && caretPos > 0 && caretPos < paintedText.Length)
			{
				offsetX += mFont.tahoma_8b.getWidth(paintedText.Substring(caretPos, 1));
			}
			break;
		case 1:
			if (num + offsetX > width - 25 && caretPos < paintedText.Length && caretPos > 0)
			{
				offsetX -= mFont.tahoma_8b.getWidth(paintedText.Substring(caretPos - 1, 1));
			}
			break;
		default:
			offsetX = -(num - (width - 12));
			break;
		}
		if (offsetX > 0)
		{
			offsetX = 0;
		}
		else if (offsetX < 0)
		{
			int num2 = mFont.tahoma_8b.getWidth(paintedText) - (width - 12);
			if (offsetX < -num2)
			{
				offsetX = -num2;
			}
		}
	}

	public void paintInputTf(mGraphics g, bool iss, int x, int y, int w, int h, int xText, int yText, string text, string info)
	{
		g.setColor(0);
		if (iss)
		{
			g.drawRegion(imgTf, 0, 81, 29, 27, 0, x, y, 0);
			g.drawRegion(imgTf, 0, 135, 29, 27, 0, x + w - 29, y, 0);
			g.drawRegion(imgTf, 0, 108, 29, 27, 0, x + w - 58, y, 0);
			for (int i = 0; i < (w - 58) / 29; i++)
			{
				g.drawRegion(imgTf, 0, 108, 29, 27, 0, x + 29 + i * 29, y, 0);
			}
		}
		else
		{
			g.drawRegion(imgTf, 0, 0, 29, 27, 0, x, y, 0);
			g.drawRegion(imgTf, 0, 54, 29, 27, 0, x + w - 29, y, 0);
			g.drawRegion(imgTf, 0, 27, 29, 27, 0, x + w - 58, y, 0);
			for (int j = 0; j < (w - 58) / 29; j++)
			{
				g.drawRegion(imgTf, 0, 27, 29, 27, 0, x + 29 + j * 29, y, 0);
			}
		}
		g.setClip(x + 3, y + 1, w - 4, h);
		if (text != null && !text.Equals(string.Empty))
		{
			mFont.tahoma_8b.drawString(g, text, xText, yText, 0);
		}
		else if (info != null)
		{
			if (iss)
			{
				mFont.tahoma_7b_focus.drawString(g, info, xText, yText, 0);
			}
			else
			{
				mFont.tahoma_7b_unfocus.drawString(g, info, xText, yText, 0);
			}
		}
	}

	public void paint(mGraphics g)
	{
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		bool flag = isFocused();
		if (inputType == INPUT_TYPE_PASSWORD)
		{
			paintedText = passwordText;
		}
		else
		{
			paintedText = text;
		}
		paintInputTf(g, flag, x, y - 1, width, height + 5, TEXT_GAP_X + offsetX + x + 1, y + (height - mFont.tahoma_8b.getHeight()) / 2 + 2, paintedText, name);
		g.setClip(x + 3, y + 1, width - 4, height - 2);
		g.setColor(0);
		if (flag && isPaintMouse && isPaintCarret)
		{
			if (keyInActiveState == 0 && (showCaretCounter > 0 || counter / CARET_SHOWING_TIME % 4 == 0))
			{
				g.setColor(7999781);
				g.fillRect(TEXT_GAP_X + 1 + offsetX + x + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos) + "a") - CARET_WIDTH - mFont.tahoma_8b.getWidth("a"), y + (height - CARET_HEIGHT) / 2 + 5, CARET_WIDTH, CARET_HEIGHT);
			}
			GameCanvas.resetTrans(g);
			if (text != null && text.Length > 0 && GameCanvas.isTouch)
			{
				g.drawImage(GameCanvas.imgClear, x + width - 13, y + height / 2 + 3, mGraphics.VCENTER | mGraphics.HCENTER);
			}
		}
	}

	private bool isFocused()
	{
		return isFocus;
	}

	private void setPasswordTest()
	{
		if (inputType == INPUT_TYPE_PASSWORD)
		{
			passwordText = string.Empty;
			for (int i = 0; i < text.Length; i++)
			{
				passwordText += "*";
			}
			if (keyInActiveState > 0 && caretPos > 0)
			{
				passwordText = passwordText.Substring(0, caretPos - 1) + text[caretPos - 1] + passwordText.Substring(caretPos, passwordText.Length);
			}
		}
	}

	public void update()
	{
		if (ModFunc.isOpenAccMAnager)
		{
			return;
		}
		isPaintCarret = true;
		if (Main.isPC)
		{
			if (timeDelayKyCode > 0)
			{
				timeDelayKyCode--;
			}
			if (timeDelayKyCode <= 0)
			{
				timeDelayKyCode = 0;
			}
		}
		if (kb != null && currentTField == this)
		{
			if (kb.text.Length < 40 && isFocus)
			{
				setText(kb.text);
			}
			if (kb.status == TouchScreenKeyboard.Status.Done && cmdDoneAction != null)
			{
				cmdDoneAction.performAction();
			}
		}
		counter++;
		if (keyInActiveState > 0)
		{
			keyInActiveState--;
			if (keyInActiveState == 0)
			{
				indexOfActiveChar = 0;
				if (mode == 1 && lastKey != changeModeKey && isFocus)
				{
					mode = 0;
				}
				lastKey = -1984;
				setPasswordTest();
			}
		}
		if (showCaretCounter > 0)
		{
			showCaretCounter--;
		}
		if (GameCanvas.isPointerJustRelease)
		{
			setTextBox();
		}
		if (indexDau != -1 && Environment.TickCount / 100 - timeDau > 5)
		{
			indexDau = -1;
		}
	}

	public void setTextBox()
	{
		if (GameCanvas.isPointerHoldIn(x + width - 20, y, 40, height))
		{
			clearAllText();
			isFocus = true;
		}
		else if (GameCanvas.isPointerHoldIn(x, y, width - 20, height))
		{
			setFocusWithKb(isFocus: true);
		}
		else
		{
			setFocus(isFocus: false);
		}
	}

	public void setFocus(bool isFocus)
	{
		if (this.isFocus != isFocus)
		{
			mode = 0;
		}
		lastKey = -1984;
		timeChangeMode = (int)(DateTime.Now.Ticks / 1000);
		this.isFocus = isFocus;
		if (isFocus)
		{
			currentTField = this;
			if (kb != null)
			{
				kb.text = currentTField.text;
			}
		}
	}

	public void setFocusWithKb(bool isFocus)
	{
		if (this.isFocus != isFocus)
		{
			mode = 0;
		}
		lastKey = -1984;
		timeChangeMode = (int)(DateTime.Now.Ticks / 1000);
		this.isFocus = isFocus;
		if (isFocus)
		{
			currentTField = this;
		}
		else if (currentTField == this)
		{
			currentTField = null;
		}
		if (!(Thread.CurrentThread.Name == Main.mainThreadName) || currentTField == null)
		{
			return;
		}
		this.isFocus = true;
		TouchScreenKeyboard.hideInput = !currentTField.showSubTextField;
		TouchScreenKeyboardType t = TouchScreenKeyboardType.ASCIICapable;
		if (inputType == INPUT_TYPE_NUMERIC)
		{
			t = TouchScreenKeyboardType.NumberPad;
		}
		bool type = false;
		if (inputType == INPUT_TYPE_PASSWORD)
		{
			type = true;
		}
		if (!Main.isPC || Main.isIPhone)
		{
			kb = TouchScreenKeyboard.Open(currentTField.text, t, autocorrection: false, multiline: false, type, alert: false, currentTField.name);
			if (kb != null)
			{
				kb.text = currentTField.text;
			}
		}
	}

	public string getText()
	{
		return text;
	}

	public void clearKb()
	{
		if (kb != null)
		{
			kb.text = string.Empty;
		}
	}

	public void setText(string text)
	{
		if (text != null)
		{
			lastKey = -1984;
			keyInActiveState = 0;
			indexOfActiveChar = 0;
			this.text = text;
			paintedText = text;
			setPasswordTest();
			caretPos = text.Length;
			setOffset();
		}
	}

	public void setMaxTextLenght(int maxTextLenght)
	{
		this.maxTextLenght = maxTextLenght;
	}

	public void setIputType(int iputType)
	{
		inputType = iputType;
		setMaxTextLenght(500);
	}

	public void perform(int idAction, object p)
	{
		if (idAction == 1000)
		{
			clear();
		}
	}

	private void processTelex(char c)
	{
		telexBuffer += c;
		if (telexBuffer.Length > 5)
		{
			telexBuffer = telexBuffer.Substring(telexBuffer.Length - 5);
		}
		string[] array = telexMap;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('|');
			string input = array2[0];
			string output = array2[1];
			if (!telexBuffer.EndsWith(input))
			{
				continue;
			}
			int pos = caretPos - input.Length;
			if (pos < 0)
			{
				continue;
			}
			string before = text.Substring(0, pos);
			string after = ((caretPos < text.Length) ? text.Substring(caretPos) : "");
			if (input.Length == 1 && "sfrxj".Contains(input))
			{
				if (pos > 0)
				{
					char prevChar = text[pos - 1];
					char newChar = addMark(prevChar, output[0]);
					if (newChar == prevChar)
					{
						text = before + input + after;
						caretPos = pos + 1;
					}
					else
					{
						text = before.Substring(0, before.Length - 1) + newChar + after;
						caretPos = pos;
					}
					telexBuffer = "";
					setPasswordTest();
					setOffset();
					break;
				}
				continue;
			}
			if (pos > 0 && input.Length == 2 && "âăêôơư".Contains(text[pos - 1].ToString()))
			{
				text = before + input[1] + after;
				caretPos = pos + 1;
				telexBuffer = "";
				setPasswordTest();
				setOffset();
			}
			else
			{
				text = before + output + after;
				caretPos = pos + 1;
				telexBuffer = "";
				setPasswordTest();
				setOffset();
			}
			break;
		}
	}

	private char addMark(char c, char mark)
	{
		if ("aáàảãạâấầẩẫậăắằẳẵặeéèẻẽẹêếềểễệiíìỉĩịoóòỏõọôốồổỗộơớờởỡợuúùủũụưứừửữựyýỳỷỹỵ".IndexOf(c) < 0)
		{
			return c;
		}
		char baseVowel = c;
		int markType = getMarkType(mark);
		string[] array;
		if ("âăêôơư".Contains(baseVowel.ToString()))
		{
			array = new string[6] { "âấầẩẫậ", "ăắằẳẵặ", "êếềểễệ", "ôốồổỗộ", "ơớờởỡợ", "ưứừửữự" };
			foreach (string specialMark in array)
			{
				if (specialMark[0] == baseVowel)
				{
					return specialMark[markType + 1];
				}
			}
		}
		array = new string[6] { "aáàảãạ", "eéèẻẽẹ", "iíìỉĩị", "oóòỏõọ", "uúùủũụ", "yýỳỷỹỵ" };
		foreach (string markSet in array)
		{
			if (markSet.Contains(baseVowel))
			{
				return markSet[markType + 1];
			}
		}
		return c;
	}

	private int getMarkType(char mark)
	{
		return mark switch
		{
			'\u0301' => 0, 
			'\u0300' => 1, 
			'\u0309' => 2, 
			'\u0303' => 3, 
			'\u0323' => 4, 
			_ => 0, 
		};
	}
}
