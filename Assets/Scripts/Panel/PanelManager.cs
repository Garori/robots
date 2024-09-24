using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public struct ActionsPrefabs
{
    public GameObject attackPrefab;
    public GameObject defendPrefab;
    public GameObject chargePrefab;
    public GameObject healPrefab;
}

[Serializable]
public struct CodePrefabs
{
    //AQUI
    public GameObject codePrefab;
    // public GameObject defendPrefab;
    // public GameObject chargePrefab;
    // public GameObject healPrefab;
}

[Serializable]
public struct StructuresPrefabs
{
    public GameObject ifPrefab;
    public GameObject elsePrefab;
    public GameObject whilePrefab;
    public GameObject forPrefab;
    public GameObject endPrefab;
    public GameObject breakPrefab;
}

[Serializable]
public struct ComparatorsPrefabs
{
    public GameObject truePrefab;
    public GameObject equalsPrefab;
    public GameObject notEqualsPrefab;
    public GameObject greaterPrefab;
    public GameObject evenPrefab;
}

[Serializable]
public struct FighterVariableModifiersPrefabs
{
    public GameObject currentPrefab;
    public GameObject halfPrefab;
    public GameObject doublePrefab;
}

[Serializable]
public struct FighterPrefabs
{
    public FighterVariableModifiersPrefabs health;
    public FighterVariableModifiersPrefabs maxHealth;
    public FighterVariableModifiersPrefabs defense;
    public FighterVariableModifiersPrefabs charge;
    public FighterVariableModifiersPrefabs damage;
}

[Serializable]
public struct VariablesPrefabs
{
    public FighterPrefabs player;
    public FighterPrefabs enemy;
    public GameObject roundPrefab;
    public GameObject[] numbersPrefabs;
}

public class PanelManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private RectTransform canvas;

    [SerializeField]
    private RectTransform linesContent;
    public GameObject lineObjectPrefab;
    public GameObject endLineObject;

    // Line info
    private float minLineWidth;
    private float lineHeight;
    private Vector2 endLineSize;
    private RectOffset linePadding;
    private List<HorizontalLayoutGroup> linesLayout;

    [Header("Padding")]
    [SerializeField]
    private int minPadding;

    [SerializeField]
    private int tabPadding;

    [Header("Prefabs")]
    [SerializeField]
    private ActionsPrefabs actionsPrefabs;

    [SerializeField]
    private CodePrefabs codePrefabs;

    [SerializeField]
    private StructuresPrefabs structuresPrefabs;

    [SerializeField]
    private ComparatorsPrefabs comparatorsPrefabs;

    [SerializeField]
    private VariablesPrefabs variablesPrefabs;

    // Blocks and lines arrays
    private int activeLines;
    private List<GameObject> lines;
    public List<BlockController> blocks { get; set; }

    // Panel size
    public float panelX { get; set; }

    private void Awake()
    {
        activeLines = 0;

        blocks = new List<BlockController>();
        lines = new List<GameObject>();
        linesLayout = new List<HorizontalLayoutGroup>();

        RectTransform lineObjectPrefabTransform = lineObjectPrefab.GetComponent<RectTransform>();
        minLineWidth = lineObjectPrefabTransform.sizeDelta.x;
        lineHeight = lineObjectPrefabTransform.sizeDelta.y;

        panelX = canvas.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x;

        endLineSize = endLineObject.GetComponent<RectTransform>().sizeDelta;

        RectOffset lineObjectPrefabPadding = lineObjectPrefab
            .GetComponent<HorizontalLayoutGroup>()
            .padding;
        linePadding = new RectOffset(
            lineObjectPrefabPadding.left,
            lineObjectPrefabPadding.right,
            lineObjectPrefabPadding.top,
            lineObjectPrefabPadding.bottom
        );
    }

    private void Start()
    {
        EventManager.BlockEnter += InsertBlock;
        EventManager.BlockExit += RemoveBlock;

        EventManager.CodeEnter += InsertCode;
        EventManager.CodeExit += RemoveCode;

        EventManager.ComparatorEnter += InsertComparator;
        EventManager.ComparatorExit += RemoveComparator;

        EventManager.VariableEnter += InsertVariable;
        EventManager.VariableExit += RemoveVariable;
    }

    private void InsertBlock(BlockController block, GameObject line)
    {
        // Pega o index da linha e adiciona o bloco na lista
        int index = line.Equals(endLineObject) ? activeLines : lines.IndexOf(line);
        blocks.Insert(index, block);

        // Cria a linha para o bloco
        GameObject newLine = Instantiate(lineObjectPrefab, linesContent);
        lines.Insert(index, newLine);
        activeLines++;
        linesLayout.Insert(index, newLine.GetComponent<HorizontalLayoutGroup>());
        newLine.transform.SetSiblingIndex(index);

        // Adiciona o bloco na linha
        block.SetParent(newLine.GetComponent<RectTransform>());
        block.isInPanel = true;

        OrganizeBlocks();
    }

    private void RemoveBlock(BlockController block)
    {
        // Verifica se o bloco está no painel
        if (!block.isInPanel)
            return;
        block.isInPanel = false;
        // Remove da lista de blocos
        int index = blocks.IndexOf(block);
        blocks.RemoveAt(index);
 
        // Remove a linha correspondente do painel
        GameObject line = lines[index];
        lines.RemoveAt(index);
        linesLayout.RemoveAt(index);
        Destroy(line);
        activeLines--;

        OrganizeBlocks();
    }

    private void InsertCode(BlockController code, GameObject line)
    {
        // Pega o index da linha e adiciona o bloco na lista
        int index = line.Equals(endLineObject) ? activeLines : lines.IndexOf(line);
        blocks.Insert(index, code);

        // Cria a linha para o bloco
        GameObject newLine = Instantiate(lineObjectPrefab, linesContent);
        lines.Insert(index, newLine);
        activeLines++;
        linesLayout.Insert(index, newLine.GetComponent<HorizontalLayoutGroup>());
        newLine.transform.SetSiblingIndex(index);

        // Adiciona o bloco na linha
        code.SetParent(newLine.GetComponent<RectTransform>());
        code.isInPanel = true;

        OrganizeBlocks();
    }

    private void RemoveCode(BlockController code)
    {
        // Verifica se o bloco está no painel
        if (!code.isInPanel)
            return;
        code.isInPanel = false;

        // Remove da lista de blocos
        int index = blocks.IndexOf(code);
        blocks.RemoveAt(index);

        // Remove a linha correspondente do painel
        GameObject line = lines[index];
        lines.RemoveAt(index);
        linesLayout.RemoveAt(index);
        Destroy(line);
        activeLines--;

        OrganizeBlocks();
    }

    private void InsertComparator(ComparatorController comparator, BlockSlotController blockSlot)
    {
        // Verifica se o espaco esta ocupado
        if (blockSlot.isOccupied())
        {
            Destroy(comparator.gameObject);
            return;
        }

        // Verifica se o bloco esta no painel
        if (!blockSlot.isInPanel)
        {
            Destroy(comparator.gameObject);
            return;
        }

        // Define parent de comparador
        comparator.isInPanel = true;
        Transform blockSlotTransform = blockSlot.GetComponent<RectTransform>();
        comparator.SetParent(blockSlotTransform);

        // Define espaco parent de comparador como blockSlot
        comparator.structureSlot = blockSlot;
        // E espaco child de blockSlot como comparador
        blockSlot.setChildBlock(comparator);
    }

    private void RemoveComparator(ComparatorController comparator)
    {
        if (!comparator.isInPanel)
            return;
        comparator.isInPanel = false;

        comparator.structureSlot.removeChildBlock();
    }

    private void InsertVariable(VariableController variable, BlockSlotController variableSlot)
    {

        // Debug.Log("aaaaaaaa" + variable);
        if (variableSlot.isOccupied())
        {
            Destroy(variable.gameObject);
            return;
        }

        if (!variableSlot.isInPanel)
        {
            Destroy(variable.gameObject);
            return;
        }

        // Define parent da variavel
        variable.isInPanel = true;
        Transform variableSlotTransform = variableSlot.GetComponent<RectTransform>();
        variable.SetParent(variableSlotTransform);

        variable.blockSlot = variableSlot;
        variableSlot.setChildBlock(variable);
    }

    private void RemoveVariable(VariableController variable)
    {
        // Verifica se o bloco esta no painel
        if (!variable.isInPanel)
            return;
        variable.isInPanel = false;

        variable.blockSlot.removeChildBlock();
    }

    private void OrganizeBlocks()
    {
        string blocksPrint = "BLOCKS ARRAY\n";
        int leftPadding = minPadding;
        float maxWidth = minLineWidth;
        for (int i = 0; i < lines.Count; i++)
        {
            GameObject blockGameObject = blocks[i].gameObject;
            HorizontalLayoutGroup line = linesLayout[i];
            blocksPrint += $"{blockGameObject.name}\n";

            if (blockGameObject.CompareTag("ElseBlock") || blockGameObject.CompareTag("EndBlock"))
                leftPadding -= tabPadding;
            leftPadding = Mathf.Max(leftPadding, minPadding);

            RectOffset padding = new RectOffset(
                leftPadding,
                linePadding.right,
                linePadding.top,
                linePadding.bottom
            );
            line.padding = padding;

            RectTransform blockTransform = blockGameObject.GetComponent<RectTransform>();
            maxWidth = Mathf.Max(
                maxWidth,
                leftPadding + blockTransform.sizeDelta.x * blockTransform.localScale.x
            );

            if (
                blockGameObject.CompareTag("ElseBlock")
                || blockGameObject.CompareTag("StructureBlock")
                || blockGameObject.CompareTag("ForBlock")
            )
                leftPadding += tabPadding;
        }

        foreach (GameObject line in lines)
        {
            Vector2 transformSize = new Vector2(maxWidth, lineHeight);
            line.GetComponent<RectTransform>().sizeDelta = transformSize;
        }
        endLineSize.x = maxWidth;
        endLineObject.GetComponent<RectTransform>().sizeDelta = endLineSize;
    }

    public void LoadCommands(List<List<Commands>> commands)
    {
        if (commands == null || commands.Count == 0)
            return;
        Clear();
        foreach (List<Commands> lineCommands in commands)
        {
            Commands mainCommand = lineCommands[0];
            switch (mainCommand)
            {
                case Commands.ELSE:
                    GameObject elseBlock = InstantiateStructure(mainCommand);
                    InsertBlock(elseBlock.GetComponent<ElseController>(), endLineObject);
                    break;
                case Commands.BREAK:
                    GameObject breakBlock = InstantiateStructure(mainCommand);
                    InsertBlock(breakBlock.GetComponent<BreakController>(), endLineObject);
                    break;
                case Commands.END:
                    GameObject endBlock = InstantiateStructure(mainCommand);
                    InsertBlock(endBlock.GetComponent<EndController>(), endLineObject);
                    break;
                case Commands.FOR:
                    GameObject forBlock = InstantiateStructure(mainCommand);
                    ForController forController = forBlock.GetComponent<ForController>();
                    InsertBlock(forController, endLineObject);

                    GameObject forVariable = InstantiateVariable(lineCommands[1]);
                    InsertVariable(
                        forVariable.GetComponent<VariableController>(),
                        forController.variableSlot
                    );
                    break;
                case Commands.IF:
                case Commands.WHILE:
                    GameObject structureBlock = InstantiateStructure(mainCommand);
                    StructureController structureController =
                        structureBlock.GetComponent<StructureController>();
                    InsertBlock(structureController, endLineObject);

                    GameObject comparator = InstantiateComparator(lineCommands[1]);
                    ComparatorController comparatorController =
                        comparator.GetComponent<ComparatorController>();
                    InsertComparator(comparatorController, structureController.comparatorSlot);

                    switch (lineCommands[1])
                    {
                        case Commands.TRUE:
                            break;
                        case Commands.EVEN:
                            GameObject variable = InstantiateVariable(lineCommands[2]);
                            InsertVariable(
                                variable.GetComponent<VariableController>(),
                                comparatorController.variableSlot1
                            );
                            break;
                        default:
                            GameObject variable1 = InstantiateVariable(lineCommands[2]);
                            InsertVariable(
                                variable1.GetComponent<VariableController>(),
                                comparatorController.variableSlot1
                            );

                            GameObject variable2 = InstantiateVariable(lineCommands[3]);
                            InsertVariable(
                                variable2.GetComponent<VariableController>(),
                                comparatorController.variableSlot2
                            );
                            break;
                    }
                    break;
                default:
                    GameObject actionBlock = InstantiateAction(mainCommand);
                    InsertBlock(actionBlock.GetComponent<BlockController>(), endLineObject);
                    break;
            }
        }
        OrganizeBlocks();
    }

    private void SetBlockAsOld(GameObject block)
    {
        block.GetComponent<BlockController>().SetInPanel();
    }

    public GameObject InstantiateAction(Commands command)
    {
        GameObject action = null;
        switch (command)
        {
            case Commands.ATTACK:
                action = Instantiate(actionsPrefabs.attackPrefab, canvas);
                break;
            case Commands.DEFEND:
                action = Instantiate(actionsPrefabs.defendPrefab, canvas);
                break;
            case Commands.CHARGE:
                action = Instantiate(actionsPrefabs.chargePrefab, canvas);
                break;
            case Commands.HEAL:
                action = Instantiate(actionsPrefabs.healPrefab, canvas);
                break;
            default:
                return null;
        }
        SetBlockAsOld(action);
        return action;
    }

    public GameObject InstantiateCode(Commands command)
    {
        GameObject action = null;
        switch (command)
        {
            case Commands.CODE:
                action = Instantiate(codePrefabs.codePrefab, canvas);
                break;
            // case Commands.DEFEND:
            //     action = Instantiate(actionsPrefabs.defendPrefab, canvas);
            //     break;
            // case Commands.CHARGE:
            //     action = Instantiate(actionsPrefabs.chargePrefab, canvas);
            //     break;
            // case Commands.HEAL:
            //     action = Instantiate(actionsPrefabs.healPrefab, canvas);
            //     break;
            default:
                return null;
        }
        SetBlockAsOld(action);
        return action;
    }

    public GameObject InstantiateStructure(Commands command)
    {
        GameObject structure = null;
        switch (command)
        {
            case Commands.IF:
                structure = Instantiate(structuresPrefabs.ifPrefab, canvas);
                break;
            case Commands.ELSE:
                structure = Instantiate(structuresPrefabs.elsePrefab, canvas);
                break;
            case Commands.WHILE:
                structure = Instantiate(structuresPrefabs.whilePrefab, canvas);
                break;
            case Commands.FOR:
                structure = Instantiate(structuresPrefabs.forPrefab, canvas);
                break;
            case Commands.END:
                structure = Instantiate(structuresPrefabs.endPrefab, canvas);
                break;
            case Commands.BREAK:
                structure = Instantiate(structuresPrefabs.breakPrefab, canvas);
                break;
            default:
                return null;
        }
        SetBlockAsOld(structure);
        return structure;
    }

    public GameObject InstantiateComparator(Commands command)
    {
        GameObject comparator = null;
        switch (command)
        {
            case Commands.TRUE:
                comparator = Instantiate(comparatorsPrefabs.truePrefab, canvas);
                break;
            case Commands.EQUALS:
                comparator = Instantiate(comparatorsPrefabs.equalsPrefab, canvas);
                break;
            case Commands.NOT_EQUALS:
                comparator = Instantiate(comparatorsPrefabs.notEqualsPrefab, canvas);
                break;
            case Commands.GREATER:
                comparator = Instantiate(comparatorsPrefabs.greaterPrefab, canvas);
                break;
            case Commands.EVEN:
                comparator = Instantiate(comparatorsPrefabs.evenPrefab, canvas);
                break;
            default:
                return null;
        }
        SetBlockAsOld(comparator);
        return comparator;
    }

    public GameObject InstantiateVariable(Commands command)
    {
        GameObject variable = null;
        switch (command)
        {
            case Commands.PLAYER_ACTUAL_HEALTH:
                variable = Instantiate(variablesPrefabs.player.health.currentPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_HEALTH_HALF:
                variable = Instantiate(variablesPrefabs.player.health.halfPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_HEALTH_DOUBLE:
                variable = Instantiate(variablesPrefabs.player.health.doublePrefab, canvas);
                break;
            case Commands.PLAYER_DAMAGE:
                variable = Instantiate(variablesPrefabs.player.damage.currentPrefab, canvas);
                break;
            case Commands.PLAYER_DAMAGE_HALF:
                variable = Instantiate(variablesPrefabs.player.damage.halfPrefab, canvas);
                break;
            case Commands.PLAYER_DAMAGE_DOUBLE:
                variable = Instantiate(variablesPrefabs.player.damage.doublePrefab, canvas);
                break;
            case Commands.PLAYER_MAX_HEALTH:
                variable = Instantiate(variablesPrefabs.player.maxHealth.currentPrefab, canvas);
                break;
            case Commands.PLAYER_MAX_HEALTH_HALF:
                variable = Instantiate(variablesPrefabs.player.maxHealth.halfPrefab, canvas);
                break;
            case Commands.PLAYER_MAX_HEALTH_DOUBLE:
                variable = Instantiate(variablesPrefabs.player.maxHealth.doublePrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_SHIELD:
                variable = Instantiate(variablesPrefabs.player.defense.currentPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_SHIELD_HALF:
                variable = Instantiate(variablesPrefabs.player.defense.halfPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_SHIELD_DOUBLE:
                variable = Instantiate(variablesPrefabs.player.defense.doublePrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_CHARGE:
                variable = Instantiate(variablesPrefabs.player.charge.currentPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_CHARGE_HALF:
                variable = Instantiate(variablesPrefabs.player.charge.halfPrefab, canvas);
                break;
            case Commands.PLAYER_ACTUAL_CHARGE_DOUBLE:
                variable = Instantiate(variablesPrefabs.player.charge.doublePrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_HEALTH:
                variable = Instantiate(variablesPrefabs.enemy.health.currentPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_HEALTH_HALF:
                variable = Instantiate(variablesPrefabs.enemy.health.halfPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_HEALTH_DOUBLE:
                variable = Instantiate(variablesPrefabs.enemy.health.doublePrefab, canvas);
                break;
            case Commands.ENEMY_DAMAGE:
                variable = Instantiate(variablesPrefabs.enemy.damage.currentPrefab, canvas);
                break;
            case Commands.ENEMY_DAMAGE_HALF:
                variable = Instantiate(variablesPrefabs.enemy.damage.halfPrefab, canvas);
                break;
            case Commands.ENEMY_DAMAGE_DOUBLE:
                variable = Instantiate(variablesPrefabs.enemy.damage.doublePrefab, canvas);
                break;
            case Commands.ENEMY_MAX_HEALTH:
                variable = Instantiate(variablesPrefabs.enemy.maxHealth.currentPrefab, canvas);
                break;
            case Commands.ENEMY_MAX_HEALTH_HALF:
                variable = Instantiate(variablesPrefabs.enemy.maxHealth.halfPrefab, canvas);
                break;
            case Commands.ENEMY_MAX_HEALTH_DOUBLE:
                variable = Instantiate(variablesPrefabs.enemy.maxHealth.doublePrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_SHIELD:
                variable = Instantiate(variablesPrefabs.enemy.defense.currentPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_SHIELD_HALF:
                variable = Instantiate(variablesPrefabs.enemy.defense.halfPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_SHIELD_DOUBLE:
                variable = Instantiate(variablesPrefabs.enemy.defense.doublePrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_CHARGE:
                variable = Instantiate(variablesPrefabs.enemy.charge.currentPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_CHARGE_HALF:
                variable = Instantiate(variablesPrefabs.enemy.charge.halfPrefab, canvas);
                break;
            case Commands.ENEMY_ACTUAL_CHARGE_DOUBLE:
                variable = Instantiate(variablesPrefabs.enemy.charge.doublePrefab, canvas);
                break;
            case Commands.ROUND:
                variable = Instantiate(variablesPrefabs.roundPrefab, canvas);
                break;
            case Commands.ZERO:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[0], canvas);
                break;
            case Commands.ONE:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[1], canvas);
                break;
            case Commands.TWO:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[2], canvas);
                break;
            case Commands.THREE:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[3], canvas);
                break;
            case Commands.FOUR:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[4], canvas);
                break;
            case Commands.FIVE:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[5], canvas);
                break;
            case Commands.SIX:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[6], canvas);
                break;
            case Commands.SEVEN:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[7], canvas);
                break;
            case Commands.EIGHT:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[8], canvas);
                break;
            case Commands.NINE:
                variable = Instantiate(variablesPrefabs.numbersPrefabs[9], canvas);
                break;
            default:
                return null;
        }
        SetBlockAsOld(variable);
        return variable;
    }

    public void Clear()
    {
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
        activeLines = 0;
        lines.Clear();
        blocks.Clear();
        OrganizeBlocks();
    }

    public void KillEvents()
    {
        EventManager.BlockEnter -= InsertBlock;
        EventManager.BlockExit -= RemoveBlock;

        EventManager.CodeEnter -= InsertCode;
        EventManager.CodeExit -= RemoveCode;

        EventManager.ComparatorEnter -= InsertComparator;
        EventManager.ComparatorExit -= RemoveComparator;

        EventManager.VariableEnter -= InsertVariable;
        EventManager.VariableExit -= RemoveVariable;
    }
}
