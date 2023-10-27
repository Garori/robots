using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compiler : MonoBehaviour
{
    [SerializeField] private int maxIterations = 10;
    [SerializeField] private int maxBlocks = 100;

    private int PC;

    private Cell[] memory;
    public Cell[] Memory { get => memory; }
    private int totalCells;
    public int TotalCells { get => totalCells; }
    [SerializeField] private bool debug;

    private void Start()
    {
        memory = new Cell[maxBlocks * 2];
        ResetAttributes();
    }

    public void Compile(Cell[] memory)
    {
        this.memory = memory;
        totalCells = memory.Length;
    }

    public void ResetAttributes()
    {
        PC = -1;
    }

    public bool Compile(List<BlockController> blocks, ref string compileResult)
    {
        if (blocks.Count > maxBlocks)
        {
            compileResult = $"COMPILATION ERROR: Can't have more than {maxBlocks} blocks";
            return false;
        }
        Stack<int> structuresStack = new Stack<int>();
        bool hasAction = false;

        ResetAttributes();
        foreach (BlockController block in blocks)
        {
            PC++;
            Commands command = block.commandName;
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

                lastStructure.jmp = PC - lastStructureIndex;
                memory[PC] = new ElseCell();
                structuresStack.Push(PC);

                continue;
            }
            if (command == Commands.FOR)
            {
                // Log the type of block object
                VariableController variableController = ((block as ForController).variableSlot.childBlock) as VariableController;
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
            ComparatorController comparatorController = ((block as StructureController).comparatorSlot.childBlock) as ComparatorController;
            if (comparatorController == null)
            {
                compileResult = "COMPILATION ERROR: WHILE or IF block without condition";
                return false;
            }

            Commands comparatorCommand = comparatorController.commandName;

            ComparatorCell comparatorCell = null;

            switch (comparatorCommand)
            {
                case Commands.TRUE:
                    comparatorCell = new TrueCell();
                    break;
                case Commands.EVEN:
                    VariableController variableController = comparatorController.variableSlot1.childBlock as VariableController;
                    if (variableController == null)
                    {
                        compileResult = "COMPILATION ERROR: EVEN comparator without variable";
                        return false;
                    }

                    comparatorCell = new EvenCell(variableController.commandName);
                    break;
                default:
                    VariableController variable1Controller = comparatorController.variableSlot1.childBlock as VariableController;
                    VariableController variable2Controller = comparatorController.variableSlot2.childBlock as VariableController;
                    if (variable1Controller == null || variable2Controller == null)
                    {
                        compileResult = $"COMPILATION ERROR: {comparatorController.gameObject.name.ToUpper()} comparator without variables";
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
                    break;
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
        ResetAttributes();
        compileResult = "COMPILED SUCCESSFULLY!!!";
        return true;
    }

    public Commands Run(BattleStatus status)
    {
        for (int iter = 0; iter < maxIterations; iter++)
        {
            PC++;
            if (PC >= totalCells) throw new PlayerOutOfActionsException();
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
                case ElseCell c:
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
