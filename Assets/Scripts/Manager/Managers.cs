using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    private TextManager _text = new TextManager();

    public static TextManager Text { get { return Instance._text; } }
}
