using System.Globalization;
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
using UnityEngine.UIElements;
using Unity.Mathematics;

public class CustomCodeManager : MonoBehaviour
{
    
    [Header("Managers")]
    public PanelManager panelManager;

    [Header("Compiler")]
    [SerializeField] private Compiler compiler;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;
    public TMP_InputField hintField;
    public TMP_InputField[] inputFields;
    public Transform blocksContainer;
    public GameObject casePrefab;
    public TMP_InputField[] caseVariables;
    // public static TMP_InputField[] caseVariables;
    public GameObject casesButton;
    public struct casoDeTeste
    {
        public GameObject botao;
        public Dictionary<string, int> variables;
        public int roundMedal;
        public int sizeMedal;
        public int hpPlayer;
        public int hpEnemy;
        public int dmgPlayer;
        public int dmgEnemy;
        public int defPlayer;
        public int defEnemy;
        public int chaPlayer;
        public int chaEnemy;
        public int index;

    }
    public List<casoDeTeste> casos = new List<casoDeTeste>();

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
        List<FighterAttributes> testesPlayer = new List<FighterAttributes>();
        List<FighterAttributes> testesEnemy = new List<FighterAttributes>();
        foreach(var caso in casos)
        {
            foreach(KeyValuePair<string, int> entry in caso.variables)
            {
                switch (entry.Key)
                {
                    case "RoundMedal":
                        roundMedal = entry.Value;
                        break;
                    case "SizeMedal":
                        sizeMedal = entry.Value;
                        break;
                    case "HPPlayer":
                        hpPlayer = entry.Value;
                        break;
                    case "HPEnemy":
                        hpEnemy = entry.Value;
                        break;
                    case "DmgPlayer":
                        dmgPlayer = entry.Value;
                        break;
                    case "DmgEnemy":
                        dmgEnemy = entry.Value;
                        break;
                    case "DefPlayer":
                        defPlayer = entry.Value;
                        break;
                    case "DefEnemy":
                        defEnemy = entry.Value;
                        break;
                    case "ChaPlayer":
                        chaPlayer = entry.Value;
                        break;
                    case "ChaEnemy":
                        chaEnemy = entry.Value;
                        break;
                }
            }
            
            testesPlayer.Add(new FighterAttributes(hpPlayer, dmgPlayer, defPlayer, chaPlayer));
            testesEnemy.Add(new FighterAttributes(hpEnemy, dmgEnemy, defEnemy, chaEnemy));
        }

        string hint = hintField.text; 

        bool[] isBlockDisabled = GetEnabledBlocks();

        CellsContainer cellsContainer = new CellsContainer(compiler, playerFighter, enemyFighter, medal, isBlockDisabled, hint, testesPlayer, testesEnemy);
        return cellsContainer;
    }

    public void ClearBlocks()
    {
        panelManager.Clear();
    }
    public void CreateCase()
    {
        Debug.Log("teste");
        int nFilhos = casesButton.transform.parent.transform.childCount - 1;

        casoDeTeste TempVAR = new casoDeTeste()
        {
            index = nFilhos,
            botao = Instantiate(casePrefab, new Vector3(), Quaternion.identity),
            variables = new Dictionary<string, int>()
        };
        foreach (TMP_InputField caseVariable in caseVariables)
        {
            switch (caseVariable.name)
            {
                // case "RoundMedal":
                //     TempVAR.roundMedal = int.Parse(caseVariable.text);
                //     break;
                // case "SizeMedal":
                //     TempVAR.sizeMedal = int.Parse(caseVariable.text);
                //     Debug.Log($"size medal: {TempVAR.sizeMedal}");
                //     break;
                case "HPPlayer":
                    TempVAR.variables["hpPlayer"] = int.Parse(caseVariable.text);
                    break;
                case "HPEnemy":
                    TempVAR.variables["hpEnemy"] = int.Parse(caseVariable.text);
                    break;
                case "DmgPlayer":
                    TempVAR.variables["dmgPlayer"] = int.Parse(caseVariable.text);
                    break;
                case "DmgEnemy":
                    TempVAR.variables["dmgEnemy"] = int.Parse(caseVariable.text);
                    break;
                case "DefPlayer":
                    TempVAR.variables["defPlayer"] = int.Parse(caseVariable.text);
                    break;
                case "DefEnemy":
                    TempVAR.variables["defEnemy"] = int.Parse(caseVariable.text);
                    break;
                case "ChaPlayer":
                    TempVAR.variables["chaPlayer"] = int.Parse(caseVariable.text);
                    break;
                case "ChaEnemy":
                    TempVAR.variables["chaEnemy"] = int.Parse(caseVariable.text);
                    break;
            }
        }
        // inicializarVariaveis(TempVAR);
        TempVAR.botao.transform.SetParent(casesButton.transform.parent, false);
        TempVAR.botao.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
            () => carregarVariaveis(TempVAR)
        );
        TMP_Text textobotao = TempVAR.botao.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        textobotao.text = $"Caso {nFilhos}";
        Debug.Log(TempVAR.index);
        casos.Add(TempVAR);
    }

    public void carregarVariaveis(casoDeTeste caso)
    {

        foreach (TMP_InputField caseVariable in caseVariables)
        {
            switch (caseVariable.name)
            {
                // case "RoundMedal":
                //     caseVariable.text = $"{caso.roundMedal}";
                //     break;
                // case "SizeMedal":
                //     Debug.Log($"size medal 3: {caso.sizeMedal}");
                //     caseVariable.text = $"{caso.sizeMedal}";
                //     break;
                case "HPPlayer":
                    caseVariable.text = $"{caso.variables["hpPlayer"]}";
                    break;
                case "HPEnemy":
                    caseVariable.text = $"{caso.variables["hpEnemy"]}";
                    break;
                case "DmgPlayer":
                    caseVariable.text = $"{caso.variables["dmgPlayer"]}";
                    break;
                case "DmgEnemy":
                    caseVariable.text = $"{caso.variables["dmgEnemy"]}";
                    break;
                case "DefPlayer":
                    caseVariable.text = $"{caso.variables["defPlayer"]}";
                    break;
                case "DefEnemy":
                    caseVariable.text = $"{caso.variables["defEnemy"]}";
                    break;
                case "ChaPlayer":
                    caseVariable.text = $"{caso.variables["chaPlayer"]}";
                    break;
                case "ChaEnemy":
                    caseVariable.text = $"{caso.variables["chaEnemy"]}";
                    break;
            }
        }
    }
}
