using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    [Header("Managers")]
    public Compiler compiler;
    public PanelManager panelManager;

    [Header("GameObjects")]
    public TMP_Text compilePopupText;

    public void compile(List<GameObject> blocks) {
        compilePopupText.SetText("Compilando...");
        compilePopupText.SetText(compiler.Compile(blocks));
    }
}
