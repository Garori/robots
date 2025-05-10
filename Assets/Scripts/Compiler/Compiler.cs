
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Globalization;
using System.Threading;
public struct Temporario
{
    public string token;
    public List<Temporario> lista;
};
public struct Token
{
    public Token(string _name, string _t, Commands _command, string _accepts, bool _reserved = true)
    {
        name = _name;
        t = _t;
        command = _command;
        reserved = _reserved;
        // expects = expects_dict2[_t];
        accepts = _accepts;

    }
    public string name;
    public string t;
    Commands command;
    // Stack<string> expects;
    string accepts;
    public bool reserved;
};

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

    private BattleManager battleManager;

    public BattleManager BattleManager { get => battleManager; }
    private int totalCells;
    public int TotalCells { get => totalCells; }
    [SerializeField] private bool debug;

    private Queue<dynamic> numbersQueueConditionals = new Queue<dynamic>();
    // private Queue<float> floatsQueueConditionals = new Queue<float>();


    private string current_token = "";
    // private string current_line = "";
    private Stack<string> stack_parenthesis = new Stack<string>();
    private Stack<string> stack_braces = new Stack<string>();
    
    private Dictionary<string, Dictionary<string, dynamic>> known_tokens = new Dictionary<string, Dictionary<string, dynamic>>()
    {
        {"\n",              new Dictionary<string,dynamic>(){{"t", "newLine"},     {"reserved", true},{"Commands",""}}},
        {"if",              new Dictionary<string,dynamic>(){{"t", "conditional_structure"},   {"reserved", true}, {"Commands",Commands.IF}}},
        {"while",           new Dictionary<string,dynamic>(){{"t", "conditional_structure"},   {"reserved", true},{"Commands",Commands.WHILE}}},
        {"for",             new Dictionary<string,dynamic>(){{"t", "for"},   {"reserved", true},  {"Commands",Commands.FOR}}},
        {"else",            new Dictionary<string,dynamic>(){{"t", "else"},        {"reserved", true},  {"Commands",Commands.ELSE}}},
        {"break",           new Dictionary<string,dynamic>(){{"t", "instruction"}, {"reserved", true},  {"Commands",Commands.BREAK}}},
        {"(",               new Dictionary<string,dynamic>(){{"t", "("},           {"reserved", true}, {"Commands",Commands.OPEN_PARENTHESIS}}},
        {")",               new Dictionary<string,dynamic>(){{"t", ")"},           {"reserved", true}, {"Commands",Commands.CLOSE_PARENTHESIS}}},
        {"{",               new Dictionary<string,dynamic>(){{"t", "{"},           {"reserved", true}, {"Commands",""}}},
        {"}",               new Dictionary<string,dynamic>(){{"t", "}"},           {"reserved", true}, {"Commands",Commands.END}}},
        {";",               new Dictionary<string,dynamic>(){{"t", ";"},           {"reserved", true},  {"Commands",""}}},
        {"attack",          new Dictionary<string,dynamic>(){{"t", "action"},      {"reserved", true}, {"Commands",Commands.ATTACK}}},
        {"defend",          new Dictionary<string,dynamic>(){{"t", "action"},      {"reserved", true}, {"Commands",Commands.DEFEND}}},
        {"heal",            new Dictionary<string,dynamic>(){{"t", "action"},      {"reserved", true}, {"Commands",Commands.HEAL}}},
        {"charge",          new Dictionary<string,dynamic>(){{"t", "action"},      {"reserved", true}, {"Commands",Commands.CHARGE}}},
        {"even",            new Dictionary<string,dynamic>(){{"t", "function_boolean"}, {"args", new List<string>{"int"}},{"reserved", true}, {"Commands",Commands.EVEN}}},
        {"J_V_MAX",         new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_MAX_HEALTH}}},
        {"J_V_MAX_DOBRO",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_MAX_HEALTH_DOUBLE}}},
        {"J_V_MAX_MET",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_MAX_HEALTH_HALF}}},
        {"J_V_ATUAL",       new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_HEALTH}}},
        {"J_V_ATUAL_DOBRO", new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_HEALTH_DOUBLE}}},
        {"J_V_ATUAL_MET",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_HEALTH_HALF}}},
        {"J_DANO",          new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_DAMAGE}}},
        {"J_DANO_DOBRO",    new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_DAMAGE_DOUBLE}}},
        {"J_DANO_MET",      new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_DAMAGE_HALF}}},
        {"J_DEF",           new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_SHIELD}}},
        {"J_DEF_DOBRO",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_SHIELD_DOUBLE}}},
        {"J_DEF_MET",       new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_SHIELD_HALF}}},
        {"J_CARGA",         new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_CHARGE}}},
        {"J_CARGA_DOBRO",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_CHARGE_DOUBLE}}},
        {"J_CARGA_MET",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.PLAYER_ACTUAL_CHARGE_HALF}}},
        {"I_V_MAX",         new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_MAX_HEALTH}}},
        {"I_V_MAX_DOBRO",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_MAX_HEALTH_DOUBLE}}},
        {"I_V_MAX_MET",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_MAX_HEALTH_HALF}}},
        {"I_V_ATUAL",       new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_HEALTH}}},
        {"I_V_ATUAL_DOBRO", new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_HEALTH_DOUBLE}}},
        {"I_V_ATUAL_MET",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_HEALTH_HALF}}},
        {"I_DANO",          new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_DAMAGE}}},
        {"I_DANO_DOBRO",    new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_DAMAGE_DOUBLE}}},
        {"I_DANO_MET",      new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_DAMAGE_HALF}}},
        {"I_DEF",           new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_SHIELD}}},
        {"I_DEF_DOBRO",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_SHIELD_DOUBLE}}},
        {"I_DEF_MET",       new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_SHIELD_HALF}}},
        {"I_CARGA",         new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_CHARGE}}},
        {"I_CARGA_DOBRO",   new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_CHARGE_DOUBLE}}},
        {"I_CARGA_MET",     new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ENEMY_ACTUAL_CHARGE_HALF}}},
        {"ROUND",           new Dictionary<string,dynamic>(){{"t", "int"},         {"reserved", true}, {"Commands", Commands.ROUND}}},
        {"!",               new Dictionary<string,dynamic>(){{"t", "logical"},     {"reserved", true}, {"Commands",Commands.NOT}}},
        {"&&",              new Dictionary<string,dynamic>(){{"t", "logical"},     {"reserved", true}, {"Commands",Commands.AND}}},
        {"||",              new Dictionary<string,dynamic>(){{"t", "logical"},     {"reserved", true}, {"Commands",Commands.OR}}},
        {">",               new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.GREATER}}},
        {">=",              new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.GREATER_EQUALS}}},
        {"<",               new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.LESSER}}},
        {"<=",              new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.LESSER_EQUALS}}},
        {"==",              new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.EQUALS}}},
        {"!=",              new Dictionary<string,dynamic>(){{"t", "comparation"}, {"reserved", true}, {"Commands",Commands.NOT_EQUALS}}},
        {"-",               new Dictionary<string,dynamic>(){{"t", "neg"},         {"reserved", true}, {"Commands",Commands.NEGATIVE}}},
        {"true",            new Dictionary<string,dynamic>(){{"t", "boolean"},     {"reserved", true}, {"Commands",Commands.TRUE}}},
        {"false",           new Dictionary<string,dynamic>(){{"t", "boolean"},     {"reserved", true}, {"Commands",Commands.TRUE } }},
    };
    Dictionary<string,dynamic> intToken = new Dictionary<string, dynamic>(){{"t", "int"}, {"reserved", true}, {"Commands", Commands.NUMBER}};
    Dictionary<string, dynamic> floatToken = new Dictionary<string, dynamic>() { { "t", "float" }, { "reserved", true }, { "Commands", Commands.NUMBER } };

    private int codeInputBlockNumber = 0;
    private int codeCompilerLine = 1;
    private void Start()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
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
    
    string tk;
    Dictionary<string, dynamic> token;
    List<string> firstBlocks = new List<string>{"conditional_structure", "for", "action", "identifier", "}", "instruction", "else"};
    List<string> firstExpressions = new List<string> { "int", "float", "function", "(", "!", "boolean" };

    Queue<string> tokens = new Queue<string>();
    List<Commands> lineCommands = new List<Commands>();
    List<List<Commands>> commands = new List<List<Commands>>();

    private void getTokenFromStr(string type = "", string name = ""){
        float auxVarFloat;
        int auxVarInt;
        if (tokens.Count == 0){
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} o código terminou abruptamente, parece estar incompleto");
        }
        tk = tokens.Dequeue();
        if (known_tokens.ContainsKey(tk)){
            token = known_tokens[tk];
        }
        else if (int.TryParse(tk, out auxVarInt))
        {
            token = intToken;
            numbersQueueConditionals.Enqueue(auxVarInt);
            // Debug.Log($"Pushed {auxVarInt}");

        }
        else if (float.TryParse(tk, out auxVarFloat)){
            token = floatToken;
            numbersQueueConditionals.Enqueue(auxVarFloat);
            // Debug.Log($"Pushed {auxVarFloat}");
        }
        else
        {
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} o token '{tk}' não foi reconhecido");
        }

        if(type != "" && type != token["t"]){
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} era esperado um token de tipo '{type}' mas, ao invés disso, recebeu um tipo '{token["t"]}'");
        }
        else if(name != "" && name != tk)
        {
            switch(tk){
                case "\n":
                    tk ="\\n";
                    break;
                case "\r":
                    tk ="\\r";
                    break;
                case "\t":
                    tk = "\\t";
                    break;
                default:
                    break;
            }
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} era esperado '{name}' mas, ao invés disso, recebeu um '{tk}'");
        }

        // erro = "nao reconhecido";
    }
    private void action(){
        Debug.Log("entrei em action");
        getTokenFromStr(name: "(");
        getTokenFromStr(name: ")");
        getTokenFromStr(name: ";");
        Debug.Log("saí de action");
    }
    private void comparation(Dictionary<string,dynamic> token_){
        Debug.Log("entrei em comparation");

        Commands comparator;
        getTokenFromStr();
        if(token["t"] == "comparation" || token["t"] == "operation")
        {
            comparator = token["Commands"];
        }
        else{throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} era esperado um token de comparação ou operação mas, ao invés disso, recebeu o token '{tk}' foi recebido");}
        getTokenFromStr();
        if (token["t"] == "int" || token["t"] == "float" || token["t"] == "function_int" || token["t"] == "function_float")
        {
            lineCommands.Add(comparator);
            lineCommands.Add(token_["Commands"]);
            lineCommands.Add(token["Commands"]);
        }
        Debug.Log("saí de comparation");

    }
    private void function(bool askForSemicolon = false){
        Debug.Log("entrei em function");
        List<string> args = new List<string>(token["args"]);
        getTokenFromStr(name: "(");
        foreach (string arg_t in args){
            getTokenFromStr(type: arg_t);
            lineCommands.Add(token["Commands"]);
            getTokenFromStr(name: ")");
            if(askForSemicolon) {getTokenFromStr(name: ";");}
        }
        Debug.Log("saí de function");

    }
    private void expression(bool pushParenthesis = true)
    {
        Debug.Log("entrei em expression");
        if (pushParenthesis) {stack_parenthesis.Push("");}
        getTokenFromStr();
        if (firstExpressions.Contains(token["t"])){
            if (token["t"] == "("){
                lineCommands.Add(token["Commands"]);
                expression();
                getTokenFromStr(name: ")");
                lineCommands.Add(token["Commands"]);
            }
            else if(token["t"] == "int" || token["t"] == "float" || token["t"] == "function_int" || token["t"] == "function_float")
            {
                comparation(token);
            }
            else if(token["t"] == "boolean"){
                if(tk == "false"){ lineCommands.Add(Commands.NOT); }
                lineCommands.Add(token["Commands"]);
            }
            else if(token["t"] == "function_boolean"){
                lineCommands.Add(token["Commands"]);
                function();
            }
            //fazer para function_number
            if(tokens.Count > 0 && tokens.Peek() == "&&"){
                getTokenFromStr();
                Debug.Log($"estou em && e dei peek em {stack_parenthesis.Peek()}");
                if (stack_parenthesis.Peek() == "||"){
                    throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} há '&&' e '||' misturados dentro de um mesmo parenteses\nPor favor indique com clareza a ordem de operações separando expressões com e's e expressões com ou's com parenteses");
                }
                stack_parenthesis.Pop();
                stack_parenthesis.Push("&&");
                lineCommands.Add(token["Commands"]);
                expression(false);
            }
            else if (tokens.Count > 0 && tokens.Peek() == "||")
            {
                Debug.Log($"estou em || e dei peek em {stack_parenthesis.Peek()}");
                getTokenFromStr();
                if (stack_parenthesis.Peek() == "&&"){
                    throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} há '&&' e '||' misturados dentro de um mesmo parenteses\nPor favor indique com clareza a ordem de operações separando expressões com e's e expressões com ou's com parenteses");
                }
                stack_parenthesis.Pop();
                stack_parenthesis.Push("||");
                lineCommands.Add(token["Commands"]);
                expression(false);
            }
            else if(firstExpressions.Contains(tokens.Peek()))
            {
                expression(false);
            }
        }
        else{ throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} uma expressão foi mal formada"); }
        if(pushParenthesis) {stack_parenthesis.Pop();}
        Debug.Log("saí de expression");

    }
    private void for_()
    {
        getTokenFromStr(name:"(");
        getTokenFromStr(type: "int");
        lineCommands.Add(token["Commands"]);
        getTokenFromStr(name: ")");
        getTokenFromStr(name: "{");
        stack_braces.Push("for");
    }

    private void conditionalStructure(){
        Debug.Log("entrei em conditional structure");
        string thisConditionalStructure = tk;
        getTokenFromStr(name: "(");
        lineCommands.Add(token["Commands"]);
        expression();
        getTokenFromStr(name: ")");
        lineCommands.Add(token["Commands"]);
        getTokenFromStr();
        while (token["t"] == "newLine"){
            codeCompilerLine++;
            getTokenFromStr();
        }
        if (tk != "{")
        {
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} era esperado o token '{{' mas, ao invés disso, recebeu o token '{tk}' foi recebido");
        }
        stack_braces.Push(thisConditionalStructure);
        Debug.Log("saí de conditional structure");

    }

    private void instruction()
    {
        Debug.Log("entrei em instruction");

        getTokenFromStr(name: ";");
        Debug.Log("saí de instruction");

    }
    private void else_(){
        Debug.Log("entrei em else");
        if (commands[commands.Count - 1][0] == Commands.END){
            commands.RemoveAt(commands.Count - 1);
        }
        getTokenFromStr(name:"{");
        stack_braces.Push("else");
        Debug.Log("saí de else");
    }
    private void end_(){
        Debug.Log("entrei em end");
        stack_braces.Pop();
        // lineCommands.Add(token["Commands"]);
        Debug.Log("saí de end");
    }
    private void callFuncByString(string f_name){
        switch(f_name)
        {
            case "conditional_structure":
                conditionalStructure();
                break;
            case "action":
                action();
                break;
            case "instruction":
                instruction();
                break;
            case "}":
                end_();
                break;
            case "for":
                for_();
                break;
            case "else":
                else_();
                break;
        }
    }
    
    private void parser(){

        codeCompilerLine = 1;
        Debug.Log("---------------------------------------------------\nENTREI NO PARSER");
        Debug.Log(tokens.Count);
        while (tokens.Count > 0)
        {
            lineCommands = new List<Commands>();
            tk = tokens.Dequeue();
            Debug.Log($"desinfileirei '{tk}'");
            if (tk == "\n"){
                codeCompilerLine++ ;
                continue;
            }
            if (known_tokens.ContainsKey(tk)){
                token = known_tokens[tk];
                if (firstBlocks.Contains(token["t"])){
                    lineCommands.Add(token["Commands"]);
                    callFuncByString(token["t"]);
                    // string __c = "";
                    // foreach (Commands _c_ in lineCommands){
                    //     __c += $"'{_c_.ToString()}',";
                    // }
                    //     Debug.Log(__c.ToString());
                    commands.Add(lineCommands);
                }
                else if (tk == "\n"){
                    codeCompilerLine++;
                }
                else
                {
                    throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} o token '{tk}' não era esperado");
                }

            }else if(Regex.IsMatch(tk,"[A-z_][A-z0-9_]*"))
            {
                throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} o token '{tk}' não foi reconhecido");
            }
            else
            {
                throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} o token '{tk}' não foi reconhecido");
            }
        }
        if(stack_braces.Count != 0){
            throw new Exception($"ERRO DE COMPILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, alguma chaves foi aberta e não foi fechada!");
        }
    }

    private void checkTokenContinuation(ref Queue<string> written_code, ref string tk, string pattern)
    {
        // string c_token = tk;
        // Debug.Log(written_code.ToString());
        // written_code.Peek();
        if (pattern == "[\\W]" && written_code.Count > 0 && known_tokens.ContainsKey(tk+written_code.Peek()))
        {
            while (written_code.Count > 0 && known_tokens.ContainsKey(tk + written_code.Peek())){
                tk += written_code.Dequeue();
            }
            return;
        }
        else if(pattern == "[\\W]" && written_code.Count > 0 && ! known_tokens.ContainsKey(tk + written_code.Peek()) && known_tokens.ContainsKey(tk))
        {
            //trata de números negativos
            if (tk != "-"){
                return;
            }
            while(Regex.IsMatch(written_code.Peek(), "[0-9]")){
                tk += written_code.Dequeue();
            }
            if(written_code.Peek() == "."){
                tk += written_code.Dequeue();
                while (written_code.Count > 0 && Regex.IsMatch(written_code.Peek(), "[0-9]"))
                {
                    tk += written_code.Dequeue();
                }
            }
            return;
        }
        else if(pattern == "[\\W]" && written_code.Count > 0 && !known_tokens.ContainsKey(tk + written_code.Peek()) && !known_tokens.ContainsKey(tk))
        {
            if (Regex.IsMatch(written_code.Peek(), pattern))
            {
                throw new Exception($"ERRO DE COMPIILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} token '{tk+written_code.Peek()}' não reconhecido");

            }
            else
            {
                throw new Exception($"ERRO DE COMPIILAÇÃO: No {codeInputBlockNumber}o bloco de código escrito, na linha {codeCompilerLine} token '{tk}' não reconhecido");
            }
        }
        while (written_code.Count > 0 && Regex.IsMatch(written_code.Peek(), pattern))
        {
            tk += written_code.Dequeue();
        }
        // Isso aqui me ajuda a ter um ponto só em números
        if (written_code.Count > 0 && pattern == "[0-9]" && written_code.Peek() == ".")
        {
            tk += written_code.Dequeue();
            while (written_code.Count > 0 && Regex.IsMatch(written_code.Peek(), pattern))
            {
                tk += written_code.Dequeue();
            }
        }

    }

    private List<List<Commands>> newCodeCompiler(CodeController codeBlock){

        codeInputBlockNumber++;
        Queue<string> written_code = new Queue<string>{};
        tokens = new Queue<string>();
        commands = new List<List<Commands>>();
        numbersQueueConditionals = new Queue<dynamic>();    
        foreach (char c in codeBlock.CodeWithin) {
            written_code.Enqueue(c.ToString());
        }
        
        while(written_code.Count > 0)
        {
            string current_char = written_code.Dequeue();
            // Debug.Log(current_char);
            if (Regex.IsMatch(current_char, "/"))
            {
                if (written_code.Peek() == "/"){
                    written_code.Dequeue();
                    while(written_code.Count > 0 && written_code.Dequeue() != "\n"){
                        continue;
                    }
                    // codeCompilerLine++;
                }

            }
            if (Regex.IsMatch(current_char, "[\n\r]"))
            {
                current_token = current_char;
                codeCompilerLine++;

            }
            else if(Regex.IsMatch(current_char,"[ \t]"))
            {
                continue;
            }
            else if(Regex.IsMatch(current_char, "[0-9]")){
                current_token = current_char;
                checkTokenContinuation(ref written_code, ref current_token, "[0-9]");
            }
            else if (Regex.IsMatch(current_char, "[A-z_]"))
            {
                current_token = current_char;
                checkTokenContinuation(ref written_code, ref current_token, "[A-z0-9_]");
            }
            else if (Regex.IsMatch(current_char, "[\\W]"))
            {
            // reading_symbol = true;
            current_token = current_char;
            checkTokenContinuation(ref written_code, ref current_token, "[\\W]");

            }
            tokens.Enqueue(current_token);
            current_token = "";
        }  

        parser();

        // foreach (List<Commands> commands1 in commands){
        //     Debug.Log("-------------------------------");
        //     foreach (Commands command in commands1){
        //         Debug.Log(command.ToString());
        //     }
        // }
        return commands;
    }

    public bool Compile(List<BlockController> blocks, ref string compileResult)
    {
        codeInputBlockNumber = 0;
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
                    Debug.Log("chamei o newcompiler");
                    codeCommands = newCodeCompiler(block as CodeController);
                    Debug.Log("saí do newcompiler" + codeCommands.Count);
                }
                catch (Exception e)
                {
                    compileResult = e.Message;
                    return false;
                }
                // Stack<int> stack_conditionals_inverted = new Stack<int>();

                //Inverte a pilha de conndicionais
                // int tamIntsStackConditionalsAUX = numbersQueueConditionals.Count;
                // Debug.Log("stack e tal né " + numbersQueueConditionals.Count);
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
            // Debug.Log(conditionalCell.type);
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
        codeInputBlockNumber = 0;
        return true;
    }


    public bool CompileCode(List<List<Commands>> blockCommands, ref string compileResult, ref int PC, ref bool hasAction)
    {
        int auxPC = PC;
        Stack<int> structuresStack = new Stack<int>();
        Stack<int> breakStackIndexes = new Stack<int>();
        List<int> whilesAndForsList = new List<int> { };

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
            Debug.Log(mainCommand.ToString());
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
                    Debug.Log("tinha nada");
                    compileResult = "ERRO DE COMPILAÇÃO: Bloco ELSE sem IF correspondente";
                    return false;
                }

                int lastStructureIndex = structuresStack.Pop();
                Cell lastStructure = memory[lastStructureIndex];
                if (!(lastStructure is IfCell))
                {
                    Debug.Log("nao era if");
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
                if (variableName == Commands.NUMBER)
                {
                    
                }

                memory[PC] = new ForCell(variableName,numbersQueueConditionals.Dequeue());
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
                    case Commands.NEGATIVE:
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
                        dynamic aux = variableName == Commands.NUMBER ? numbersQueueConditionals.Dequeue() : -1;
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

                        dynamic aux1 = variable1Name == Commands.NUMBER ? numbersQueueConditionals.Dequeue() : -1;
                        dynamic aux2 = variable2Name == Commands.NUMBER ? numbersQueueConditionals.Dequeue() : -1;
                        Debug.Log($"vars for {comparatorCommand.ToString()} are {aux1} and {aux2}");
                        switch (comparatorCommand)
                        {
                            case Commands.EQUALS:
                                comparatorCell = new EqualsCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(EqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.NOT_EQUALS:
                                comparatorCell = new NotEqualsCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(NotEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER:
                                comparatorCell = new GreaterCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(GreaterCell), comparator: comparatorCell);
                                break;
                            case Commands.GREATER_EQUALS:
                                comparatorCell = new GreaterEqualsCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(GreaterEqualsCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER:
                                comparatorCell = new LesserCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(LesserCell), comparator: comparatorCell);
                                break;
                            case Commands.LESSER_EQUALS:
                                comparatorCell = new LesserEqualsCell(variable1Name, variable2Name, variable1NUMBER: aux1, variable2NUMBER: aux2);
                                conditionalCell = new ConditionalCell(typeof(LesserEqualsCell), comparator: comparatorCell);
                                break;
                            default:
                                compileResult = "ERRO DE COMPILAÇÃO: Alguma estrutura condicional contém um erro ";
                                return false;
                        }
                        break;
                }
                // Debug.Log(conditionalCell.type);
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
                Debug.Log("colocou um if ");
                structureStart = new IfCell(conditionalList);
            }
            else
            {
                structureStart = new WhileCell(conditionalList);
                whilesAndForsList.Add(PC);
            }
            memory[PC] = (Cell)structureStart;
            Debug.Log("colocou um if 2");
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


