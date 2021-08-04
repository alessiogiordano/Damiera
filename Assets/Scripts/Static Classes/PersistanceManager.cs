using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PersistanceManager
{
    // First Load
    public static bool firstLoad {
        get => !PlayerPrefs.HasKey("DAMIERA-VERSION");
        set
        {
            if (value)
            {
                PlayerPrefs.DeleteKey("DAMIERA-VERSION");
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetString("DAMIERA-VERSION", "0.9");
                PlayerPrefs.Save();
            }
        }
    }
    // Preferences
    public static ViewportRotation viewportRotation {
        get => (ViewportRotation) PlayerPrefs.GetInt("VIEWPORT-ROTATION", (int) ViewportRotation.MouseAndKeyboard);
        set
        {
             PlayerPrefs.SetInt("VIEWPORT-ROTATION", (int) value);
             PlayerPrefs.Save();
        }
    }
    public static bool canRotateWithKeyboard {
        get => viewportRotation == ViewportRotation.MouseAndKeyboard || viewportRotation == ViewportRotation.KeyboardOnly;
    }
    public static bool canRotateWithMouse {
        get => viewportRotation == ViewportRotation.MouseAndKeyboard || viewportRotation == ViewportRotation.MouseOnly;
    }
    public static HdpiScaling hdpiScaling {
        get => (HdpiScaling) PlayerPrefs.GetInt("HDPI-SCALING", (int) HdpiScaling.Auto);
        set
        {
             PlayerPrefs.SetInt("HDPI-SCALING", (int) value);
             PlayerPrefs.Save();
        }
    }
    public static ComputerDifficulty computerDifficulty {
        get => (ComputerDifficulty) PlayerPrefs.GetInt("COMPUTER-DIFFICULTY", (int) ComputerDifficulty.Normal);
        set
        {
             PlayerPrefs.SetInt("COMPUTER-DIFFICULTY", (int) value);
             PlayerPrefs.Save();
        }
    }
    public static bool autoRotate {
        get => PlayerPrefs.GetInt("AUTO-ROTATE", 1) != 0;
        set
        {
             PlayerPrefs.SetInt("AUTO-ROTATE", value ? 1 : 0);
             PlayerPrefs.Save();
        }
    }
    public static bool voiceover {
        get => PlayerPrefs.GetInt("VOICEOVER", 1) != 0;
        set
        {
             PlayerPrefs.SetInt("VOICEOVER", value ? 1 : 0);
             PlayerPrefs.Save();
        }
    }
    public static bool soundtrack {
        get => PlayerPrefs.GetInt("SOUNDTRACK", 1) != 0;
        set
        {
             PlayerPrefs.SetInt("SOUNDTRACK", value ? 1 : 0);
             PlayerPrefs.Save();
        }
    }
    public static int soundVolume {
        get => PlayerPrefs.GetInt("SOUND-VOLUME", 10);
        set
        {
             PlayerPrefs.SetInt("SOUND-VOLUME", value);
             PlayerPrefs.Save();
        }
    }
    public static int musicVolume {
        get => PlayerPrefs.GetInt("MUSIC-VOLUME", 5);
        set
        {
             PlayerPrefs.SetInt("MUSIC-VOLUME", value);
             PlayerPrefs.Save();
        }
    }
    // Saved Games
    public static (string, string, string, bool, bool, int, int, int, bool, PlayerColor, BoardCell[], bool[]) slotOne
    {
        get => GetSlot(1);
        set => SaveSlot(1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8, value.Item9, value.Item10, value.Item11, value.Item12);
    }
    public static bool slotOneHasValue
    {
        get => SlotContainsData(1);
        set { if(!value) DeleteSlot(1); }
    }
    public static (string, string, string, bool, bool, int, int, int, bool, PlayerColor, BoardCell[], bool[]) slotTwo
    {
        get => GetSlot(2);
        set => SaveSlot(2, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8, value.Item9, value.Item10, value.Item11, value.Item12);
    }
    public static bool slotTwoHasValue
    {
        get => SlotContainsData(2);
        set { if(!value) DeleteSlot(2); }
    }
    public static (string, string, string, bool, bool, int, int, int, bool, PlayerColor, BoardCell[], bool[]) slotThree
    {
        get => GetSlot(3);
        set => SaveSlot(3, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8, value.Item9, value.Item10, value.Item11, value.Item12);
    }
    public static bool slotThreeHasValue
    {
        get => SlotContainsData(3);
        set { if(!value) DeleteSlot(3); }
    }
    public static (string, string, string, bool, bool, int, int, int, bool, PlayerColor, BoardCell[], bool[]) slotFour
    {
        get => GetSlot(4);
        set => SaveSlot(4, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8, value.Item9, value.Item10, value.Item11, value.Item12);
    }
    public static bool slotFourHasValue
    {
        get => SlotContainsData(4);
        set { if(!value) DeleteSlot(4); }
    }
    public static void SaveSlot(int slot,
                                string whiteName, string blackName,
                                bool isWhiteHuman, bool isBlackHuman,
                                int whiteScore, int blackScore, int aiDifficulty,
                                bool completed, PlayerColor turn,
                                BoardCell[] layout, bool[] damaLayout
                               )
    {
        PlayerPrefs.SetInt($"SLOT{slot}-TIMESTAMP", Timestamp());
        PlayerPrefs.SetString($"SLOT{slot}-WHITENAME", whiteName);
        PlayerPrefs.SetString($"SLOT{slot}-BLACKNAME", blackName);
        PlayerPrefs.SetInt($"SLOT{slot}-WHITEHUMAN", isWhiteHuman ? 1 : 0);
        PlayerPrefs.SetInt($"SLOT{slot}-BLACKHUMAN", isBlackHuman ? 1 : 0);
        PlayerPrefs.SetInt($"SLOT{slot}-WHITESCORE", whiteScore);
        PlayerPrefs.SetInt($"SLOT{slot}-BLACKSCORE", blackScore);
        PlayerPrefs.SetInt($"SLOT{slot}-AIDIFFICULTY", aiDifficulty);
        PlayerPrefs.SetInt($"SLOT{slot}-COMPLETED", completed ? 1 : 0);
        PlayerPrefs.SetInt($"SLOT{slot}-TURN", (int) turn);
        PlayerPrefs.SetString($"SLOT{slot}-LAYOUT", layout.Serialize());
        PlayerPrefs.SetString($"SLOT{slot}-DAMA", damaLayout.Serialize());
        PlayerPrefs.Save();
    }

    public static (string, string, string, bool, bool, int, int, int, bool, PlayerColor, BoardCell[], bool[]) GetSlot(int slot)
    {
        string timestamp = GetSlotTimestamp(slot);
        string whiteName = PlayerPrefs.GetString($"SLOT{slot}-WHITENAME");
        string blackName = PlayerPrefs.GetString($"SLOT{slot}-BLACKNAME");
        bool isWhiteHuman = PlayerPrefs.GetInt($"SLOT{slot}-WHITEHUMAN") != 0;
        bool isBlackHuman = PlayerPrefs.GetInt($"SLOT{slot}-BLACKHUMAN") != 0;
        int whiteScore = PlayerPrefs.GetInt($"SLOT{slot}-WHITESCORE");
        int blackScore = PlayerPrefs.GetInt($"SLOT{slot}-BLACKSCORE");
        int aiDifficulty = PlayerPrefs.GetInt($"SLOT{slot}-AIDIFFICULTY");
        bool completed = PlayerPrefs.GetInt($"SLOT{slot}-COMPLETED") != 0;
        PlayerColor turn = (PlayerColor) PlayerPrefs.GetInt($"SLOT{slot}-TURN");
        BoardCell[] layout = BoardCell.Deserialize(PlayerPrefs.GetString($"SLOT{slot}-LAYOUT"));
        bool[] damaLayout = BoardCell.DeserializeDama(PlayerPrefs.GetString($"SLOT{slot}-DAMA"));
        return (timestamp, whiteName, blackName, isWhiteHuman, isBlackHuman, whiteScore, blackScore, aiDifficulty, completed, turn, layout, damaLayout);
    }
    public static string GetSlotTimestamp(int slot) => FormattedDate(DateFromTimestamp(PlayerPrefs.GetInt($"SLOT{slot}-TIMESTAMP")));
    public static void DeleteSlot(int slot)
    {
        PlayerPrefs.DeleteKey($"SLOT{slot}-TIMESTAMP");
        PlayerPrefs.DeleteKey($"SLOT{slot}-WHITENAME");
        PlayerPrefs.DeleteKey($"SLOT{slot}-BLACKNAME");
        PlayerPrefs.DeleteKey($"SLOT{slot}-WHITEHUMAN");
        PlayerPrefs.DeleteKey($"SLOT{slot}-BLACKHUMAN");
        PlayerPrefs.DeleteKey($"SLOT{slot}-WHITESCORE");
        PlayerPrefs.DeleteKey($"SLOT{slot}-BLACKSCORE");
        PlayerPrefs.DeleteKey($"SLOT{slot}-AIDIFFICULTY");
        PlayerPrefs.DeleteKey($"SLOT{slot}-COMPLETED");
        PlayerPrefs.DeleteKey($"SLOT{slot}-TURN");
        PlayerPrefs.DeleteKey($"SLOT{slot}-LAYOUT");
        PlayerPrefs.DeleteKey($"SLOT{slot}-DAMA");
        PlayerPrefs.Save();
    }
    public static bool SlotContainsData(int slot) => PlayerPrefs.HasKey($"SLOT{slot}-TIMESTAMP");

    // Score Record
    public static bool hasRecord { get => PlayerPrefs.HasKey($"HIGHSCORE{1}-TIMESTAMP"); }
    public static string record
    {
        get {
            if (hasRecord)
            {
                return  "Record: " + PlayerPrefs.GetInt($"HIGHSCORE{1}-SCORE")
                    + " punt" + (PlayerPrefs.GetInt($"HIGHSCORE{1}-SCORE") == 1 ? "o" : "i") + " di " + PlayerPrefs.GetString($"HIGHSCORE{1}-PLAYERNAME")
                    + " contro " + PlayerPrefs.GetString($"HIGHSCORE{1}-ADVERSARYNAME")
                    + " (" + FormattedDate(DateFromTimestamp(PlayerPrefs.GetInt($"HIGHSCORE{1}-TIMESTAMP"))) + ")";
            }
            else
            {
                return "Inizia a giocare e vedrai qui il punteggio record";
            }
        }
    }
    public static string[] scoreboard
    {
        get
        {
            string[] result = new string[4];
            for (int i = 0; i < 4; i++)
            {
                if (PlayerPrefs.HasKey($"HIGHSCORE{i+1}-TIMESTAMP"))
                {
                    result[i] = PlayerPrefs.GetInt($"HIGHSCORE{i+1}-SCORE")
                    + " punt" + (PlayerPrefs.GetInt($"HIGHSCORE{i+1}-SCORE") == 1 ? "o" : "i") + " di " + PlayerPrefs.GetString($"HIGHSCORE{i+1}-PLAYERNAME")
                    + " contro " + PlayerPrefs.GetString($"HIGHSCORE{i+1}-ADVERSARYNAME")
                    + " (" + FormattedDate(DateFromTimestamp(PlayerPrefs.GetInt($"HIGHSCORE{i+1}-TIMESTAMP"))) + ")";
                }
                else
                {
                    result[i] = "";
                }
            }
            return result;
        }
    }
    public static void RegisterRecord(int score, string playerName, string adversaryName)
    {
        int ranking = 0;
        for (int i = 1; i < 5; i++)
        {
            if (PlayerPrefs.HasKey($"HIGHSCORE{i}-TIMESTAMP"))
            {
                int existingScore = PlayerPrefs.GetInt($"HIGHSCORE{i}-SCORE");
                if (existingScore == score &&
                    PlayerPrefs.GetString($"HIGHSCORE{i}-PLAYERNAME") == playerName &&
                    PlayerPrefs.GetString($"HIGHSCORE{i}-ADVERSARYNAME") == adversaryName) return;
                if (existingScore < score)
                {
                    ranking = i;
                    break;
                }
            }
            else
            {
                ranking = i;
                break;
            }
        }
        if (ranking > 0)
        {
            // Slide Down
            for (int j = 3; j >= ranking; j--)
            {
                if (PlayerPrefs.HasKey($"HIGHSCORE{j}-TIMESTAMP"))
                {
                    PlayerPrefs.SetInt($"HIGHSCORE{j+1}-SCORE", PlayerPrefs.GetInt($"HIGHSCORE{j}-SCORE"));
                    PlayerPrefs.SetString($"HIGHSCORE{j+1}-PLAYERNAME", PlayerPrefs.GetString($"HIGHSCORE{j}-PLAYERNAME"));
                    PlayerPrefs.SetString($"HIGHSCORE{j+1}-ADVERSARYNAME", PlayerPrefs.GetString($"HIGHSCORE{j}-ADVERSARYNAME"));
                    PlayerPrefs.SetInt($"HIGHSCORE{j+1}-TIMESTAMP", PlayerPrefs.GetInt($"HIGHSCORE{j}-TIMESTAMP"));
                }
            }
            // Set New
            PlayerPrefs.SetInt($"HIGHSCORE{ranking}-SCORE", score);
            PlayerPrefs.SetString($"HIGHSCORE{ranking}-PLAYERNAME", playerName);
            PlayerPrefs.SetString($"HIGHSCORE{ranking}-ADVERSARYNAME", adversaryName);
            PlayerPrefs.SetInt($"HIGHSCORE{ranking}-TIMESTAMP", Timestamp());
            PlayerPrefs.Save();
        }
    }
    public static void DeleteScoreboard()
    {
        for (int i = 1; i < 5; i++)
        {
            PlayerPrefs.DeleteKey($"HIGHSCORE{i}-TIMESTAMP");
            PlayerPrefs.DeleteKey($"HIGHSCORE{i}-SCORE");
            PlayerPrefs.DeleteKey($"HIGHSCORE{i}-PLAYERNAME");
            PlayerPrefs.DeleteKey($"HIGHSCORE{i}-ADVERSARYNAME");
        }
        PlayerPrefs.Save();
    }

    // Date Methods
    public static int Timestamp() => (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    public static DateTime DateFromTimestamp(int timestamp) => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    public static string FormattedDate(DateTime date) => date.ToString("HH:mm  dd.MM.yyyy");
}
