using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compiler : MonoBehaviour {
    [SerializeField] private int maxIterations = 10;
    [SerializeField] private int maxBlocks = 100;

    private int PC;

    private Cell[] memory;
    private int totalCells;

    private void Start() {
        memory = new Cell[maxBlocks * 2];
    }

    public string Compile(List<GameObject> blocks) {
        if (blocks.Count > maxBlocks) {
            return "ERRO DE COMPILAÇÃO: Passou do máximo de blocos";
        }
        Stack<int> structuresStack = new Stack<int>();
        PC = -1;

        int i = -1;
        foreach (GameObject block in blocks) {
            i++;
            Commands command = block.GetComponent<BlockController>().commandName;
            switch (command) {
                case Commands.ATTACK:
                case Commands.DEFEND:
                case Commands.CHARGE:
                case Commands.DODGE:
                case Commands.HEAL:
                    memory[i] = new ActionCell(command);
                    continue;
            }
            if (command == Commands.END) {
                if (structuresStack.Count == 0) {
                    return "ERRO DE COMPILAÇÃO: Bloco END sem estrutura correspondente";
                }
                memory[i] = new EndCell(0);

                int lastStructureIndex = structuresStack.Pop();
                Debug.Log(lastStructureIndex);
                Cell lastStructure = memory[lastStructureIndex];
                lastStructure.jmp = i - lastStructureIndex;

                if (lastStructure is IfCell || lastStructure is ElseCell) continue;

                memory[i].jmp = lastStructureIndex - i - 1;
                continue;
            }
            if (command == Commands.ELSE) {
                if (structuresStack.Count == 0) {
                    return "ERRO DE COMPILAÇÃO: Bloco ELSE começando condição";
                }

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell)) {
                    return "ERRO DE COMPILAÇÃO: Bloco ELSE sem bloco IF correspondente";
                }

                memory[i] = new EndCell(0);
                lastStructure.jmp = i - lastStructureIndex;

                i++;
                memory[i] = new ElseCell(((IfCell)lastStructure).comparatorCell);
                structuresStack.Push(i);

                continue;
            }
            if (command == Commands.FOR) {
                return "ERRO DE COMPILAÇÃO: Bloco FOR ainda não foi implementado";
            }
            ComparatorController comparatorController = block.GetComponentInChildren<ComparatorController>();
            if (comparatorController == null) {
                return "ERRO DE COMPILAÇÃO: Bloco WHILE ou IF sem condição";
            }

            Commands comparatorCommand = comparatorController.commandName;
            Transform comparatorTransform = comparatorController.gameObject.GetComponent<RectTransform>();

            ComparatorCell comparatorCell = null;

            if (comparatorCommand == Commands.EVEN) {
                VariableController variableController = comparatorTransform.GetComponentInChildren<VariableController>();
                if (variableController == null) {
                    return "ERRO DE COMPILAÇÃO: Comparador EVEN sem variável";
                }

                comparatorCell = new EvenCell(variableController.commandName);
            } else {
                VariableController variable1Controller = comparatorTransform.GetChild(0).GetComponentInChildren<VariableController>();
                VariableController variable2Controller = comparatorTransform.GetChild(1).GetComponentInChildren<VariableController>();
                if (variable1Controller == null || variable2Controller == null) {
                    return $"ERRO DE COMPILAÇÃO: Comparador {comparatorTransform.gameObject.name.ToUpper()} sem variáveis";
                }

                switch (comparatorCommand) {
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
            if (command == Commands.IF) {
                structureStart = new IfCell(comparatorCell);
            } else {
                structureStart = new WhileCell(comparatorCell);
            }
            memory[i] = (Cell)structureStart;
            structuresStack.Push(i);
        }
        if (structuresStack.Count != 0) {
            return "ERRO DE COMPILAÇÃO: As estruturas não foram todas devidamente finalizadas";
        }
        totalCells = i + 1;
        return "COMPILADO COM SUCESSO!!!";
    }

    public Commands Run(BattleStatus status) {
        for (int iter = 0; iter < maxIterations; iter++) {
            PC = (PC + 1) % totalCells;
            Cell cell = memory[PC];

            switch (cell) {
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

    private void JumpCond(IConditionCell cell, BattleStatus status) {
        if (!cell.Evaluate(status)) Jump((Cell)cell);
    }

    private void Jump(Cell cell) {
        PC += cell.jmp;
    }
}
