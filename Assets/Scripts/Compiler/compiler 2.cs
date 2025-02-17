using System.ComponentModel.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Unity.Mathematics;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

public class Compiler2 : MonoBehaviour
{
    [SerializeField] private int maxIterations = 10;
    [SerializeField] private int maxBlocks = 100;

    private int PC;

    private Stack<string> runStructureStack = new Stack<string>();


    private Cell[] memory;
    public Cell[] Memory { get => memory; }
    private bool currentlyWhileLoop;
    public bool CurrentlyWhileTrue { get => currentlyWhileLoop; }

    public string aaaa = "";
    private BattleManager battleManager;

    public BattleManager BattleManager { get => battleManager; }
    private int totalCells;
    public int TotalCells { get => totalCells; }
    [SerializeField] private bool debug;

    // public bool hasWhileTrue = false;

    private void Start()
    {
        currentlyWhileLoop = false;
        memory = new Cell[maxBlocks * 2];
        ResetAttributes();
    }

    public void Compile(Cell[] memory)
    {
        currentlyWhileLoop = false;
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

    public struct Temporario
    {
        public string token;
        public List<Temporario> lista;
    }



    public List<string> GetTokensList(string cond)
    {
        Debug.Log("inicial = " + cond);
        List<string> lista = new List<string>();
        int indexAbre = (cond.IndexOf('(') != -1) ? cond.IndexOf('(') : int.MaxValue;
        int indexFecha = (cond.IndexOf(')') != -1) ? cond.IndexOf(')') : int.MaxValue;
        int indexAnd = (cond.IndexOf("&&") != -1) ? cond.IndexOf("&&") : int.MaxValue;
        int indexOr = (cond.IndexOf("||") != -1) ? cond.IndexOf("||") : int.MaxValue;
        if (indexAbre == int.MaxValue && indexFecha == int.MaxValue && indexAnd == int.MaxValue && indexOr == int.MaxValue)
        {
            lista.Add(cond);
            aaaa += cond;
            return lista;
        }

        while (indexAbre < int.MaxValue || indexFecha < int.MaxValue || indexAnd < int.MaxValue || indexOr < int.MaxValue)
        {
            int minParentesis = math.min(indexAbre, indexFecha);
            int minAndOr = math.min(indexAnd, indexOr);
            if (minAndOr < minParentesis)
            {
                Debug.Log("minadnor = " + minAndOr);
                lista.Add(cond.Substring(0, minAndOr).Trim());
                lista.Add(cond.Substring(minAndOr, 2).Trim());
                aaaa += cond.Substring(0, minAndOr).Trim() + ",";
                aaaa += cond.Substring(minAndOr, 2).Trim() + ",";
                Debug.Log("na lista: " + lista[lista.Count - 2].ToString());
                Debug.Log("na lista: " + lista[lista.Count - 1].ToString());
                cond = cond.Substring(minAndOr + 2);
            }
            else
            {
                Debug.Log("minparentesis = " + minParentesis);
                if (indexAbre < indexFecha)
                {
                    lista.Add("(");
                    cond = cond.Substring(minParentesis + 1);
                    aaaa += "(,";
                    // lista.Add(cond.Substring(minParentesis).Trim());
                    Debug.Log("na lista: " + lista[lista.Count - 1].ToString());
                }
                else
                {
                    lista.Add(cond.Substring(0, minParentesis).Trim());
                    aaaa += cond.Substring(0, minParentesis).Trim() + ",";
                    lista.Add(")");
                    aaaa += "),";
                    cond = cond.Substring(minParentesis + 1);
                    Debug.Log("na lista: " + lista[lista.Count - 2].ToString());
                    Debug.Log("na lista: " + lista[lista.Count - 1].ToString());
                    // lista.Add(cond.Substring(minParentesis).Trim());
                }
            }


            indexAbre = (cond.IndexOf('(') != -1) ? cond.IndexOf('(') : int.MaxValue;
            indexFecha = (cond.IndexOf(')') != -1) ? cond.IndexOf(')') : int.MaxValue;
            indexAnd = (cond.IndexOf("&&") != -1) ? cond.IndexOf("&&") : int.MaxValue;
            indexOr = (cond.IndexOf("||") != -1) ? cond.IndexOf("||") : int.MaxValue;
            Debug.Log("new cond = " + cond);

        }
        lista.Add(cond);
        aaaa += cond;
        Debug.Log("tamanho lista final = " + lista.Count);
        return lista;
    }

    public List<string> GetEveryCommand(List<string> lista)
    {
        List<string> listaAUX = new List<string>();
        Debug.Log("aaaaaaaaaaaaaaaa " + listaAUX.Count);
        foreach (string cond in lista)
        {
            Debug.Log("cond = " + cond);
            char[] aux = { '>', '<', '=', '!' };
            int index = cond.IndexOfAny(aux);
            Debug.Log("INDEX OF ANY  = " + index);
            if (index != -1)
            {
                string comparator = cond.Substring(index, 2);
                if (comparator[1] != '=')
                {
                    Debug.Log(comparator[0]);
                    listaAUX.Add(comparator[0].ToString());
                    listaAUX.Add(cond.Substring(0, index));
                    Debug.Log(cond.Substring(0, index));
                    listaAUX.Add(cond.Substring(index + 1));
                    Debug.Log(cond.Substring(index + 1));
                }
                else
                {
                    listaAUX.Add(comparator);
                    listaAUX.Add(cond.Substring(0, index));
                    listaAUX.Add(cond.Substring(index + 2));
                }

            }
            else
            {
                listaAUX.Add(cond);
            }
        }
        foreach (string cond in listaAUX)
        {
            aaaa += cond + ", ";
        }
        return listaAUX;

    }
    public List<List<Commands>> GetCommands(List<BlockController> blocks)
    {
        List<List<Commands>> commands = new List<List<Commands>>();
        foreach (BlockController block in blocks)
        {
            if (block.commandName == Commands.CODE)
            {
                Debug.Log("é um bloco de código");
                CodeController code = block as CodeController;
                Debug.Log(code.CodeWithin);
                foreach (string rawLine in code.CodeWithin.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string line = rawLine.Trim().ToUpper();
                    List<Commands> lineCommandsCode = new List<Commands>();
                    switch (line)
                    {
                        case "ATTACK":
                        case "DEFEND":
                        case "CHARGE":
                        case "DODGE":
                        case "HEAL":
                        case "END":
                        case "BREAK":
                        case "ELSE":
                        case "CONTINUE":
                            lineCommandsCode.Add((Commands)Enum.Parse(typeof(Commands), line));
                            break;
                        default: // FOR, IF , WHILE
                            int conditionStart = line.IndexOf('(');
                            int conditionEnd = line.LastIndexOf(')');
                            Debug.Log(conditionStart);
                            Debug.Log(conditionEnd);
                            Debug.Log(line.Substring(conditionStart + 1, conditionEnd - conditionStart - 1));

                            List<string> ListaCondicionais = new List<string>();
                            Temporario rawCondicao = new Temporario();
                            rawCondicao.token = line.Substring(conditionStart + 1, conditionEnd - conditionStart - 1);
                            Debug.Log("RAW CONDITION = " + rawCondicao.token);
                            ListaCondicionais = GetTokensList(rawCondicao.token);
                            ListaCondicionais = ListaCondicionais.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                            aaaa = "";
                            ListaCondicionais = GetEveryCommand(ListaCondicionais);
                            Debug.Log("aaaa = " + aaaa);

                            Commands mainCommand = (Commands)Enum.Parse(typeof(Commands), line.Substring(0, conditionStart).Trim());
                            Debug.Log("comando pelo método de INDEX OF: " + mainCommand);
                            // List<string> splittedLine = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            // Debug.Log("sou uma estrutura "+splittedLine[0]);
                            lineCommandsCode.Add(mainCommand);
                            // foreach (string token in splittedLine.GetRange(1, splittedLine.Count-1))
                            foreach (string token in ListaCondicionais)
                            {
                                string tokenTraduzido = "";
                                Debug.Log("token atual = " + token);
                                switch (token)
                                {
                                    case ">":
                                        tokenTraduzido = "GREATER";
                                        break;
                                    case "==":
                                        tokenTraduzido = "EQUALS";
                                        break;
                                    case "!=":
                                        tokenTraduzido = "NOT_EQUALS";
                                        break;
                                    case "TRUE":
                                        tokenTraduzido = "TRUE";
                                        break;
                                    case ">=":
                                        tokenTraduzido = "GREATER_EQUALS";
                                        break;
                                    case "<":
                                        tokenTraduzido = "LESSER";
                                        break;
                                    case "<=":
                                        tokenTraduzido = "LESSER_EQUALS";
                                        break;
                                    case "&&":
                                        tokenTraduzido = "AND";
                                        break;
                                    case "||":
                                        tokenTraduzido = "OR";
                                        break;
                                    case "(":
                                        tokenTraduzido = "OPEN_PARENTHESIS";
                                        break;
                                    case ")":
                                        tokenTraduzido = "CLOSE_PARENTHESIS";
                                        break;
                                    default:
                                        string[] numbers = { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "ZERO" };
                                        if (numbers.Contains(token)) { tokenTraduzido = token; break; }
                                        if (token[0] == 'P') tokenTraduzido += "PLAYER_";
                                        else tokenTraduzido += "ENEMY_";
                                        if (token.Substring(1, 3) == "MHP") tokenTraduzido += "MAX_HEALTH";
                                        else if (token.Substring(1, 3) == "CHP") tokenTraduzido += "ACTUAL_HEALTH";
                                        else if (token.Substring(1, 3) == "DAM") tokenTraduzido += "DAMAGE";
                                        else if (token.Substring(1, 3) == "SHI") tokenTraduzido += "ACTUAL_SHIELD";
                                        else if (token.Substring(1, 3) == "CHA") tokenTraduzido += "ACTUAL_CHARGE";
                                        else { tokenTraduzido = "ERROR"; break; }
                                        if (token[token.Count() - 1] == 'H') tokenTraduzido += "_HALF";
                                        else if (token[token.Count() - 1] == 'D') tokenTraduzido += "_DOUBLE";
                                        else if (token[token.Count() - 1] == 'C') tokenTraduzido += "";
                                        else { tokenTraduzido = "ERROR"; break; }
                                        Debug.Log("aqui = " + token);
                                        break;

                                }
                                Debug.Log(tokenTraduzido);
                                lineCommandsCode.Add((Commands)Enum.Parse(typeof(Commands), tokenTraduzido));
                            }
                            break;

                    }
                    commands.Add(lineCommandsCode);
                }
                continue;
            }
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
            print(lineCommands);
            commands.Add(lineCommands);
        }
        return commands;
    }

    // public bool Compile(List<BlockController> blocks, ref string compileResult)
    // {
    //     List<List<Commands>> commands = GetCommands(blocks);
    //     return Compile(commands, ref compileResult);
    // }

    public bool Compile(List<List<Commands>> blockCommands, ref string compileResult, ref int PC)
    {
        // lastBreakIndex = -1;
        if (blockCommands.Count > maxBlocks)
        {
            compileResult = $"ERRO DE COMPILAÇÃO: Não pode haver mais de {maxBlocks} blocos";
            return false;
        }
        Stack<int> structuresStack = new Stack<int>();
        Stack<int> breakStackIndexes = new Stack<int>();
        List<int> whilesAndForsList = new List<int> { };
        bool hasAction = false;


        ResetAttributes();
        PC = -1;
        foreach (List<Commands> lineCommands in blockCommands)
        {
            try
            {
                if (memory[PC].GetType() == typeof(AfterEndCell))
                {
                    PC--;
                }
            }
            catch { }

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
            if (mainCommand == Commands.CODE)
            {
                memory[PC] = new CodeCell();
                continue;
            }
            if (mainCommand == Commands.BREAK)
            {
                if (whilesAndForsList.Count == 0)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco BREAK sem estrutura correspondente";
                    return false;
                }
                breakStackIndexes.Push(PC);
                memory[PC] = new BreakCell();
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
                if (lastStructure is WhileCell || lastStructure is ForCell)
                {
                    whilesAndForsList.RemoveAt(whilesAndForsList.Count - 1);
                    if (breakStackIndexes.Count > 0)
                    {
                        int lastBreakIndex = breakStackIndexes.Pop();
                        memory[lastBreakIndex].jmp = PC - lastBreakIndex;
                    }

                }
                PC++;
                memory[PC] = new AfterEndCell();

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
                whilesAndForsList.Add(PC);
                Commands variableName = lineCommands[1];
                if (variableName == Commands.NONE)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco FOR sem número de repetições";
                    return false;
                }

                if (variableName == Commands.ERROR)
                {
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco FOR com variável não conhecida";
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
            //passou daqui é porque é ou if ou while
            //todo ao inves de pegar line commands[1] tenho que ver o tamanho do line commands
            //todo o line commands vai vai com tudo separadinho
            //todo fazer um for(int i == 1, i< lineCommands.Count, i++) que leia de 1 em um bloquinho
            //todo sendo '>','<','=='... lê os próximos dois 
            //todo sendo '&&','||' levanta flag de calcular quando o próximo estiver pronto
            //todo sendo '(' segura os resultados até achar um ')'
            List<ConditionalCell> conditionalList = new List<ConditionalCell>();
            Stack<int> parentesisStack = new Stack<int>();
            string lastANDOR = "";
            for (int i = 1; i < lineCommands.Count; i++)
            {

                Commands comparatorCommand = lineCommands[i];
                if (comparatorCommand == Commands.NONE)
                {
                    //todo devo ter que ajeitar isso aqui ainda
                    compileResult = "ERRO DE COMPILAÇÃO: WHILE ou IF sem comparador";
                    return false;
                }
                ConditionalCell conditionalCell = null;
                ComparatorCell comparatorCell = null;

                switch (comparatorCommand)
                {
                    case Commands.AND:
                        conditionalCell = new ConditionalCell(typeof(AndCell));
                        if (lastANDOR == "OR")
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: AND e OR não separados por parentesis para dar ordem de operações clara";
                            return false;
                        }
                        lastANDOR = "AND";
                        break;
                    case Commands.OR:
                        conditionalCell = new ConditionalCell(typeof(OrCell));
                        if (lastANDOR == "AND")
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: AND e OR não separados por parentesis para dar ordem de operações clara";
                            return false;
                        }
                        lastANDOR = "OR";
                        break;
                    case Commands.OPEN_PARENTHESIS:
                        conditionalCell = new ConditionalCell(typeof(OpenParenthesisCell));
                        parentesisStack.Push(0);
                        break;
                    case Commands.CLOSE_PARENTHESIS:
                        conditionalCell = new ConditionalCell(typeof(CloseParenthesisCell));
                        try
                        {
                            parentesisStack.Pop();
                        }
                        catch
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: Parentesis aberto sem ter sido fechado!";
                            return false;
                        }
                        break;
                    case Commands.TRUE:
                        conditionalCell = new ConditionalCell(typeof(bool), resultado: true);
                        break;
                    case Commands.EVEN:
                        if (lineCommands[i + 1] != Commands.OPEN_PARENTHESIS || lineCommands[i + 3] != Commands.CLOSE_PARENTHESIS)
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: Comparador EVEN mal formado";
                            return false;
                        }
                        Commands variableName = lineCommands[i + 2];
                        if (variableName == Commands.NONE)
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: Comparador EVEN sem variável";
                            return false;
                        }

                        if (variableName == Commands.ERROR)
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: Comparador EVEN com variável não conhecida";
                            return false;
                        }
                        i += 3;
                        comparatorCell = new EvenCell(variableName);
                        conditionalCell = new ConditionalCell(typeof(EvenCell), comparator: comparatorCell);
                        break;
                    default:
                        Commands variable1Name = lineCommands[i + 1];
                        Commands variable2Name = lineCommands[i + 2];
                        i += 2;
                        if (variable1Name == Commands.NONE || variable2Name == Commands.NONE)
                        {
                            compileResult = $"ERRO DE COMPILAÇÃO: {comparatorCommand} comparador sem variável(s)";
                            return false;
                        }

                        if (variable1Name == Commands.ERROR || variable2Name == Commands.ERROR)
                        {
                            compileResult = $"ERRO DE COMPILAÇÃO: {comparatorCommand} com variável não conhecida";
                            return false;
                        }

                        switch (comparatorCommand)
                        {
                            case Commands.EQUALS:
                                comparatorCell = new EqualsCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(EqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.NOT_EQUALS:
                                comparatorCell = new NotEqualsCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(NotEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER:
                                comparatorCell = new GreaterCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(GreaterCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER_EQUALS:
                                comparatorCell = new GreaterEqualsCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(GreaterEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER:
                                comparatorCell = new LesserCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(LesserCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER_EQUALS:
                                comparatorCell = new LesserEqualsCell(variable1Name, variable2Name);
                                conditionalCell = new ConditionalCell(typeof(LesserEqualsCell), comparator: comparatorCell);
                                break;
                        }
                        break;
                }
                Debug.Log(conditionalCell.tipo);
                conditionalList.Add(conditionalCell);
            }
            if (parentesisStack.Count > 0)
            {
                compileResult = "ERRO DE COMPILAÇÃO: Parentesis fechado sem ter sido aberto";
                return false;
            }

            IConditionCell structureStart = null;
            if (mainCommand == Commands.IF)
            {
                structureStart = new IfCell(conditionalList);
            }
            else
            {
                structureStart = new WhileCell(conditionalList);
                whilesAndForsList.Add(PC);
            }
            memory[PC] = (Cell)structureStart;
            structuresStack.Push(PC);
        }
        if (structuresStack.Count != 0)
        {
            compileResult = "ERRO DE COMPILAÇÃO: A estrutura não foi fechada corretamente";
            // compileResult = "ERRO DE COMPILAÇÃO: Uma estrutura não foi fechada corretamente ou existe um WHILE TRUE sem break";
            return false;
        }
        if (!hasAction)
        {
            compileResult = "ERRO DE COMPILAÇÃO: Não há nenhuma ação no código";
            return false;
        }
        totalCells = PC + 1;
        // foreach (Cell cell in memory)
        // {
        //     Debug.Log("bbbb" + cell);
        // }
        ResetAttributes();
        compileResult = "COMPILAÇÃO BEM SUCEDIDA!!!";
        return true;
    }

    public Commands Run(BattleStatus status, BattleManager battleManager)
    {
        this.battleManager = battleManager;
        for (int iter = 0; iter < maxIterations; iter++)
        {
            PC++;
            if (PC >= totalCells) throw new PlayerOutOfActionsException();
            Cell cell = memory[PC];
            if (debug) Debug.Log($"Entering cell {cell} at index {PC}");
            // Debug.Log($"Entering cell {cell} at index {PC}");

            switch (cell)
            {
                case ActionCell c:
                    return c.action;
                case AfterEndCell c:
                    battleManager.currentlyWhileLoop = false;
                    battleManager.checkWin();
                    if (battleManager.IsOver != 0) return Commands.NONE;
                    break;
                case BreakCell c:
                    battleManager.currentlyWhileLoop = false;
                    Jump(c);
                    break;
                case WhileCell c:
                    runStructureStack.Push("WhileCell");
                    battleManager.currentlyWhileLoop = true;
                    // if (c.comparatorCell.ToString() == "TrueCell")
                    // {
                    // }
                    JumpCond(c, status);
                    break;
                case IConditionCell c:
                    runStructureStack.Push("NotWhileCell");
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

    public Commands Run(BattleStatus status)
    {
        // this.battleManager = battleManager;
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
                case AfterEndCell c:
                    break;
                case BreakCell c:
                    Jump(c);
                    break;
                case WhileCell c:
                    JumpCond(c, status);
                    break;
                case IConditionCell c:
                    // runStructureStack.Push("NotWhileCell");
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
        return Commands.NONE;
        throw new ActionTookTooLongException();
    }

    private void JumpCond(IConditionCell cell, BattleStatus status)
    {
        string lastStructure = "";
        try
        {
            lastStructure = runStructureStack.Pop();
        }
        catch
        {
            lastStructure = "NotWhileCell";
        }
        if (!cell.Evaluate(status))
        {
            if (lastStructure == "WhileCell")
            {
                this.battleManager.currentlyWhileLoop = false;
            }
            Jump((Cell)cell);
        }
    }

    private void Jump(Cell cell)
    {
        if (debug) Debug.Log($"Jumping {cell.jmp} cells");
        PC += cell.jmp;
    }

    public List<List<Commands>> Decompile(Cell[] cells)
    //TODO Fazer com que o bloco de CODE volte como um bloco de CODE
    {
        List<List<Commands>> decompilado = new List<List<Commands>>();
        bool breakForEach = false;
        foreach (Cell cell in cells)
        {
            List<Commands> cmds = new List<Commands>();
            switch (cell)
            {
                case WhileCell c:
                    cmds.Add(Commands.WHILE);
                    cmds = GetInsides(c, cmds);
                    break;
                case IfCell c:
                    cmds.Add(Commands.IF);
                    cmds = GetInsides(c, cmds);
                    break;
                case ForCell c:
                    // Debug.Log(c.ToString());
                    cmds.Add(Commands.FOR);
                    // Debug.Log(c.GetVariable());
                    cmds.Add((Commands)Enum.Parse(typeof(Commands), c.GetVariable().ToString().Replace("Cell", "").ToUpper()));
                    break;
                case IConditionCell c:
                    Debug.Log("iCOND");
                    Debug.Log(c.ToString());
                    Debug.Log("Não sei o que é isso nem o que fazer com isso");
                    break;
                default:
                    try
                    {
                        // Debug.Log(cell.ToString());
                        cmds.Add((Commands)Enum.Parse(typeof(Commands), cell.ToString().Replace("Cell", "").ToUpper()));
                    }
                    catch
                    {
                        if (cell == null)
                        {
                            // Debug.Log("Nulooooo");
                            breakForEach = true;
                        }
                    }
                    break;
            }
            if (breakForEach)
            {
                break;
            }
            decompilado.Add(cmds);
        }
        return decompilado;
    }

    private List<Commands> GetInsides(WhileCell c, List<Commands> cmds)
    {
        switch (c.comparatorCell)
        {
            case TrueCell c2:
                // Debug.Log(c2.ToString());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
                break;
            case EvenCell c2:
                // Debug.Log(c2.ToString());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
                // Debug.Log(c2.GetVariable());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.GetVariable().ToString().Replace("Cell", "").ToUpper()));
                break;
            default:
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.ToString().Replace("Cell", "").ToUpper()));
                // Debug.Log(c.comparatorCell.GetVariables()[0]);
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[0].ToString().Replace("Cell", "").ToUpper()));
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[1].ToString().Replace("Cell", "").ToUpper()));
                break;
        }
        return cmds;
    }

    private List<Commands> GetInsides(IfCell c, List<Commands> cmds)
    {
        switch (c.comparatorCell)
        {
            case TrueCell c2:
                // Debug.Log(c2.ToString());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
                break;
            case EvenCell c2:
                // Debug.Log(c2.ToString());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
                // Debug.Log(c2.GetVariable());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.GetVariable().ToString().Replace("Cell", "").ToUpper()));
                break;
            default:
                // Debug.Log(c.comparatorCell.ToString());
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.ToString().Replace("Cell", "").ToUpper()));
                // Debug.Log(c.comparatorCell.GetVariables()[0]);
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[0].ToString().Replace("Cell", "").ToUpper()));
                cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[1].ToString().Replace("Cell", "").ToUpper()));
                break;
        }
        return cmds;
    }

}