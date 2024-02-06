using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class CustomCodeManager : MonoBehaviour
{
    [Header("Managers")]
    public PanelManager panelManager;

    [Header("Compiler")]
    [SerializeField] private Compiler compiler;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;
    public TMP_InputField[] inputFields;
    public Transform blocksContainer;

    public Dictionary<string, int> variables;

    public bool isCustom = true;
    private string folderName;

    void Start()
    {
        if (BattleData.isTest)
        {
            panelManager.LoadCommands(BattleData.levelCommands);
            BattleData.isTest = false;
        }
        
        if (!Directory.Exists("CustomMemories"))
        {
            Directory.CreateDirectory("CustomMemories");
        }
    }

    private void Update()
    {
        folderName = isCustom ? "CustomMemories" : "Memories";
    }

    public void ExportCode()
    {
        if (!Compile()) return;

        CellsContainer cellsContainer = CreateCellsContainer();

        int memoryCount = Directory.GetFiles(folderName).Length;
        string fileName = (memoryCount + 1) + ".bin";
        cellsContainer.Serialize(folderName + "/" + fileName);
        Debug.Log("Código exportado");
    }

    private bool[] GetEnabledBlocks()
    {
        bool[] enabledBlocks = new bool[Enum.GetNames(typeof(Commands)).Length];
        foreach (Transform child in blocksContainer)
        {
            BlockController blockController = child.GetComponent<BlockController>();
            if (blockController == null) continue;

            Commands command = blockController.commandName;
            if (command == null) continue;

            int index = (int)command;
            if (index < 0) continue;
            if (index >= enabledBlocks.Length) continue;

            bool isEnabled = blockController.isEnabled;
            enabledBlocks[index] = isEnabled;
        }
        return enabledBlocks;
    }

    public void QuitGame()
    {
        panelManager.KillEvents();
        SceneManager.LoadScene("Menu");
    }

    public void LoadTestBattle()
    {
        if (!Compile()) return;

        BattleData.levelMemory = CreateCellsContainer();
        BattleData.isTest = true;
        BattleData.levelCommands = compiler.GetCommands(panelManager.blocks);

        panelManager.KillEvents();
        SceneManager.LoadScene("Battle");
    }

    private bool Compile()
    {
        string compileResult = "";
        List<BlockController> blocks = panelManager.blocks;
        bool compiled = compiler.Compile(blocks, ref compileResult);
        if (!compiled)
        {
            compilePopupText.transform.parent.gameObject.SetActive(true);
            compilePopupText.SetText(compileResult);
            return false;
        }
        return true;
    }

    private CellsContainer CreateCellsContainer()
    {
        int roundMedal = 0;
        int sizeMedal = 0;
        int hpPlayer = 0;
        int hpEnemy = 0;
        int dmgPlayer = 0;
        int dmgEnemy = 0;
        int defPlayer = 0;
        int defEnemy = 0;
        int chaPlayer = 0;
        int chaEnemy = 0;
        foreach (TMP_InputField inputField in inputFields)
        {
            switch (inputField.name)
            {
                case "RoundMedal":
                    roundMedal = int.Parse(inputField.text);
                    break;
                case "SizeMedal":
                    sizeMedal = int.Parse(inputField.text);
                    break;
                case "HPPlayer":
                    hpPlayer = int.Parse(inputField.text);
                    break;
                case "HPEnemy":
                    hpEnemy = int.Parse(inputField.text);
                    break;
                case "DmgPlayer":
                    dmgPlayer = int.Parse(inputField.text);
                    break;
                case "DmgEnemy":
                    dmgEnemy = int.Parse(inputField.text);
                    break;
                case "DefPlayer":
                    defPlayer = int.Parse(inputField.text);
                    break;
                case "DefEnemy":
                    defEnemy = int.Parse(inputField.text);
                    break;
                case "ChaPlayer":
                    chaPlayer = int.Parse(inputField.text);
                    break;
                case "ChaEnemy":
                    chaEnemy = int.Parse(inputField.text);
                    break;
            }
        }

        Medal medal = new Medal(roundMedal, sizeMedal);
        FighterAttributes playerFighter = new FighterAttributes(hpPlayer, dmgPlayer, defPlayer, chaPlayer);
        FighterAttributes enemyFighter = new FighterAttributes(hpEnemy, dmgEnemy, defEnemy, chaEnemy);

        bool[] isBlockDisabled = GetEnabledBlocks();

        CellsContainer cellsContainer = new CellsContainer(compiler, playerFighter, enemyFighter, medal, isBlockDisabled);
        return cellsContainer;
    }

    public void ClearBlocks()
    {
        panelManager.Clear();
    }
}
