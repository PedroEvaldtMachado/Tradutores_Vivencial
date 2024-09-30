using sly.lexer;
using sly.parser.generator;
using sly.parser.parser;
using System.Globalization;

namespace Tradutores_Vivencial1;
public class Parser
{
    public Parser()
    {

    }

    public List<TokenTipoValor> Variaveis { get; set; } = [];
    public Stack<ResultadoCondicao> Condicoes { get; set; } = new Stack<ResultadoCondicao>();
    public List<Func<object>> Funcoes { get; set; } = [];


    [Production("declaration: type IDENTIFIER multipleDeclaration* SEMICOLON[d]")]
    public object Declaration(Token<Tokens> typeToken, Token<Tokens> firstIdentifier, List<object> tokens)
    {
        var identifiers = new List<Token<Tokens>> { firstIdentifier };
        identifiers.AddRange(tokens.Select(t => (Token<Tokens>)t));

        foreach (var token in identifiers)
        {
            Variaveis.Add(new TokenTipoValor() { Token = token, TipoValor = typeToken.TokenID });
        }

        return $"{typeToken.Value} {string.Join(", ", identifiers.Select(t => t.Value))};";
    }

    [Production("multipleDeclaration: COMMA[d] IDENTIFIER")]
    public object MultipleDeclaration(Token<Tokens> identifier)
    {
        return identifier;
    }

    [Production("type: CHAR ARRAY?")]
    public object TypeChar(Token<Tokens> typeToken, Token<Tokens> array)
    {
        return typeToken;
    }

    [Production("type: INT")]
    public object TypeInt(Token<Tokens> typeToken)
    {
        return typeToken;
    }

    [Production("type: FLOAT")]
    public object TypeFloat(Token<Tokens> typeToken)
    {
        return typeToken;
    }

    [Production("typeArray: ARRAY")]
    public object TypeArray(Token<Tokens> typeToken)
    {
        return typeToken;
    }

    [Production("instruction: [if_statement|while_statement|declaration|simglecommand]*")]
    public object Instruction(List<object> instrucoes)
    {
        return string.Join("\n", instrucoes).Trim();
    }

    [Production("if_statement: IF[d] LBRACKET[d] ifcondition RBRACKET[d] ifblock (ELSE[d] elseblock)?")]
    public object IfStatement(object condition, object ifBlock, ValueOption<Group<Tokens, object>> elseBlock)
    {
        var elsePart = elseBlock.IsSome ? $" else {elseBlock.Match(r => r, () => null).Items[0].Value}" : "";
        Condicoes.Pop();
        return $"if ({((ResultadoCondicao)condition).Expressao}) {ifBlock}{elsePart}";
    }

    [Production("while_statement: WHILE[d] LBRACKET[d] whilecondition RBRACKET[d] whileblock")]
    public object WhileStatement(object condition, object block)
    {
        Condicoes.Pop();
        return $"while ({((ResultadoCondicao)condition).Expressao}) {block}";
    }

    [Production("ifcondition: condition")]
    public object IfCondition(object conditionOriginal)
    {
        if (conditionOriginal is ResultadoCondicao condition)
        {
            condition.Comando = Tokens.IF;
            Condicoes.Push(condition);
        }

        return conditionOriginal;
    }

    [Production("whilecondition: condition")]
    public object WhileCondition(object conditionOriginal)
    {
        if (conditionOriginal is ResultadoCondicao condition)
        {
            condition.Comando = Tokens.WHILE;
            Condicoes.Push(condition);
        }

        return conditionOriginal;
    }

    [Production("condition: [IDENTIFIER|INTVALUES|NUMBERVALUES|CHARVALUES] RELATIONAL_OPERATOR [IDENTIFIER|INTVALUES|NUMBERVALUES|CHARVALUES]")]
    public object Condition(Token<Tokens> left, Token<Tokens> op, Token<Tokens> right)
    {
        var retorno = new ResultadoCondicao
        {
            Expressao = $"{left.Value} {op.Value} {right.Value}",
            Resultado = Condicao(left, op.Value, right)
        };

        retorno.Tokens.Add(left);
        retorno.Tokens.Add(right);

        return retorno;
    }

    [Production("simglecommand: command")]
    public object SingleCommand(object command)
    {
        foreach (var funcao in Funcoes)
        {
            funcao();
        }

        Funcoes.Clear();

        return command;
    }

    [Production("command: IDENTIFIER ASSIGNMENT_OPERATOR [IDENTIFIER|INTVALUES|NUMBERVALUES|CHARVALUES] (MATH_OPERATOR IDENTIFIER)* SEMICOLON[d]")]
    public object Command(Token<Tokens> identifier, Token<Tokens> op, Token<Tokens> first, List<Group<Tokens, object>> rest)
    {
        Funcoes.Add(() => Comando(identifier, op, first, rest, true));

        var expr = Comando(identifier, op, first, rest, false);

        return $"{identifier.Value} {op.Value} {expr};";
    }

    [Production("block: LBRACE[d] command* RBRACE[d]")]
    public object Block(List<object> commands)
    {
        return $"{{ {string.Join("\n", commands)} }}";
    }

    [Production("ifblock: block")]
    public object IfBlock(object block)
    {
        var cond = Condicoes.Peek();

        if (cond?.Resultado == true && Funcoes.Count != 0)
        {
            foreach (var function in Funcoes)
            {
                function();
            }
        }

        Funcoes.Clear();

        return block;
    }

    [Production("elseblock: block")]
    public object ElseBlock(object block)
    {
        var cond = Condicoes.Peek();

        if (cond?.Resultado == false && Funcoes.Count != 0)
        {
            foreach (var function in Funcoes)
            {
                function();
            }
        }

        Funcoes.Clear();

        return block;
    }

    [Production("whileblock: block")]
    public object WhileBlock(object block)
    {
        var cond = Condicoes.Peek();
        var res = true;

        while (cond?.Resultado == true && res)
        {
            foreach (var function in Funcoes)
            {
                function();
            }

            res = Condicao(cond.Tokens[0], cond.Expressao.Split(" ")[1], cond.Tokens[1]);
        }

        Funcoes.Clear();

        return block;
    }

    public Token<Tokens> Operacao(Token<Tokens> first, string operacao, Token<Tokens> last)
    {
        var variavel1 = ObterSeExistir(first);
        var variavel2 = ObterSeExistir(last);

        if (variavel1.TipoValor == Tokens.CHAR || first.TokenID == Tokens.CHARVALUES)
        {
            string valor = operacao switch
            {
                "+" => variavel1.Valor + (variavel2.Valor?.ToString() ?? string.Empty),
                _ => throw new NotImplementedException(),
            };

            return new Token<Tokens>(Tokens.CHAR, valor, first.Position);
        }
        else if (variavel1.TipoValor == Tokens.INT || first.TokenID == Tokens.INTVALUES)
        {
            var valorVariavel1 = (int)variavel1.Valor;
            var valorVariavel2 = Convert.ToInt32(variavel2.Valor);

            int valor = operacao switch
            {
                "+" => valorVariavel1 + valorVariavel2,
                "-" => valorVariavel1 - valorVariavel2,
                "*" => valorVariavel1 * valorVariavel2,
                "/" => valorVariavel1 / valorVariavel2,
                _ => throw new NotImplementedException(),
            };

            return new Token<Tokens>(Tokens.INT, valor.ToString(), first.Position);
        }
        else if (variavel1.TipoValor == Tokens.FLOAT || first.TokenID == Tokens.NUMBERVALUES)
        {
            var valorVariavel1 = (float)variavel1.Valor;
            var valorVariavel2 = (float)Convert.ToDouble(variavel2.Valor, CultureInfo.InvariantCulture);

            float valor = operacao switch
            {
                "+" => valorVariavel1 + valorVariavel2,
                "-" => valorVariavel1 - valorVariavel2,
                "*" => valorVariavel1 * valorVariavel2,
                "/" => valorVariavel1 / valorVariavel2,
                _ => throw new NotImplementedException(),
            };

            return new Token<Tokens>(Tokens.FLOAT, valor.ToString(CultureInfo.InvariantCulture), first.Position);
        }

        throw new NotImplementedException();
    }

    public Token<Tokens> Atribuicao(ref Token<Tokens> first, string operacao, Token<Tokens> last)
    {
        var variavel = ObterSeExistir(first);

        if (variavel.TipoValor == Tokens.CHAR || first.TokenID == Tokens.CHARVALUES)
        {
            variavel.Valor = operacao switch
            {
                "=" => last,
                _ => throw new NotImplementedException(),
            };
        }
        else if (variavel.TipoValor == Tokens.INT || first.TokenID == Tokens.INTVALUES)
        {
            var valor = operacao switch
            {
                "=" => last.Value,
                "+=" => Operacao(first, operacao[0].ToString(), last).Value,
                "-=" => Operacao(first, operacao[0].ToString(), last).Value,
                "*=" => Operacao(first, operacao[0].ToString(), last).Value,
                "/=" => Operacao(first, operacao[0].ToString(), last).Value,
                _ => throw new NotImplementedException(),
            };

            variavel.Valor = Convert.ToInt32(valor);
        }
        else if (variavel.TipoValor == Tokens.FLOAT || first.TokenID == Tokens.NUMBERVALUES)
        {
            var valor = operacao switch
            {
                "=" => last.Value,
                "+=" => Operacao(first, operacao[0].ToString(), last).Value,
                "-=" => Operacao(first, operacao[0].ToString(), last).Value,
                "*=" => Operacao(first, operacao[0].ToString(), last).Value,
                "/=" => Operacao(first, operacao[0].ToString(), last).Value,
                _ => throw new NotImplementedException(),
            };

            variavel.Valor = (float)Convert.ToDouble(valor, CultureInfo.InvariantCulture);
        }

        return variavel.Token;
    }

    public bool Condicao(Token<Tokens> first, string condicao, Token<Tokens> last)
    {
        var variavel1 = ObterSeExistir(first);
        var variavel2 = ObterSeExistir(last);

        if (variavel1.TipoValor == Tokens.CHAR || first.TokenID == Tokens.CHARVALUES)
        {
            return condicao switch
            {
                "==" => variavel1.Valor == variavel2.Valor,
                "!=" => variavel1.Valor != variavel2.Valor,
                _ => throw new NotImplementedException(),
            };
        }
        else if (variavel1.TipoValor == Tokens.INT || first.TokenID == Tokens.INTVALUES)
        {
            var valorVariavel1 = (int)variavel1.Valor;
            var valorVariavel2 = Convert.ToInt32(variavel2.Valor);

            return condicao switch
            {
                "==" => valorVariavel1 == valorVariavel2,
                "!=" => valorVariavel1 != valorVariavel2,
                "<=" => valorVariavel1 <= valorVariavel2,
                ">=" => valorVariavel1 >= valorVariavel2,
                "<" => valorVariavel1 < valorVariavel2,
                ">" => valorVariavel1 > valorVariavel2,
                _ => throw new NotImplementedException(),
            };
        }
        else if (variavel1.TipoValor == Tokens.FLOAT || first.TokenID == Tokens.NUMBERVALUES)
        {
            var valorVariavel1 = (float)variavel1.Valor;
            var valorVariavel2 = (float)Convert.ToDouble(variavel2.Valor, CultureInfo.InvariantCulture);

            return condicao switch
            {
                "==" => valorVariavel1 == valorVariavel2,
                "!=" => valorVariavel1 != valorVariavel2,
                "<=" => valorVariavel1 <= valorVariavel2,
                ">=" => valorVariavel1 >= valorVariavel2,
                "<" => valorVariavel1 < valorVariavel2,
                ">" => valorVariavel1 > valorVariavel2,
                _ => throw new NotImplementedException(),
            };
        }

        throw new NotImplementedException();
    }

    public string Comando(Token<Tokens> identifier, Token<Tokens> op, Token<Tokens> first, List<Group<Tokens, object>> rest, bool execute)
    {
        var operando = first;
        var expr = first.Value;

        foreach (var group in rest)
        {
            if (execute)
            {
                operando = Operacao(operando, group.Token(0).Value, group.Token(1));
            }

            expr += $" {group.Token(0).Value} {group.Token(1).Value}";
        }

        if (execute)
        {
            Atribuicao(ref identifier, op.Value, operando);
        }

        return expr;
    }

    public TokenTipoValor ObterSeExistir(Token<Tokens> token)
    {
        return Variaveis.FirstOrDefault(t => t.MesmoToken(token)) ?? new TokenTipoValor() { Token = token, TipoValor = token.TokenID, Valor = token.Value };
    }
}

public class TokenTipoValor
{
    public Token<Tokens> Token { get; set; }
    public Tokens TipoValor { get; set; }
    public object Valor { get; set; }

    public bool MesmoToken(Token<Tokens> token2)
    {
        return MesmoToken(Token, token2);
    }

    public static bool MesmoToken(Token<Tokens> token, Token<Tokens> token2)
    {
        return token.TokenID == token2.TokenID && token.Value == token2.Value;
    }
}

public class ResultadoCondicao
{
    public Tokens Comando { get; set; } = Tradutores_Vivencial1.Tokens.WHITESPACE;
    public string Expressao { get; set; }
    public bool Resultado { get; set; }
    public List<Token<Tokens>> Tokens { get; set; } = [];
}