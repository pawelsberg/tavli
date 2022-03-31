namespace Pawelsberg.Tavli.Model.Common;

public static class TextParser
{
    public static string Read(string wholeText, string textToRead)
    {
        if (!wholeText.StartsWith(textToRead))
            throw new Exception($"Expected '{textToRead}'");
        return wholeText.Substring(textToRead.Length);
    }
    public static string Read<TEnum>(string wholeText, out TEnum enumValue) where TEnum : struct
    {
        string enumValueText = wholeText.Substring(0, wholeText.IndexOf(' '));
        if (!Enum.TryParse(enumValueText, out enumValue))
            throw new Exception($"Cannot parse {nameof(TEnum)}");

        return wholeText.Substring(enumValue.ToString().Length);
    }
    public static string ReadInt(string wholeText, out int intValue)
    {
        string intValueText = wholeText.Substring(0, wholeText.IndexOf(' '));
        if (!int.TryParse(intValueText, out intValue))
            throw new Exception("Cannot parse white player pips value");
        return wholeText.Substring(intValue.ToString().Length);
    }

    public static string ReadBoardSix(string wholeText, out List<PlayerColour?> boardSix)
    {
        string textFromFirst = Read(wholeText, "║ ");
        PlayerColour? firstPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromFirst[0]);
        string textFromSecond = Read(textFromFirst.Substring(1), " │ ");
        PlayerColour? secondPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromSecond[0]);
        string textFromThird = Read(textFromSecond.Substring(1), " │ ");
        PlayerColour? thirdPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromThird[0]);
        string textFromForth = Read(textFromThird.Substring(1), " │ ");
        PlayerColour? forthPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromForth[0]);
        string textFromFiveth = Read(textFromForth.Substring(1), " │ ");
        PlayerColour? fivethPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromFiveth[0]);
        string textFromSixth = Read(textFromFiveth.Substring(1), " │ ");
        PlayerColour? sixthPlayerColour = PlayerColourExtensions.ToPlayerColourNullable(textFromSixth[0]);
        boardSix = new List<PlayerColour?> { firstPlayerColour, secondPlayerColour, thirdPlayerColour, forthPlayerColour, fivethPlayerColour, sixthPlayerColour };
        return Read(textFromSixth.Substring(1), " ║");
    }
}

public record ParsableText
{
    public string Text;
    public ParsableText Read(string textToRead)
    {
        return new ParsableText { Text = TextParser.Read(Text, textToRead) };
    }
    public ParsableText Read<TEnum>(out TEnum enumValue) where TEnum : struct
    {
        return new ParsableText { Text = TextParser.Read(Text, out enumValue) };
    }
    public ParsableText ReadInt(out int intValue)
    {
        return new ParsableText { Text = TextParser.ReadInt(Text, out intValue) };
    }
    public ParsableText ReadIntNullable(out int? intNullableValue, int occupiedSpace)
    {
        string intValueText = Text.Substring(0, occupiedSpace);
        if (string.IsNullOrWhiteSpace(intValueText))
            intNullableValue = null;
        else
        {
            if (!int.TryParse(intValueText, out int intValue))
                throw new Exception("Cannot parse white player pips value");
            intNullableValue = intValue;
        }
        return new ParsableText { Text = Text.Substring(occupiedSpace) };
    }
    public ParsableText ReadPlayerColourNullable(out PlayerColour? playerColour)
    {
        playerColour = PlayerColourExtensions.ToPlayerColourNullable(Text[0]);
        return new ParsableText { Text = Text.Substring(1) };
    }
    public ParsableText ReadBoardSix(out List<PlayerColour?> boardSix)
    {
        ParsableText result =
            Read("║ ")
            .ReadPlayerColourNullable(out PlayerColour? firstPlayerColour)
            .Read(" │ ")
            .ReadPlayerColourNullable(out PlayerColour? secondPlayerColour)
            .Read(" │ ")
            .ReadPlayerColourNullable(out PlayerColour? thirdPlayerColour)
            .Read(" │ ")
            .ReadPlayerColourNullable(out PlayerColour? forthPlayerColour)
            .Read(" │ ")
            .ReadPlayerColourNullable(out PlayerColour? fivethPlayerColour)
            .Read(" │ ")
            .ReadPlayerColourNullable(out PlayerColour? sixthPlayerColour)
            .Read(" ║")
            ;
        boardSix = new List<PlayerColour?> { firstPlayerColour, secondPlayerColour, thirdPlayerColour, forthPlayerColour, fivethPlayerColour, sixthPlayerColour };
        return result;
    }
    public ParsableText ReadBoardLine(out List<PlayerColour?> leftBoardSix, out List<PlayerColour?> rightBoardSix)
    {
        return ReadBoardSix(out leftBoardSix)
             .Read("   ")
             .ReadBoardSix(out rightBoardSix)
             .Read("   ║\r\n");
    }
    public ParsableText ReadBoardLineWithNumbers(out List<PlayerColour?> leftBoardSix, out int? leftNumber, out List<PlayerColour?> rightBoardSix, out int? rightNumber)
    {
        return ReadBoardSix(out leftBoardSix)
            .ReadIntNullable(out leftNumber, 3)
            .ReadBoardSix(out rightBoardSix)
            .ReadIntNullable(out rightNumber, 3)
            .Read("║\r\n");
    }
    public ParsableText ReadBoardLineWithPlayerColours(out List<PlayerColour?> leftBoardSix, out PlayerColour? leftPlayerColour, out List<PlayerColour?> rightBoardSix, out PlayerColour? rightPlayerColour)
    {
        return ReadBoardSix(out leftBoardSix)
            .Read(" ")
            .ReadPlayerColourNullable(out leftPlayerColour)
            .Read(" ")
            .ReadBoardSix(out rightBoardSix)
            .Read(" ")
            .ReadPlayerColourNullable(out rightPlayerColour)
            .Read(" ║\r\n");
    }
    public ParsableText WhileCharacterReadBoardLines(char character, out List<(List<PlayerColour?>, List<PlayerColour?>)> leftRightLines)
    {
        if (Text[0] == character)
        {
            leftRightLines = new List<(List<PlayerColour?>, List<PlayerColour?>)> { };
            return this;
        }
        else
        {
            ParsableText result =
            ReadBoardLine(out List<PlayerColour?> leftBoardSix, out List<PlayerColour?> rightBoardSix)
            .WhileCharacterReadBoardLines(character, out List<(List<PlayerColour?>, List<PlayerColour?>)> nextLeftRightLines);
            leftRightLines = new List<(List<PlayerColour?>, List<PlayerColour?>)> { (leftBoardSix, rightBoardSix) }.Concat(nextLeftRightLines).ToList();

            return result;
        }
    }
}

