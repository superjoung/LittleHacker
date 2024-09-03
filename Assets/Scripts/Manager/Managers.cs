using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    private TextManager _text = new TextManager();
    private SoundManager _sound = new SoundManager();

    public static SoundManager Sound { get { return Instance._sound; } }
    public static TextManager Text { get { return Instance._text; } }
}
