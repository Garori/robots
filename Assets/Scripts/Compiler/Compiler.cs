using System.ComponentModel.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Unity.Mathematics;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using System.Data.SqlTypes;

public class Compiler : MonoBehaviour
{
    [SerializeField] private int maxIterations = 10;
    [SerializeField] private int maxBlocks = 100;

    private int PC;

    private Stack<string> runStructureStack = new Stack<string>();


    private Cell[] memory;
    public Cell[] Memory { get => memory; }
    private bool currentlyWhileLoop;
    public bool CurrentlyWhileTrue { get => currentlyWhileLoop;}

    public string aaaa ="";
    private BattleManager battleManager;

    public BattleManager BattleManager { get => battleManager; }
    private int totalCells;
    public int TotalCells { get => totalCells; }
    [SerializeField] private bool debug;

    private Stack<int> intsStackConditionals = new Stack<int>();

    // public bool hasWhileTrue = false;

    private void Start()
    {
        currentlyWhileLoop = false;
        memory = new Cell[maxBlocks * 2];
        ResetAttributes();
    }

    public void Compile(Cell[] memory)
    {
        currentlyWhileLoop  = false;
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

    public struct Temporario {
        public string token;
        public List<Temporario> lista;
    }



    public List<string> GetTokensList(string cond)
    {
        // Debug.Log("inicial = " + cond);
        cond = cond.Trim();
        List<string> lista = new List<string>();
        Dictionary<string,int> dict = new Dictionary<string,int>();
        // dict.Add
        dict.Add("(", (cond.IndexOf("(") != -1) ? cond.IndexOf("(") : int.MaxValue);
        dict.Add(")", (cond.IndexOf(")") != -1) ? cond.IndexOf(")") : int.MaxValue);
        dict.Add("&&", (cond.IndexOf("&&") != -1) ? cond.IndexOf("&&") : int.MaxValue);
        dict.Add("||", (cond.IndexOf("||") != -1) ? cond.IndexOf("||") : int.MaxValue);
        dict.Add("even", (cond.IndexOf("even") != -1) ? cond.IndexOf("even") : int.MaxValue);
        dict.Add("!", (cond.IndexOf("!") != -1) ? cond.IndexOf("!") : int.MaxValue);
        // bool stillHasTokens = true;
        // int k = 0;
        // foreach(int i in dict.Values)
        // {
        //     if (i == int.MaxValue)
        //     {
        //         k++;
        //     }
        // }
        // // if (indexAbre == int.MaxValue && indexFecha == int.MaxValue && indexAnd == int.MaxValue && indexOr == int.MaxValue && indexEven == int.MaxValue)
        // if(k==6)
        // {
        //     lista.Add(cond);
        //     // aaaa += cond;
        //     return lista;
        // }
        int minValue = int.MaxValue;
        string minKey = "";
        foreach (string key in dict.Keys)
        {
            // Debug.Log("value = " + i);
            if (dict[key] < minValue)
            {
                if (key == "!" && cond[dict[key] + 1] == '=') { continue; }
                minValue = dict[key];
                minKey = key;
            }
        }
        // while (indexAbre < int.MaxValue || indexFecha < int.MaxValue || indexAnd < int.MaxValue || indexOr < int.MaxValue || indexEven < int.MaxValue)
        while (minKey != "")
        {
            lista.Add(cond.Substring(0, minValue).Trim()); 
            // aaaa += lista.Last() + ",";
            lista.Add(cond.Substring(minValue, minKey.Length).Trim());
            // aaaa += lista.Last() + ",";
            cond = cond.Substring(minValue + minKey.Length);
            minValue = int.MaxValue;
            minKey = "";
            Dictionary<string, int> aux = new Dictionary<string, int>(dict);
            foreach (string key in dict.Keys)
            {
                aux[key] = (cond.IndexOf(key) != -1) ? cond.IndexOf(key) : int.MaxValue;
                // Debug.Log("value = " + i);
                if (aux[key] < minValue)
                {
                    if (key == "!" && cond[aux[key] + 1] == '=') { continue; }
                    minValue = aux[key];
                    minKey = key;
                }
            }
            dict = aux;
        }
        lista.Add(cond);
        // aaaa += cond;
        // Debug.Log("tamanho lista final = " + lista.Count);
        return lista;
    }

    public List<string> GetEveryCommand(List<string> lista)
    {
        List<string> listaAUX = new List<string>();
        // Debug.Log("aaaaaaaaaaaaaaaa " + listaAUX.Count);
        foreach (string cond in lista)
        {
            Debug.Log("cond = " + cond);
            char[] aux = {'>','<','=','!'};
            int index = cond.IndexOfAny(aux);
            // Debug.Log("INDEX OF ANY  = " + index);
            if (index != -1)
            {
                Debug.Log(index);
                Debug.Log(cond.Length);
                if(cond == "!")
                {
                    listaAUX.Add(cond);
                    continue;
                }
                string comparator = cond.Substring(index,2);
                if(comparator[1] != '='){
                    // Debug.Log(comparator[0]);
                    listaAUX.Add(comparator[0].ToString());
                    listaAUX.Add(cond.Substring(0,index));
                    Debug.Log(cond.Substring(0, index));
                    listaAUX.Add(cond.Substring(index+1));
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
        foreach (string cond in listaAUX){
            aaaa += cond+", ";
        }
        listaAUX = listaAUX.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        Debug.Log("____________________________________________________________");
        Debug.Log($"listaAUX = {listaAUX}");
        foreach (string cond in listaAUX){
            Debug.Log(cond);
        }
        Debug.Log("____________________________________________________________");
        return listaAUX;

    }
    public List<List<Commands>> GetCodeCommands(CodeController codeBlock)
    {
        List<List<Commands>> commands = new List<List<Commands>>();
        // bool flagFaltaAbrirChaves = false;
        Stack<string> structures = new Stack<string>();
        string lastStructure = "";
        string notOpenedStucture = "";
        bool lastStructureWasAnIfAndItReceivedAnEndBlock = false;
        foreach (string rawLine in codeBlock.CodeWithin.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            string line = rawLine.Trim();
            List<Commands> lineCommandsCode = new List<Commands>();
            switch (line)
            {
                case "attack();":
                case "defend();":
                case "charge();":
                case "dodge();":
                case "heal();":
                // case "END":
                case "break;":
                // case "ELSE":
                case "continue;":
                    lastStructureWasAnIfAndItReceivedAnEndBlock = false;
                    Debug.Log("line = " + line);
                    // if(flagFaltaAbrirChaves) {throw new Exception("ERRO DE COMPILAÇÃO: Alguma estrutura de repetição em um bloco CODE não foi aberta corretamente, '{' era esperado"); }
                    line = line.Replace("();","").ToUpper();
                    lineCommandsCode.Add((Commands)Enum.Parse(typeof(Commands), line));
                    commands.Add(lineCommandsCode);
                    continue;
                    // break;
                case "}":
                    // if (!flagChaves) {throw new Exception("ERRO DE COMPILAÇ");}
                    // Debug.Log($"last estrutura = {Structures[Structures.Count-1]}");
                    try
                    {
                        lastStructure = structures.Pop();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("ERRO DE COMPILAÇÃO: Alguma estrutura de repetição em um bloco CODE não foi aberta corretamente, '{' era esperado");
                    }

                    if (lastStructure == "if")
                    {
                        // Debug.Log("é verdade e eu atesto");
                        lastStructureWasAnIfAndItReceivedAnEndBlock = true;
                    }
                    // if (flagFaltaAbrirChaves) { throw new Exception("ERRO DE COMPILAÇÃO: Alguma estrutura de repetição em um bloco CODE não foi aberta corretamente, '{' era esperado"); }
                    lineCommandsCode.Add(Commands.END);
                    commands.Add(lineCommandsCode);
                    Debug.Log("line = " + line);
                    continue;
                    // break;
                case "{":
                    // string a = "}      \n     ";
                    // Debug.Log($"'{a.Trim()}'");
                    // flagFaltaAbrirChaves = false;
                    // lastStructure = structures.Pop();
                    // if (notOpenedStucture != "")
                    // {
                    structures.Push(notOpenedStucture);
                    notOpenedStucture = "";
                    // }
                    // commands.Add(lineCommandsCode);
                    continue;
                // break;
                // default:
                //     //todo ver se consigo ler as linhas não padrões e fazer operações matemáticas
                //     break;
                case "attack()":
                case "defend()":
                case "charge()":
                case "dodge()":
                case "heal()":
                // case "END":
                case "break":
                // case "ELSE":
                case "continue":
                    throw new Exception("ERRO DE COMPILAÇÃO: ';' era esperado");

            }
            int conditionStart = line.IndexOf('(');
            string[] userVars = { };
            string[] userFuncs = { };
            if (userVars.Contains(line))
            {
                // todo

            }
            else if (conditionStart != -1 || line.IndexOf("else") == 0)
            {
                string functionOrStructure = "else";
                if (conditionStart != -1)
                {
                    functionOrStructure = line.Substring(0, conditionStart).Trim();
                }
                if(userFuncs.Contains(functionOrStructure))
                {
                    // todo
                    // commands.Add(lineCommandsCode);
                    continue;
                }
                print(functionOrStructure);
                switch (functionOrStructure)
                {
                    case "for":
                    case "if":
                    case "while": // FOR, IF , WHILE
                    case "else":
                        notOpenedStucture = functionOrStructure;

                        // if (flagFaltaAbrirChaves) { throw new Exception("ERRO DE COMPILAÇÃO: Alguma estrutura de repetição em um bloco CODE não foi aberta corretamente, '{' era esperado"); }
                        Debug.Log("line = " + line);
                        Debug.Log("AAAAAAAAAAA = " + line.Substring(line.Length-1).Trim());                        
                        if (functionOrStructure == "else")
                        {
                            if (lastStructureWasAnIfAndItReceivedAnEndBlock)
                            {
                                commands.RemoveAt(commands.Count - 1);
                            }
                            Debug.Log("antes de empurrar else = " + line.Substring(line.Length-1).Trim());
                            if (line.Substring(line.Length-1).Trim() != "e" || line.Substring(line.Length-1).Trim() == "{")
                            {
                                if (line.Substring(line.Length-1).Trim() == "{")
                                {
                                    Debug.Log("entrei no lugar de empurrar 1");
                                    structures.Push(functionOrStructure);
                                    notOpenedStucture = "";
                                }
                            }

                            Commands elseCommand = (Commands)Enum.Parse(typeof(Commands), "ELSE");
                            // lastStructure = "ELSE";
                            lineCommandsCode.Add(elseCommand);
                            lastStructureWasAnIfAndItReceivedAnEndBlock = false;
                            break;
                        }

                        lastStructureWasAnIfAndItReceivedAnEndBlock = false;

                        conditionStart = line.IndexOf('(');
                        int conditionEnd = line.LastIndexOf(')');
                        // if (line.Substring(0,5).IndexOf("WHILE")==-1 && line.Substring(0, 5).IndexOf("IF") == -1 && line.Substring(0, 5).IndexOf("FOR") == -1)
                        // {
                        //     throw new Exception("ERRO DE COMPILAÇÃO: Variável ou chamada não reconhecida em um bloco CODE");
                        // }
                        // if (conditionStart == -1 || conditionEnd == -1)
                        // {

                        // }
                        // Debug.Log(conditionStart);
                        // Debug.Log(conditionEnd);
                        // Debug.Log(line.Substring(conditionStart, conditionEnd - conditionStart+1));
                        // Debug.Log(line.Substring(conditionStart + 1, conditionEnd - conditionStart - 1));
                        Debug.Log("antes de empurrar resto = " + line.Substring(conditionEnd).Trim());
                        if (line.Substring(conditionEnd).Trim() == ")" || (conditionEnd < line.Length && line.Substring(conditionEnd,2).Trim() == "){"))
                        {
                            if (line.Substring(conditionEnd).Trim() == "){")
                            {
                                Debug.Log("entrei no lugar de empurrar 2");
                                structures.Push(functionOrStructure);
                                notOpenedStucture = "";
                            }
                            // Debug.Log("safe")
                        }
                        else
                        {
                            Debug.Log("nao safe");
                            throw new Exception("ERRO DE COMPILAÇÃO: A declaração de alguma estrutura de repetição dentro de um bloco CODE está incorreta");
                        }

                        List<string> ListaCondicionais = new List<string>();
                        Temporario rawCondicao = new Temporario();
                        rawCondicao.token = line.Substring(conditionStart, conditionEnd - conditionStart+1);
                        // rawCondicao.token = line.Substring(conditionStart + 1, conditionEnd - conditionStart - 1);
                        // Debug.Log("RAW CONDITION = " + rawCondicao.token);
                        ListaCondicionais = GetTokensList(rawCondicao.token);
                        ListaCondicionais = ListaCondicionais.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        ListaCondicionais = GetEveryCommand(ListaCondicionais);
                        
                        Commands mainCommand = (Commands)Enum.Parse(typeof(Commands), line.Substring(0, conditionStart).Trim().ToUpper());

                        lastStructure = line.Substring(0, conditionStart).Trim();
                        // Debug.Log("comando pelo método de INDEX OF: "+mainCommand);
                        // List<string> splittedLine = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                        // Debug.Log("sou uma estrutura "+splittedLine[0]);
                        print(mainCommand);
                        lineCommandsCode.Add(mainCommand);
                        // foreach (string token in splittedLine.GetRange(1, splittedLine.Count-1))
                        bool hasAtLeastOneBool = false;
                        foreach (string _token in ListaCondicionais)
                        {
                            string token = _token.Trim();
                            string tokenTraduzido= "";
                            Debug.Log($"token atual = '{token}'");
                            switch (token)
                            {
                                case ">":
                                    tokenTraduzido = "GREATER";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "!":
                                    tokenTraduzido = "NOT";
                                    break;
                                case "==":
                                    tokenTraduzido = "EQUALS";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "!=":
                                    tokenTraduzido = "NOT_EQUALS";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "true":
                                    tokenTraduzido = "TRUE";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "false":
                                    //faz adicionar um NOT true
                                    lineCommandsCode.Add((Commands)Enum.Parse(typeof(Commands), "NOT"));
                                    tokenTraduzido = "TRUE";
                                    hasAtLeastOneBool = true;
                                    break;
                                case ">=":
                                    tokenTraduzido = "GREATER_EQUALS";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "<":
                                    tokenTraduzido = "LESSER";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "<=":
                                    tokenTraduzido = "LESSER_EQUALS";
                                    hasAtLeastOneBool = true;
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
                                case "even":
                                    tokenTraduzido = "EVEN";
                                    hasAtLeastOneBool = true;
                                    break;
                                case "ROUND":
                                    tokenTraduzido = "ROUND";
                                    break;
                                default:
                                    // string[] numbers = {"ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "ZERO" };
                                    // if(numbers.Contains(token.Trim())) {tokenTraduzido = token; break;}
                                    int number;
                                    if(int.TryParse(token.Trim(), out number))
                                    {
                                        tokenTraduzido = "NUMBER";
                                        intsStackConditionals.Push(number);
                                        Debug.Log($"pushed {number}");
                                        break;
                                    }
                                    try 
                                    {
                                        string[] token_parts = token.Split('_');
                                        if(token_parts.Length == 1){
                                            throw new Exception("");
                                        }
                                        string[] para_testar = {"DANO", "DEF", "CARGA", "MAX", "ATUAL"};
                                        if (para_testar.Contains(token_parts[token_parts.Count() - 1]))
                                        {
                                            token += "_";
                                            token_parts = token.Split('_');
                                        }
                                        print($"tokeeeeeeeeeeen {token}");
                                        foreach(string t in token_parts)
                                        {
                                            print(t);
                                        }
                                        if (token_parts[0]=="J") tokenTraduzido += "PLAYER_";
                                        else if (token_parts[0]=="I") tokenTraduzido += "ENEMY_";
                                        else { tokenTraduzido = "ERROR"; break; }
                                        if (token_parts[1] == "V") 
                                        {
                                            if(token_parts[2] == "MAX") tokenTraduzido += "MAX_HEALTH";
                                            else if (token_parts[2] == "MAX") tokenTraduzido += "ACTUAL_HEALTH";
                                            else { tokenTraduzido = "ERROR"; break; }
                                        }
                                        else if (token_parts[1] == "DANO") tokenTraduzido += "DAMAGE";
                                        else if (token_parts[1] == "DEF") tokenTraduzido += "ACTUAL_SHIELD";
                                        else if (token_parts[1] == "CARGA") tokenTraduzido += "ACTUAL_CHARGE";
                                        else {tokenTraduzido = "ERROR"; break;}
                                        if (token_parts[token_parts.Count()-1] == "MET") tokenTraduzido += "_HALF";
                                        else if (token_parts[token_parts.Count() - 1] == "DOBRO") tokenTraduzido += "_DOUBLE";
                                        else if (token_parts[token_parts.Count() - 1] == "") tokenTraduzido += "";
                                        else { tokenTraduzido = "ERROR"; break; }
                                    }
                                    catch {
                                        throw new Exception($"ERRO DE COMPILAÇÃO: Em uma condicional o token \"{token}\" não foi reconhecido");
                                        tokenTraduzido = "ERROR"; 
                                        break; 
                                    }
                                    // Debug.Log("aqui = " + token);
                                    break;

                            }
                            // Debug.Log(tokenTraduzido);
                            lineCommandsCode.Add((Commands)Enum.Parse(typeof(Commands), tokenTraduzido));
                        }
                        if (!hasAtLeastOneBool)
                        {
                            throw new Exception("ERRO DE COMPILAÇÃO: Condicional sem comparador ou sem TRUE");
                        }
                        break;
                    default:
                        throw new Exception("ERRO DE COMPILAÇÃO: Não foi possível compilar alguma linha de um bloco CODE");
                }
            }
            else 
            {
                throw new Exception("ERRO DE COMPILAÇÃO: Não foi possível compilar alguma linha de um bloco CODE");
            }
            commands.Add(lineCommandsCode);
        }

        // foreach (List<Commands> commands1 in commands)
        // {
        //     Debug.Log(commands1[0].ToString());
        // }
        if (structures.Count != 0)
        {
            throw new Exception("ERRO DE COMPILAÇÃO: Alguma estrutura existente em um bloco CODE não foi aberta ou fechada corretamente");
        }
        return commands;
    }

    //em desuso
    // public bool Compile(List<BlockController> blocks, ref string compileResult)
    // {
    //     List<List<Commands>> commands = GetCommands(blocks);
    //     return Compile(commands, ref compileResult);
    // }

    public bool Compile(List<BlockController> blocks, ref string compileResult)
    {
        // lastBreakIndex = -1;
        if (blocks.Count > maxBlocks)
        {
            compileResult = $"ERRO DE COMPILAÇÃO: Não pode haver mais de {maxBlocks} blocos";
            return false;
        }
        Stack<int> structuresStack = new Stack<int>();
        Stack<int> breakStackIndexes = new Stack<int>();
        List<int> whilesAndForsList = new List<int> {};
        bool hasAction = false;


        ResetAttributes();
        PC = -1;
        foreach (BlockController block in blocks)
        {
            try
            {
                if (memory[PC].GetType() == typeof(AfterEndCell))
                {
                    PC--;
                }
            }
            catch {}

            PC++;
            Commands mainCommand = block.commandName;
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
                //todo
                // Debug.Log("vai entrar no bloco de código");
                // Debug.Log("PC ANTES DO CODE = " + PC);
                memory[PC] = new CodeCell((block as CodeController).CodeWithin);
                List<List<Commands>> codeCommands = null;
                try
                {
                    codeCommands = GetCodeCommands(block as CodeController);
                }
                catch (Exception e)
                {
                    compileResult = e.Message;
                    return false;
                }
                Stack<int> aux = new Stack<int>();
                // Debug.Log($"embaixo daqui  {intsStackConditionals.Count} {intsStackConditionals.Peek()}");
                foreach (int _ in intsStackConditionals)
                {
                    Debug.Log(_);
                }
                Debug.Log("__________________________");
                // Debug.Log($"embaixo daqui2  {intsStackConditionals.Count} {intsStackConditionals.Peek()}");
                //Inverte a pilha de conndicionais
                int tamIntsStackConditionalsAUX = intsStackConditionals.Count;
                for (int k = 0; k <tamIntsStackConditionalsAUX ; k++)
                {
                    Debug.Log("k" + k);
                    int auxaa = intsStackConditionals.Pop();
                    aux.Push(auxaa);
                }
                intsStackConditionals = aux;
                // Debug.Log("stack e tal né " + intsStackConditionals.Count);
                bool ret = CompileCode(codeCommands, ref compileResult, ref PC, ref hasAction);
                // Debug.Log("PC DEPOIS DO CODE = " + PC);
                if(!ret) { return false; }
                // Debug.Log("saiu do bloco de código");
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
                    whilesAndForsList.RemoveAt(whilesAndForsList.Count -1);
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
                    Debug.Log("1");
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco ELSE sem IF correspondente";
                    return false;
                }

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell))
                {
                    Debug.Log("2");
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
                Commands variableName = ((block as ForController).variableSlot.childBlock as VariableController).commandName;
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
            ComparatorController comparator = (block as StructureController).comparatorSlot.childBlock as ComparatorController;
            Commands comparatorCommand = comparator.commandName;
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
                case Commands.TRUE:
                    conditionalCell = new ConditionalCell(typeof(bool), resultado: true,comparator: new TrueCell());
                    break;
                case Commands.NOT:
                    conditionalCell = new ConditionalCell(typeof(NotCell)); //, comparator: new NotCell());
                    break;
                case Commands.EVEN:
                    Commands variableName = comparator.variableSlot1.childBlock.commandName;
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
                    // i+=3;
                    comparatorCell = new EvenCell(variableName);
                    conditionalCell = new ConditionalCell(typeof(EvenCell), comparator: comparatorCell);
                    break;
                default:
                    Commands variable1Name = comparator.variableSlot1.childBlock.commandName;
                    Commands variable2Name = comparator.variableSlot2.childBlock.commandName;
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
            // Debug.Log(conditionalCell.tipo);
            conditionalList.Add(conditionalCell);
            // }
            // if(parentesisStack.Count > 0)
            // {
            //     compileResult = "ERRO DE COMPILAÇÃO: Parentesis fechado sem ter sido aberto";
            //     return false;
            // }

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
            compileResult = "ERRO DE COMPILAÇÃO: Uma estrutura não foi fechada corretamente";
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
        foreach (Cell cell in memory)
        {
            if(cell != null)
            {
                Debug.Log(cell.ToString());
            }
        }
        return true;
    }


    public bool CompileCode(List<List<Commands>> blockCommands, ref string compileResult, ref int PC, ref bool hasAction)
    {
        int auxPC = PC;
        Stack<int> structuresStack = new Stack<int>();
        Stack<int> breakStackIndexes = new Stack<int>();
        List<int> whilesAndForsList = new List<int> { };
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        foreach (List<Commands> a in blockCommands){
            Debug.Log("------");
            foreach(Commands caaa in a){
                Debug.Log("" + caaa);
            }
            Debug.Log("------");
        }
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        foreach (List<Commands> lineCommands in blockCommands)
        {
            // Debug.Log(lineCommands);
            try
            {
                if (memory[PC].GetType() == typeof(AfterEndCell))
                {
                    PC--;
                }
            }
            catch { }

            PC++;
            Debug.Log("pc dentro do code = " + PC);
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
                    compileResult = "ERRO DE COMPILAÇÃO: '}' sem estrutura correspondente";
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
            Stack<string> parentesisStack = new Stack<string>();
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
                Debug.Log(comparatorCommand);
                ConditionalCell conditionalCell = null;
                ComparatorCell comparatorCell = null;
                List<string> andOrorganizer = new List<string>();
                switch (comparatorCommand)
                {
                    case Commands.NOT:
                        if (lineCommands.Count >= i+2 &&  i >=2)
                        {
                            Debug.Log(lineCommands[i + 1]);
                            if (lineCommands[i+1] == Commands.AND || lineCommands[i + 1] == Commands.OR || lineCommands[i + 1] == Commands.CLOSE_PARENTHESIS)
                            {
                                compileResult = "ERRO DE COMPILAÇÃO: Not ('!') negando um token inválido";
                                return false;
                            }
                        }
                        conditionalCell = new ConditionalCell(typeof(NotCell));
                        break;
                    case Commands.AND:
                        lastANDOR = parentesisStack.Pop();
                        if (lastANDOR == "OR")
                        {
                            parentesisStack.Push("OR");
                            compileResult = "ERRO DE COMPILAÇÃO: AND e OR não separados por parentesis para dar ordem de operações clara";
                            return false;
                        }
                        parentesisStack.Push("AND");
                        conditionalCell = new ConditionalCell(typeof(AndCell));
                        break;
                    case Commands.OR:
                        lastANDOR = parentesisStack.Pop();
                        if (lastANDOR == "AND")
                        {
                            parentesisStack.Push("AND");
                            compileResult = "ERRO DE COMPILAÇÃO: AND e OR não separados por parentesis para dar ordem de operações clara";
                            return false;
                        }
                        parentesisStack.Push("OR");
                        conditionalCell = new ConditionalCell(typeof(OrCell));
                        break;
                    case Commands.OPEN_PARENTHESIS:
                        conditionalCell = new ConditionalCell(typeof(OpenParenthesisCell));
                        parentesisStack.Push("");
                        lastANDOR = "";
                        break;
                    case Commands.CLOSE_PARENTHESIS:
                        conditionalCell = new ConditionalCell(typeof(CloseParenthesisCell));
                        try
                        {
                            lastANDOR = "";
                            parentesisStack.Pop();
                        }
                        catch
                        {
                            compileResult = "ERRO DE COMPILAÇÃO: Parentesis fechado sem ter sido aberto!";
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
                        int aux = variableName == Commands.NUMBER ? intsStackConditionals.Pop() : -1;
                        comparatorCell = new EvenCell(variableName, variableINT: aux);
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
                            print(variable1Name);
                            print(variable2Name);
                            compileResult = $"ERRO DE COMPILAÇÃO: {comparatorCommand} com variável não conhecida";
                            return false;
                        }

                        int aux1 = variable1Name == Commands.NUMBER ? intsStackConditionals.Pop() : -1;
                        int aux2 = variable2Name == Commands.NUMBER ? intsStackConditionals.Pop() : -1;
                        Debug.Log($"vars for {comparatorCommand.ToString()} are {aux1} and {aux2}");
                        switch (comparatorCommand)
                        {
                            case Commands.EQUALS:
                                comparatorCell = new EqualsCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(EqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.NOT_EQUALS:
                                comparatorCell = new NotEqualsCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(NotEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER:
                                comparatorCell = new GreaterCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(GreaterCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER_EQUALS:
                                comparatorCell = new GreaterEqualsCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(GreaterEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER:
                                comparatorCell = new LesserCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(LesserCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER_EQUALS:
                                comparatorCell = new LesserEqualsCell(variable1Name, variable2Name, variable1INT: aux1, variable2INT: aux2);
                                conditionalCell = new ConditionalCell(typeof(LesserEqualsCell), comparator: comparatorCell);
                                break;
                            default:
                                compileResult = "ERRO DE COMPILAÇÃO: Alguma estrutura condicional contém um erro ";
                                return false;
                        }
                        break;
                }
                // Debug.Log(conditionalCell.tipo);
                conditionalList.Add(conditionalCell);
            }
            if (parentesisStack.Count > 0)
            {
                compileResult = "ERRO DE COMPILAÇÃO: Alguma estrutura de repetição de um bloco CODE tem uma condição com parenteses abertos e nunca fechados";
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
        Debug.Log($"PC LOGO DEPOIS DO COMPILE CODE = {PC}");
        Debug.Log($"AUXPC LOGO DEPOIS DO COMPILE CODE = {auxPC}");
        for (int i = auxPC+1; i <=PC; i++)
        {
            memory[i].isFromACodeBlock = true;
        }
        if (structuresStack.Count != 0)
        {
            compileResult = "ERRO DE COMPILAÇÃO: Alguma estrutura não foi fechada corretamente";
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
        // ResetAttributes();
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
                case CodeCell c:
                    iter --;
                    continue;
                case ActionCell c:
                    return c.action;
                case AfterEndCell c:
                    iter--;
                    battleManager.currentlyWhileLoop = false;
                    battleManager.checkWin();
                    if(battleManager.IsOver != 0) return Commands.NONE;
                    break;
                case BreakCell c:
                    battleManager.currentlyWhileLoop = false;
                    Jump(c);
                    break;
                case WhileCell c:
                    runStructureStack.Push("WhileCell");
                    battleManager.currentlyWhileLoop = true;
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
                case CodeCell c:
                    iter--;
                    continue;
                case ActionCell c:
                    return c.action;
                case AfterEndCell c:
                    iter--;
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
        catch{
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

    // public List<BlockController> Decompile(Cell[] cells)
    // //TODO Fazer com que o bloco de CODE volte como um bloco de CODE
    // {
    //     List<BlockController> decompilado = new List<BlockController>();
    //     bool breakForEach = false;
    //     foreach (Cell cell in cells)
    //     {
    //         if(cell.isFromACodeBlock)
    //         {
    //             continue;
    //         }
    //         List<Commands> cmds = new List<Commands>();
    //         switch (cell)
    //         {
    //             case WhileCell c:
    //                 cmds.Add(Commands.WHILE);
    //                 cmds = GetInsides(c,cmds);
    //                 break;
    //             case IfCell c:
    //                 cmds.Add(Commands.IF);
    //                 cmds = GetInsides(c, cmds);
    //                 break;
    //             case ForCell c:
    //                 // Debug.Log(c.ToString());
    //                 cmds.Add(Commands.FOR);
    //                 // Debug.Log(c.GetVariable());
    //                 cmds.Add((Commands)Enum.Parse(typeof(Commands), c.GetVariable().ToString().Replace("Cell", "").ToUpper()));
    //                 break;
    //             case IConditionCell c:
    //                 Debug.Log("iCOND");
    //                 Debug.Log(c.ToString());
    //                 Debug.Log("Não sei o que é isso nem o que fazer com isso");
    //                 break;
    //             default:
    //                 try
    //                 {
    //                     // Debug.Log(cell.ToString());
    //                     cmds.Add((Commands)Enum.Parse(typeof(Commands), cell.ToString().Replace("Cell", "").ToUpper()));
    //                 }
    //                 catch
    //                 {
    //                     if (cell == null)
    //                     {
    //                         // Debug.Log("Nulooooo");
    //                         breakForEach = true;
    //                     }
    //                 }
    //                 break;
    //         }
    //         if (breakForEach)
    //         {
    //             break;
    //         }
    //         decompilado.Add(cmds);
    //     }
    //     return decompilado;
    // }

    // private List<Commands> GetInsides(WhileCell c, List<Commands> cmds)
    // {
    //     switch (c.comparatorCell)
    //     {
    //         case TrueCell c2:
    //             // Debug.Log(c2.ToString());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //         case EvenCell c2:
    //             // Debug.Log(c2.ToString());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
    //             // Debug.Log(c2.GetVariable());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.GetVariable().ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //         default:
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.ToString().Replace("Cell", "").ToUpper()));
    //             // Debug.Log(c.comparatorCell.GetVariables()[0]);
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[0].ToString().Replace("Cell", "").ToUpper()));
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[1].ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //     }
    //     return cmds;
    // }

    // private List<Commands> GetInsides(IfCell c, List<Commands> cmds)
    // {
    //     switch (c.comparatorCell)
    //     {
    //         case TrueCell c2:
    //             // Debug.Log(c2.ToString());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //         case EvenCell c2:
    //             // Debug.Log(c2.ToString());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.ToString().Replace("Cell", "").ToUpper()));
    //             // Debug.Log(c2.GetVariable());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c2.GetVariable().ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //         default:
    //             // Debug.Log(c.comparatorCell.ToString());
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.ToString().Replace("Cell", "").ToUpper()));
    //             // Debug.Log(c.comparatorCell.GetVariables()[0]);
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[0].ToString().Replace("Cell", "").ToUpper()));
    //             cmds.Add((Commands)Enum.Parse(typeof(Commands), c.comparatorCell.GetVariables()[1].ToString().Replace("Cell", "").ToUpper()));
    //             break;
    //     }
    //     return cmds;
    // }

}


