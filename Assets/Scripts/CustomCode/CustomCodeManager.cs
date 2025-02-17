using System.Reflection;
using System.Xml;
// using System.Reflection.Metadata.Ecma335;
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
using JetBrains.Annotations;
using System.Linq;

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
    public bool isCustom = true;

    [Header("Casos de teste")]
    public GameObject casePrefab;
    public GameObject casesPopUp;

    public TMP_InputField[] caseVariables;
    public GameObject createCaseButton;
    public struct casoDeTeste
    {
        public GameObject botao;
        public Dictionary<string, int> variables;
        public int index;

    }
    public List<casoDeTeste> casos = new List<casoDeTeste>();

    public Dictionary<string, int> variables;

    private string folderName;
    private GameObject caseVariablesContainer;
    private GameObject saveCaseButton;
    private GameObject deleteCaseButton;
    private TMP_Text textoCasoNumero;
    private int currentCaseIndex;
    private bool carregandoDepoisDoTesteDaBatalha = false;
    private int carregandoDepoisDoTesteDaBatalhaIterator;

    void Start()
    {

        Debug.Log(Memories.getToEdit());
        Debug.Log(Memories.getNewLevel());
        Debug.Log("é teste? " + BattleData.isTest);
        caseVariablesContainer  =    casesPopUp.gameObject.transform.GetChild(0).Find("caseVariables").gameObject;
        textoCasoNumero         =    casesPopUp.gameObject.transform.GetChild(0).Find("textoCasoNumero").gameObject.GetComponent<TMP_Text>();
        saveCaseButton          =    casesPopUp.gameObject.transform.GetChild(0).Find("salvarCaseBTN").gameObject;
        deleteCaseButton        =    casesPopUp.gameObject.transform.GetChild(0).Find("deleteCaseBTN").gameObject;
        if (BattleData.isTest)
        {
            if (!Memories.getNewLevel() && !Memories.getToEdit())
            {
                Debug.Log("entrou no if not new level e not to edit");
                GameObject botaoExport = GameObject.Find("ExportButton");
                Debug.Log(botaoExport);
                botaoExport.GetComponentInChildren<TMP_Text>().text = "Salvar";
                BattleData.levelMemory = Memories.GetMemory(BattleData.selectedLevel);
                // BattleData.levelBlocks = compiler.Decompile(BattleData.levelMemory.memory);
                Memories.setToEdit(true);
            }
            caseVariablesContainer.SetActive(true);
            saveCaseButton.SetActive(true);
            deleteCaseButton.SetActive(true);
            carregandoDepoisDoTesteDaBatalha = true;
            Debug.Log(BattleData.levelMemory.memory.ToList().Count());
            panelManager.LoadCommands(BattleData.levelMemory.memory.ToList());
            foreach (TMP_InputField inputField in inputFields)
            {
                switch (inputField.name)
                {
                    case "RoundMedal":
                        inputField.text = BattleData.levelMemory.medal.maxRounds.ToString();
                        break;
                    case "SizeMedal":
                        inputField.text = BattleData.levelMemory.medal.maxSize.ToString();
                        break;
                    case "HPPlayer":
                        inputField.text = BattleData.levelMemory.playerFighterAttributes.maxLifePoints.ToString();
                        break;
                    case "HPEnemy":
                        inputField.text = BattleData.levelMemory.enemyFighterAttributes.maxLifePoints.ToString();
                        break;
                    case "DmgPlayer":
                        inputField.text = BattleData.levelMemory.playerFighterAttributes.minAttackPoints.ToString();
                        break;
                    case "DmgEnemy":
                        inputField.text = BattleData.levelMemory.enemyFighterAttributes.minAttackPoints.ToString();
                        break;
                    case "DefPlayer":
                        inputField.text = BattleData.levelMemory.playerFighterAttributes.maxDefensePoints.ToString();
                        break;
                    case "DefEnemy":
                        inputField.text = BattleData.levelMemory.enemyFighterAttributes.maxDefensePoints.ToString();
                        break;
                    case "ChaPlayer":
                        inputField.text = BattleData.levelMemory.playerFighterAttributes.maxChargePoints.ToString();
                        break;
                    case "ChaEnemy":
                        inputField.text = BattleData.levelMemory.enemyFighterAttributes.maxChargePoints.ToString();
                        break;
                }
            }
            
            try
            {
                for (int i = 0; i < BattleData.levelMemory.testesPlayer.Count; i++)
                {
                    carregandoDepoisDoTesteDaBatalhaIterator = i;
                    CreateCase();
                    SalvarCase();
                }
            }
            catch
            {}
            
            hintField.text = BattleData.levelMemory.hint;
            DisableBlocks(BattleData.levelMemory.enabledBlocks);
            BattleData.isTest = false;
            carregandoDepoisDoTesteDaBatalha = false;
            caseVariablesContainer.SetActive(false);
            saveCaseButton.SetActive(false);
            deleteCaseButton.SetActive(false);

        }
        
        if (!Directory.Exists("CustomMemories"))
        {
            Directory.CreateDirectory("CustomMemories");
        }

        if (casos.Count == 0)
        {
            caseVariablesContainer.SetActive(false);
            deleteCaseButton.SetActive(false);
            saveCaseButton.SetActive(false);
            textoCasoNumero.SetText("");
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
        string fileName = "";
        Debug.Log(Memories.getNewLevel());
        if (Memories.getNewLevel())
        {
            fileName = (memoryCount + 1) + ".bin";
            cellsContainer.Serialize(folderName + "/" + fileName);
            Debug.Log("Código exportado");
            Memories.setNewLevel(false);
            GameObject botaoExport = GameObject.Find("ExportButton");
            Debug.Log(botaoExport);
            botaoExport.GetComponentInChildren<TMP_Text>().text = "Salvar";
            BattleData.levelMemory.fileName = fileName;
        }
        else
        {
            cellsContainer.fileName = BattleData.levelMemory.fileName;
            cellsContainer.UpdateFile();
            // Debug.Log(BattleData.levelMemory.fileName);
        }
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

    private void DisableBlocks(bool[] enabledBlocks)
    {
        // bool[] enabledBlocks = new bool[Enum.GetNames(typeof(Commands)).Length];
        foreach (Transform child in blocksContainer)
        {
            BlockController blockController = child.GetComponent<BlockController>();
            if (blockController == null) continue;

            Commands command = blockController.commandName;

            int index = (int)command;
            if (index < 0) continue;
            if (index >= enabledBlocks.Length) 
            {
                blockController.isEnabled = false;
                continue;
            }

            blockController.isEnabled = enabledBlocks[index];
        }
        // return enabledBlocks;
    }

    public void QuitGame()
    {
        panelManager.KillEvents();
        SceneManager.LoadScene("Menu");
    }

    public void LoadTestBattle()
    {
        if (!Compile()) return;

        foreach(Cell c in compiler.Memory)
        {
            try
            {
                Debug.Log("--" + c.ToString());

            }
            catch
            {

            }
        }
        BattleData.isTest = true;
        BattleData.levelMemory = CreateCellsContainer();
        foreach(Cell c in BattleData.levelMemory.memory.ToList())
        {
            // Debug.Log("--" + c);
        }
        try
        {
            BattleData.levelMemory.fileName = Memories.GetMemory(BattleData.selectedLevel).fileName;
        }
        catch (Exception ex)
        {}
        // BattleData.levelCommands = compiler.GetCommands(panelManager.blocks);

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
                Debug.Log(entry.Key);
                Debug.Log(entry.Value);
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
        if(casos.Count > 0 && casos[casos.Count - 1].botao == null){
            return;
        }
        caseVariablesContainer.SetActive(true);
        int nFilhos = casos.Count;
        textoCasoNumero.text = $"Caso {nFilhos}";
        casos.Add(new casoDeTeste(){
            index = nFilhos,
            botao = null,
            variables = new Dictionary<string, int>()
        });
        // casos[casos.Count -1].botao.transform.SetParent(createCaseButton.transform.parent, false);
        currentCaseIndex = nFilhos;
        if (!carregandoDepoisDoTesteDaBatalha)
        {
            carregarCase(casos[nFilhos].index);
        }
        else
        {
            currentCaseIndex = casos[nFilhos].index;
        }
    }

    public void DeleteCase()
    {
        //Deletar o case e renomear o resto
        DestroyImmediate(casos[currentCaseIndex].botao);
        casos.RemoveAt(currentCaseIndex);
        for (int i = 0; i < casos.Count; i++)
        {
            DestroyImmediate(casos[i].botao);
            casoDeTeste aux = new casoDeTeste()
            {
                index = i,
                botao = Instantiate(casePrefab, new Vector3(), Quaternion.identity),
                variables = casos[i].variables,
            };
            Debug.Log(aux.index);
            aux.botao.transform.SetParent(createCaseButton.transform.parent, false);
            aux.botao.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = $"Caso {i}";
            aux.botao.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                () => carregarCase(aux.index)
            );
            casos[i] = aux;
        }
        // if (casos.Count > 0 )
        // {
        //     carregarCase(casos[0].index);
        // }
        // else
        // {
        caseVariablesContainer.SetActive(false);
        textoCasoNumero.SetText("");
        saveCaseButton.SetActive(false);
        deleteCaseButton.SetActive(false);
        // }
    }
    public void SalvarCase()
    {
        Debug.Log(currentCaseIndex);
        if (casos[currentCaseIndex].botao != null)
        {
            DestroyImmediate(casos[currentCaseIndex].botao);
        }
        casoDeTeste TempVAR = new casoDeTeste()
        {
            index = currentCaseIndex,
            botao = Instantiate(casePrefab, new Vector3(), Quaternion.identity),
            variables = new Dictionary<string, int>()
        };
        if (!carregandoDepoisDoTesteDaBatalha)
        {
            foreach (TMP_InputField caseVariable in caseVariables)
            {
                switch (caseVariable.name)
                {
                    case "HPPlayer":
                        TempVAR.variables["HPPlayer"] = int.Parse(caseVariable.text);
                        break;
                    case "HPEnemy":
                        TempVAR.variables["HPEnemy"] = int.Parse(caseVariable.text);
                        break;
                    case "DmgPlayer":
                        TempVAR.variables["DmgPlayer"] = int.Parse(caseVariable.text);
                        break;
                    case "DmgEnemy":
                        TempVAR.variables["DmgEnemy"] = int.Parse(caseVariable.text);
                        break;
                    case "DefPlayer":
                        TempVAR.variables["DefPlayer"] = int.Parse(caseVariable.text);
                        break;
                    case "DefEnemy":
                        TempVAR.variables["DefEnemy"] = int.Parse(caseVariable.text);
                        break;
                    case "ChaPlayer":
                        TempVAR.variables["ChaPlayer"] = int.Parse(caseVariable.text);
                        break;
                    case "ChaEnemy":
                        TempVAR.variables["ChaEnemy"] = int.Parse(caseVariable.text);
                        break;
                }
            }
        }
        else
        {
            foreach (TMP_InputField caseVariable in caseVariables)
            {
                switch (caseVariable.name)
                {
                    case "HPPlayer":
                        TempVAR.variables["HPPlayer"] = BattleData.levelMemory.testesPlayer[carregandoDepoisDoTesteDaBatalhaIterator].maxLifePoints;
                        break;
                    case "HPEnemy":
                        TempVAR.variables["HPEnemy"] = BattleData.levelMemory.testesEnemy[carregandoDepoisDoTesteDaBatalhaIterator].maxLifePoints;
                        break;
                    case "DmgPlayer":
                        TempVAR.variables["DmgPlayer"] = BattleData.levelMemory.testesPlayer[carregandoDepoisDoTesteDaBatalhaIterator].minAttackPoints;
                        break;
                    case "DmgEnemy":
                        TempVAR.variables["DmgEnemy"] = BattleData.levelMemory.testesEnemy[carregandoDepoisDoTesteDaBatalhaIterator].minAttackPoints;
                        break;
                    case "DefPlayer":
                        TempVAR.variables["DefPlayer"] = BattleData.levelMemory.testesPlayer[carregandoDepoisDoTesteDaBatalhaIterator].maxDefensePoints;
                        break;
                    case "DefEnemy":
                        TempVAR.variables["DefEnemy"] = BattleData.levelMemory.testesEnemy[carregandoDepoisDoTesteDaBatalhaIterator].maxDefensePoints;
                        break;
                    case "ChaPlayer":
                        TempVAR.variables["ChaPlayer"] = BattleData.levelMemory.testesPlayer[carregandoDepoisDoTesteDaBatalhaIterator].maxChargePoints;
                        break;
                    case "ChaEnemy":
                        TempVAR.variables["ChaEnemy"] = BattleData.levelMemory.testesEnemy[carregandoDepoisDoTesteDaBatalhaIterator].maxChargePoints;
                        break;
                }
            }
        }
        // inicializarVariaveis(TempVAR);
        TempVAR.botao.transform.SetParent(createCaseButton.transform.parent, false);
        TempVAR.botao.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
            () => carregarCase(TempVAR.index)
        );
        TMP_Text textobotao = TempVAR.botao.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        textobotao.text = $"Caso {currentCaseIndex}";
        casos[currentCaseIndex] = TempVAR;
        caseVariablesContainer.SetActive(false);
        textoCasoNumero.SetText("");
        saveCaseButton.SetActive(false);
        deleteCaseButton.SetActive(false);
        // casos.Add(TempVAR);
    }

    public void carregarCase(int casoNum)
    {
        Debug.Log("carregando " + casoNum);
        currentCaseIndex = casoNum;
        casoDeTeste caso = casos[casoNum];
        caseVariablesContainer.SetActive(true);
        saveCaseButton.SetActive(true);
        deleteCaseButton.SetActive(true);
        textoCasoNumero.text = $"Caso {casoNum}";
        
        if (caso.variables.Count == 0){
            return;
        }
        foreach (TMP_InputField caseVariable in caseVariables)
        {
            switch (caseVariable.name)
            {
                case "HPPlayer":
                    caseVariable.text = $"{caso.variables["HPPlayer"]}";
                    break;
                case "HPEnemy":
                    caseVariable.text = $"{caso.variables["HPEnemy"]}";
                    break;
                case "DmgPlayer":
                    caseVariable.text = $"{caso.variables["DmgPlayer"]}";
                    break;
                case "DmgEnemy":
                    caseVariable.text = $"{caso.variables["DmgEnemy"]}";
                    break;
                case "DefPlayer":
                    caseVariable.text = $"{caso.variables["DefPlayer"]}";
                    break;
                case "DefEnemy":
                    caseVariable.text = $"{caso.variables["DefEnemy"]}";
                    break;
                case "ChaPlayer":
                    caseVariable.text = $"{caso.variables["ChaPlayer"]}";
                    break;
                case "ChaEnemy":
                    caseVariable.text = $"{caso.variables["ChaEnemy"]}";
                    break;
            }
        }
        Debug.Log("carregado" + casoNum);
    }
}
