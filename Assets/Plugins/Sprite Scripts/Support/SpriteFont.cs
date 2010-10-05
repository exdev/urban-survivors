//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


/// <remarks>
/// A struct that holds important information
/// about a sprite-based character.
/// </remarks>
public struct SpriteChar
{
	/// <summary>
	/// The "id" of the character (usually the ASCII value).
	/// </summary>
	public int id;

	/// <summary>
	/// The UV coords of the character.
	/// </summary>
	public Rect UVs;

	/// <summary>
	/// The offset, in pixels, of the char's mesh from its "zero-point".
	/// </summary>
	public float xOffset, yOffset;

	/// <summary>
	/// How far to move, in pixels, from this char to position the next one.
	/// </summary>
	public float xAdvance;
	
	/// <summary>
	/// The map of kernings to use for preceding characters.
	/// The key is the previous character, and the value is 
	/// the kerning amount, in pixels.
	/// </summary>
	public Dictionary<char, float> kernings;

	/// <summary>
	/// Gets the kerning amount given the previous character.
	/// </summary>
	/// <param name="prevChar">The character that precedes this one.</param>
	/// <returns>The kerning amount, in pixels.</returns>
	public float GetKerning(char prevChar)
	{
		if(kernings == null)
			return 0;

		float amount = 0;
		kernings.TryGetValue(prevChar, out amount);
		return amount;
	}
}


/// <remarks>
/// A class that holds information about a font
/// intended for use with SpriteText.
/// </remarks>
public class SpriteFont
{
	// Parsing delegate type:
	protected delegate void ParserDel(string line);

	/// <summary>
	/// The TextAsset that defines the font.
	/// </summary>
	public TextAsset fontDef;

	// Maps character IDs to that character's
	// index in the array.
#if UNITY_IPHONE && !UNITY_3_0
	protected Hashtable charMap = new Hashtable();
#else
	protected Dictionary<char, int>charMap = new Dictionary<char, int>();
#endif

	// Our characters:
	protected SpriteChar[] chars;

 	protected int lineHeight;
	/// <summary>
	/// The default height, in pixels, between lines.
	/// </summary>
	public int LineHeight
	{
		get { return lineHeight; }
		set { lineHeight = value; }
	}

	protected int baseHeight;
	/// <summary>
	///	The distance, in pixels, from the absolute top
	/// of a line to the baseline:
	/// </summary>
	public int BaseHeight
	{
		get { return baseHeight; }
	}
	// The width and height of the font atlas.
	protected int texWidth, texHeight;

	/// <summary>
	/// The name of the font face.
	/// </summary>
 	protected string face;

	protected int pxSize;
	/// <summary>
	///	The size (height) of the font, in pixels.
	/// This is the height, in pixels of a full-height
	/// character.
	/// </summary>
	public int PixelSize
	{
		get { return pxSize; }
	}

	// Working vars:
	int kerningsCount;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="def">TextAsset that defines the font.</param>
	public SpriteFont(TextAsset def)
	{
		Load(def);
	}

	/// <summary>
	/// Loads a font from the specified font definition TextAsset
	/// and adds it to the font store.
	/// </summary>
	/// <param name="fontDef">The TextAsset that defines the font.</param>
	public void Load(TextAsset def)
	{
		if (def == null)
			return;

		int pos, c=0;

		fontDef = def;

		string[] lines = fontDef.text.Split(new char[]{'\n'});

		pos = ParseSection("info", lines, HeaderParser, 0);
		pos = ParseSection("common", lines, CommonParser, pos);
		pos = ParseSection("chars count", lines, CharCountParser, pos);

		while (pos < lines.Length && c < chars.Length)
			if (CharParser(lines[pos++], c))
				++c; // Tally another character

		pos = ParseSection("kernings count", lines, KerningCountParser, pos);

		c = 0;
		while (pos < lines.Length && c < kerningsCount)
			if (KerningParser(lines[pos++]))
				++c; // Tally another kerning
	}

	// Finds a line that starts with "tag" and passes that line
	// to the specified parsing delegate.
	int ParseSection(string tag, string[] lines, ParserDel parser, int pos)
	{
		for (; pos < lines.Length; ++pos)
		{
			string line = lines[pos].Trim();

			if (line.Length < 1)
				continue;

			if (line.StartsWith(tag))
			{
				parser(line);
				return ++pos;
			}
		}

		return pos;
	}

	// Returns the index of the field matching
	// the specified label
	int FindField(string label, string[] fields, int pos)
	{
		for(; pos<fields.Length; ++pos)
		{
			if (label == fields[pos])
				return pos;
		}

		Debug.LogError("Missing \"" + label + "\" field in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
		return pos;
	}

	// Parses the font definition header.
	void HeaderParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("face", vals, 1);
		face = vals[index + 1].Trim(new char[] {'\"'});

		index = FindField("size", vals, index);
		pxSize = Mathf.Abs(int.Parse(vals[index + 1]));
	}

	// Parses the "common" line
	void CommonParser(string line)
	{
		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("lineHeight", vals, 1);
		lineHeight = int.Parse(vals[index + 1]);

		index = FindField("base", vals, index);
		baseHeight = int.Parse(vals[index + 1]);

		index = FindField("scaleW", vals, index);
		texWidth = int.Parse(vals[index + 1]);

		index = FindField("scaleH", vals, index);
		texHeight = int.Parse(vals[index + 1]);

		index = FindField("pages", vals, index);
		
		if(int.Parse(vals[index + 1]) > 1)
			Debug.LogError("Multiple pages/textures detected for font \"" + face + "\". only one font atlas is supported.");
	}

	// Parses the "chars count" line
	void CharCountParser(string line)
	{
		string[] vals = line.Split(new char[] { '=' });

		if(vals.Length < 2)
		{
			Debug.LogError("Malformed \"chars count\" line in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return;
		}

		// Add one for the space character that is
		// always included but not counted:
		chars = new SpriteChar[int.Parse(vals[1]) + 1];
	}

	// Parses a character definition line
	bool CharParser(string line, int charNum)
	{
		if (!line.StartsWith("char"))
			return false;

		float x, y, width, height;

		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("id", vals, 1);
		chars[charNum].id = int.Parse(vals[index + 1]);

		index = FindField("x", vals, index);
		x = float.Parse(vals[index + 1]) / (float)texWidth;

		index = FindField("y", vals, index);
		y = 1f - float.Parse(vals[index + 1]) / (float)texHeight;

		index = FindField("width", vals, index);
		width = float.Parse(vals[index + 1]) / (float)texWidth;

		index = FindField("height", vals, index);
		height = float.Parse(vals[index + 1]) / (float)texHeight;

		index = FindField("xoffset", vals, index);
		chars[charNum].xOffset = float.Parse(vals[index + 1]);

		index = FindField("yoffset", vals, index);
		chars[charNum].yOffset = -float.Parse(vals[index + 1]);

		index = FindField("xadvance", vals, index);
		chars[charNum].xAdvance = int.Parse(vals[index + 1]);

		// Build our character's UVs:
		chars[charNum].UVs.x = x;
		chars[charNum].UVs.y = y - height;
		chars[charNum].UVs.xMax = x + width;
		chars[charNum].UVs.yMax = y;

		charMap.Add(Convert.ToChar(chars[charNum].id), charNum);

		return true;
	}

	// Parses the kernings count
	void KerningCountParser(string line)
	{
		string[] vals = line.Split(new char[] { '=' });
		kerningsCount = int.Parse(vals[1]);
	}

	// Parses the kernings
	bool KerningParser(string line)
	{
		if (!line.StartsWith("kerning"))
			return false;

		int first, second, amount;

		string[] vals = line.Split(new char[] { ' ', '=' });

		int index = FindField("first", vals, 1);
		first = int.Parse(vals[index + 1]);

		index = FindField("second", vals, index);
		second = int.Parse(vals[index + 1]);

		index = FindField("amount", vals, index);
		amount = int.Parse(vals[index + 1]);

		// Now add the kerning info to the appropriate character:
		SpriteChar ch = GetSpriteChar(Convert.ToChar(second));

		if (ch.kernings == null)
			ch.kernings = new Dictionary<char, float>();

		ch.kernings.Add(Convert.ToChar(first), (float)amount);

		return true;
	}

	/// <summary>
	/// Returns a reference to the SpriteChar that
	/// corresponds to the specified character ID
	/// (usually the numeric Unicode value).
	/// </summary>
	/// <param name="ch">The numeric value/code of the desired character.
	/// This value can be obtained from a char with Convert.ToInt32().</param>
	/// <returns>Reference to the corresponding SpriteChar that contains information about the character.</returns>
	public SpriteChar GetSpriteChar(char ch)
	{
		int index;
#if UNITY_IPHONE && !UNITY_3_0
		if (!charMap.ContainsKey(ch))
#else
		if (!charMap.TryGetValue(ch, out index))
#endif
			return default(SpriteChar); // Character not found

#if UNITY_IPHONE && !UNITY_3_0
		index = (int) charMap[ch];
		return chars[index];
#else
		return chars[index];
#endif
	}

	/// <summary>
	/// Returns whether the specified character is part
	/// of this font definition.
	/// </summary>
	/// <param name="ch">Character to check.</param>
	/// <returns>True if the character exists in the font definition.  False otherwise.</returns>
	public bool ContainsCharacter(char ch)
	{
		return charMap.ContainsKey(ch);
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(string str)
	{
		SpriteChar chr;
		float width;

		if(str.Length < 1)
			return 0;

		// Get the first character:
		chr = GetSpriteChar(str[0]);
		width = chr.xAdvance;

		for(int i=1; i<str.Length; ++i)
		{
			chr = GetSpriteChar(str[i]);
			width += chr.xAdvance + chr.GetKerning(str[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <param name="start">The index of the first character of the substring to be measured.</param>
	/// <param name="end">The index of the last character of the substring.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(string str, int start, int end)
	{
		SpriteChar chr;
		float width;

		if (start >= str.Length || end < start)
			return 0;

		end = Mathf.Clamp(end, 0, str.Length-1);

		// Get the first character:
		chr = GetSpriteChar(str[start]);
		width = chr.xAdvance;

		for (int i = start+1; i <= end; ++i)
		{
			chr = GetSpriteChar(str[i]);
			width += chr.xAdvance + chr.GetKerning(str[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified string
	/// would be, in pixels.
	/// </summary>
	/// <param name="str">The string to measure.</param>
	/// <param name="start">The index of the first character of the substring to be measured.</param>
	/// <param name="end">The index of the last character of the substring.</param>
	/// <returns>The width, in pixels, of the string.</returns>
	public float GetWidth(StringBuilder sb, int start, int end)
	{
		SpriteChar chr;
		float width;

		if (start >= sb.Length || end < start)
			return 0;

		end = Mathf.Clamp(end, 0, sb.Length - 1);

		// Get the first character:
		chr = GetSpriteChar(sb[start]);
		width = chr.xAdvance;

		for (int i = start + 1; i <= end; ++i)
		{
			chr = GetSpriteChar(sb[i]);
			width += chr.xAdvance + chr.GetKerning(sb[i - 1]);
		}

		return width;
	}

	/// <summary>
	/// Gets how wide the specified character
	/// would be, in pixels, when displayed.
	/// </summary>
	/// <param name="prevChar">The character previous to that being measured.</param>
	/// <param name="str">The character to measure.</param>
	/// <returns>The width, in pixels, of the character, as displayed (includes the xAdvance).</returns>
	public float GetWidth(char prevChar, char c)
	{
		SpriteChar chr = GetSpriteChar(c);
		return chr.xAdvance + chr.GetKerning(prevChar);
	}

	/// <summary>
	/// Returns a version of the specified string with all
	/// characters removed which are not defined for this font.
	/// </summary>
	/// <param name="str">The string to be stripped of unsupported characters.</param>
	/// <returns>A new string containing only those characters supported by this font.</returns>
	public string RemoveUnsupportedCharacters(string str)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < str.Length; ++i)
			if (charMap.ContainsKey(str[i]) || str[i] == '\n' || str[i] == '\t')
				sb.Append(str[i]);

		return sb.ToString();
	}
}
