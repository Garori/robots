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

    // public bool hasWhileTrue = false;

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
        foreach (Cell cell in memory)
        {
            if (cell != null) cell.ResetCell(); 
        }
    }

    public List<List<Commands>> GetCommands(List<BlockController> blocks)
    {
        List<List<Commands>> commands = new List<List<Commands>>();
        foreach (BlockController block in blocks)
        {
            List<Commands> lineCommands = new List<Commands>();
            lineCommands.Add(block.commandName);
            switch (block)
            {
                case ActionController c:
                    break;
                case StructureController c:
                    lineCommands.Add(c.GetComparatorCommand());
                    switch (c.GetComparatorCommand())
                    {
                        case Commands.TRUE:
                            // hasWhileTrue = true;
                            break;
                        case Commands.EVEN:
                            lineCommands.Add(c.GetVariable1Command());
                            break;
                        default:
                            lineCommands.Add(c.GetVariable1Command());
                            lineCommands.Add(c.GetVariable2Command());
                            break;
                    }
                    break;
                case ForController c:
                    lineCommands.Add(c.GetVariableCommand());
                    break;
            }
            commands.Add(lineCommands);
        }
        return commands;
    }

    public bool Compile(List<BlockController> blocks, ref string compileResult)
    {
        List<List<Commands>> commands = GetCommands(blocks);
        return Compile(commands, ref compileResult);
    }

    public bool Compile(List<List<Commands>> blockCommands, ref string compileResult)
    {
        if (blockCommands.Count > maxBlocks)
        {
            compileResult = $"ERRO DE COMPILAÇÃO: Não pode haver mais de {maxBlocks} blocos";
            return false;
        }
        Stack<int> structuresStack = new Stack<int>();
        bool hasAction = false;

        ResetAttributes();
        foreach (List<Commands> lineCommands in blockCommands)
        {
            PC++;
            Commands mainCommand = lineCommands[0];
            switch (mainCommand)
            {
                case Commands.ATTACK:
                case Commands.DEFEND:
                case Commands.CHARGE:
                case Commands.DODGE:
                case Commands.HEAL:
                    memory[PC] = new ActionCell(mainCommand);
                    hasAction = true;
                    continue;
            }
            if (mainCommand == Commands.END)
            {
                if (structuresStack.Count == 0)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco END sem estrutura correspondente";
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
            if (mainCommand == Commands.ELSE)
            {
                if (structuresStack.Count == 0)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco ELSE sem IF correspondente";
                    return false;
                }

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell))
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco ELSE sem IF correspondente";
                    return false;
                }

                lastStructure.jmp = PC - lastStructureIndex;
                memory[PC] = new ElseCell();
                structuresStack.Push(PC);

                continue;
            }
            if (mainCommand == Commands.FOR)
            {
                Commands variableName = lineCommands[1];
                if (variableName == Commands.NONE)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco FOR sem número de repetições";
                    return false;
                }

                if (variableName == Commands.ZERO)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco FOR deve ter número de repetições maior que zero";
                    return false;
                }

                memory[PC] = new ForCell(variableName);
                structuresStack.Push(PC);
                continue;
            }

            Commands comparatorCommand = lineCommands[1];
            if (comparatorCommand == Commands.NONE)
            {
                compileResult = "ERRO DE COMPILAÇÃO: WHILE ou IF sem comparador";
                return false;
            }
            ComparatorCell comparatorCell = null;

            switch (comparatorCommand)
            {
                case Commands.TRUE:
                    comparatorCell = new TrueCell();
                    if(mainCommand == Commands.WHILE)
                    {
                        structuresStack.Push(PC);
                    }
                    break;
                case Commands.EVEN:
                    Commands variableName = lineCommands[2];
                    if (variableName == Commands.NONE)
                    {
                        compileResult = "ERRO DE COMPILAÇÃO: Comparador EVEN sem variável";
                        return false;
                    }

                    comparatorCell = new EvenCell(variableName);
                    break;
                default:
                    Commands variable1Name = lineCommands[2];
                    Commands variable2Name = lineCommands[3];
                    if (variable1Name == Commands.NONE || variable2Name == Commands.NONE)
                    {
                        compileResult = $"ERRO DE COMPILAÇÃO: {comparatorCommand} comparador sem variável(s)";
                        return false;
                    }

                    switch (comparatorCommand)
                    {
                        case Commands.EQUALS:
                            comparatorCell = new EqualsCell(variable1Name, variable2Name);
                            break;
                        case Commands.NOT_EQUALS:
                            comparatorCell = new NotEqualsCell(variable1Name, variable2Name);
                            break;
                        case Commands.GREATER:
                            comparatorCell = new GreaterCell(variable1Name, variable2Name);
                            break;
                    }
                    break;
            }

            IConditionCell structureStart = null;
            if (mainCommand == Commands.IF)
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
            compileResult = "ERRO DE COMPILAÇÃO: A estrutura não foi fechada corretamente";
            // compileResult = "ERRO DE COMPILAÇÃO: A estrutura não foi fechada corretamente ou existe um While TRUE sem break";
            return false;
        }
        if (!hasAction)
        {
            compileResult = "ERRO DE COMPILAÇÃO: Não há nenhuma ação no código";
            return false;
        }
        totalCells = PC + 1;
        ResetAttributes();
        compileResult = "COMPILAÇÃO BEM SUCEDIDA!!!";
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
            Debug.Log($"Entering cell {cell} at index {PC}");

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
