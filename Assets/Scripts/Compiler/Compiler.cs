using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compiler : MonoBehaviour {
    [SerializeField] private int maxIterations = 100;
    [SerializeField] private int maxBlocks = 100;
    private int PC;
    [SerializeField] private Cell[] memory;

    private List<ComparatorCell> conditions;

    public bool Compile(List<GameObject> blocks) {
        if (blocks.Count > maxBlocks) return false;
        Stack<int> structuresStack = new Stack<int>();
        conditions = new List<ComparatorCell>();
        int conditionsIndex = 0;

        int i = -1;
        foreach (GameObject block in blocks) {
            i++;
            Commands command = block.GetComponent<BlockController>().commandName;
            switch (command) {
                case Commands.Attack:
                case Commands.Defend:
                case Commands.Charge:
                case Commands.Dodge:
                case Commands.Heal:
                    memory[i] = new ActionCell(command);
                    continue;
            }
            if (command == Commands.End) {
                if (structuresStack.Count == 0) return false;
                memory[i] = new EndCell(0);

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                lastStructure.jmp = i - lastStructureIndex;

                if (lastStructure is IfCell) continue;

                memory[i].jmp = lastStructureIndex - i - 1;
                continue;
            }
            if (command == Commands.Else) {
                if (structuresStack.Count == 0) return false;

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell)) return false;

                memory[i] = new EndCell(0);
                lastStructure.jmp = i - lastStructureIndex;

                i++;
                memory[i] = new ElseCell(((IfCell)lastStructure).condId);
                continue;
            }
            if (command == Commands.For) {
                return false;
            }
            ComparatorController comparatorController = block.GetComponentInChildren<ComparatorController>();
            if (comparatorController == null) return false;

            Commands comparatorCommand = comparatorController.commandName;
            Transform comparatorTransform = comparatorController.gameObject.GetComponent<RectTransform>();

            ComparatorCell comparatorCell = null;

            if (comparatorCommand == Commands.Even) {
                VariableController variableController = comparatorTransform.GetComponentInChildren<VariableController>();
                if (variableController == null) return false;

                comparatorCell = new EvenCell(variableController.commandName);
            } else {
                VariableController variable1Controller = comparatorTransform.GetChild(0).GetComponentInChildren<VariableController>();
                VariableController variable2Controller = comparatorTransform.GetChild(1).GetComponentInChildren<VariableController>();
                if (variable1Controller == null || variable2Controller == null) return false;

                switch (comparatorCommand) {
                    case Commands.Equals:
                        comparatorCell = new EqualsCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                    case Commands.NotEquals:
                        comparatorCell = new NotEqualsCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                    case Commands.Greater:
                        comparatorCell = new GreaterCell(variable1Controller.commandName, variable2Controller.commandName);
                        break;
                }
            }
            conditions.Add(comparatorCell);

            Cell structureStart = null;
            if (command == Commands.If) {
                structureStart = new IfCell(conditionsIndex);
            } else {
                new WhileCell(conditionsIndex);
            }
            memory[i] = structureStart;
            structuresStack.Push(i);

            conditionsIndex++;
        }
        memory[i + 1] = null;
        return true;
    }
}
