using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compiler : MonoBehaviour
{
    [SerializeField] private int maxIterations = 10;
    [SerializeField] private int maxBlocks = 100;

    private int PC;

    private Cell[] memory;
    private int totalCells;
    [SerializeField] private bool debug;

    private void Start()
    {
        memory = new Cell[maxBlocks * 2];
        PC = -1;
    }

    public void Compile(Cell[] memory)
    {
        this.memory = memory;
        totalCells = memory.Length;
    }

    public bool Compile(List<GameObject> blocks, ref string compileResult)
    {
        if (blocks.Count > maxBlocks)
        {
            compileResult = $"COMPILATION ERROR: Can't have more than {maxBlocks} blocks";
            return false;
        }
        Stack<int> structuresStack = new Stack<int>();
        bool hasAction = false;

        PC = -1;
        foreach (GameObject block in blocks)
        {
            PC++;
            Commands command = block.GetComponent<BlockController>().commandName;
            switch (command)
            {
                case Commands.ATTACK:
                case Commands.DEFEND:
                case Commands.CHARGE:
                case Commands.DODGE:
                case Commands.HEAL:
                    memory[PC] = new ActionCell(command);
                    hasAction = true;
                    continue;
            }
            if (command == Commands.END)
            {
                if (structuresStack.Count == 0)
                {
                    compileResult = "COMPILATION ERROR: END block without corresponding structure";
                    return false;
                }
                memory[PC] = new EndCell();

                int lastStructureIndex = structuresStack.Pop();
                Debug.Log(lastStructureIndex);
                Cell lastStructure = memory[lastStructureIndex];
                lastStructure.jmp = PC - lastStructureIndex;

                if (lastStructure is IfCell || lastStructure is ElseCell) continue;

                memory[PC].jmp = lastStructureIndex - PC - 1;
                continue;
            }
            if (command == Commands.ELSE)
            {
                if (structuresStack.Count == 0)
                {
                    compileResult = "COMPILATION ERROR: ELSE block without corresponding IF";
                    return false;
                }

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell))
                {
                    compileResult = "COMPILATION ERROR: ELSE block without corresponding IF";
                    return false;
                }

                memory[PC] = new EndCell();
                lastStructure.jmp = PC - lastStructureIndex;

                PC++;
                memory[PC] = new ElseCell(((IfCell)lastStructure).comparatorCell);
                structuresStack.Push(PC);

                continue;
            }
            if (command == Commands.FOR)
            {
                VariableController variableController = block.GetComponentInChildren<VariableController>();
                if (variableController == null)
                {
                    compileResult = "COMPILATION ERROR: FOR block without number";
                    return false;
                }

                Commands variableName = variableController.commandName;

                if (variableName == Commands.ZERO)
                {
                    compileResult = "COMPILATION ERROR: FOR block must have at least 1";
                    return false;
                }

                memory[PC] = new ForCell(variableName);
                structuresStack.Push(PC);
                continue;
            }
            ComparatorController comparatorController = block.GetComponentInChildren<ComparatorController>();
            if (comparatorController == null)
            {
                compileResult = "COMPILATION ERROR: WHILE or IF block without condition";
                return false;
            }

            Commands comparatorCommand = comparatorController.commandName;
            Transform comparatorTransform = comparatorController.gameObject.GetComponent<RectTransform>();

            ComparatorCell comparatorCell = null;

            if (comparatorCommand == Commands.EVEN)
            {
                VariableController variableController = comparatorTransform.GetComponentInChildren<VariableController>();
                if (variableController == null)
                {
                    compileResult = "COMPILATION ERROR: EVEN comparator without variable";
                    return false;
                }

                comparatorCell = new EvenCell(variableController.commandName);
            }
            else
            {
                VariableController variable1Controller = comparatorTransform.GetChild(0).GetComponentInChildren<VariableController>();
                VariableController variable2Controller = comparatorTransform.GetChild(1).GetComponentInChildren<VariableController>();
                if (variable1Controller == null || variable2Controller == null)
                {
                    compileResult = $"COMPILATION ERROR: {comparatorTransform.gameObject.name.ToUpper()} comparator without variables";
                    return false;
                }

                switch (comparatorCommand)
                {
                    case Commands.EQUALS:
                        comparatorCell = new EqualsCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                    case Commands.NOT_EQUALS:
                        comparatorCell = new NotEqualsCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                    case Commands.GREATER:
                        comparatorCell = new GreaterCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                }
            }

            IConditionCell structureStart = null;
            if (command == Commands.IF)
            {
                structureStart = new IfCell(comparatorCell);
            }
            else
            {
                structureStart = new WhileCell(comparatorCell);
            }
            memory[PC] = (Cell)structureStart;
            structuresStack.Push(PC);
        }
        if (structuresStack.Count != 0)
        {
            compileResult = "COMPILATION ERROR: The structures were not properly ended";
            return false;
        }
        if (!hasAction)
        {
            compileResult = "COMPILATION ERROR: There isn't an action to be done";
            return false;
        }
        totalCells = PC + 1;
        compileResult = "COMPILED SUCCESSFULLY!!!";
        return true;
    }

    public Commands Run(BattleStatus status)
    {
        for (int iter = 0; iter < maxIterations; iter++)
        {
            PC = (PC + 1) % totalCells;
            Cell cell = memory[PC];
            if (debug) Debug.Log($"Entering cell {cell} at index {PC}");

            switch (cell)
            {
                case ActionCell c:
                    return c.action;
                case IConditionCell c:
                    JumpCond(c, status);
                    break;
                case EndCell c:
                    Jump(c);
                    break;
            }
        }
        throw new ActionTookTooLongException();
    }

    private void JumpCond(IConditionCell cell, BattleStatus status)
    {
        if (!cell.Evaluate(status)) Jump((Cell)cell);
    }

    private void Jump(Cell cell)
    {
        if (debug) Debug.Log($"Jumping {cell.jmp} cells");
        PC += cell.jmp;
    }
}
