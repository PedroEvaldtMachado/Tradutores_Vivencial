using sly.lexer;
using sly.parser.generator;
using sly.parser.parser;

namespace Tradutores_Vivencial1;

public class Parser
{
    [Production("declaration: type IDENTIFIER multipleDeclaration* SEMICOLON[d]")]
    public string Declaration(string type, Token<Tokens> firstIdentifier, List<string> otherIdentifiers)
    {
        var identifiers = new List<string> { firstIdentifier.Value };
        identifiers.AddRange(otherIdentifiers);

        return $"{type} {string.Join(", ", identifiers)};";
    }

    [Production("multipleDeclaration: COMMA[d] IDENTIFIER")]
    public string MultipleDeclaration(Token<Tokens> identifier)
    {
        return identifier.Value.Trim();
    }

    [Production("type: CHAR typeArray?")]
    public string TypeChar(Token<Tokens> typeToken, ValueOption<string> array)
    {
        return typeToken.Value.Trim() + array.Match(r => r, () => string.Empty);
    }

    [Production("type: INT")]
    public string TypeInt(Token<Tokens> typeToken)
    {
        return typeToken.Value.Trim();
    }

    [Production("type: FLOAT")]
    public string TypeFloat(Token<Tokens> typeToken)
    {
        return typeToken.Value.Trim();
    }

    [Production("typeArray: ARRAY")]
    public string TypeArray(Token<Tokens> typeToken)
    {
        return typeToken.Value.Trim();
    }

    [Production("instruction: [declaration|if_statement|while_statement|command]*")]
    public string Instruction(List<string> instrucoes)
    {
        return string.Join("", instrucoes).Trim();
    }

    [Production("if_statement: IF[d] LBRACKET[d] condition RBRACKET[d] block (ELSE[d] block)?")]
    public string IfStatement(string condition, string ifBlock, ValueOption<Group<Tokens, string>> elseBlock)
    {
        var elsePart = elseBlock.IsSome ? $" else {elseBlock.Match(r => r, () => null).Items[0].Value}" : "";
        return $"if ({condition}) {ifBlock}{elsePart}";
    }

    [Production("while_statement: WHILE[d] LBRACKET[d] condition RBRACKET[d] block")]
    public string WhileStatement(string condition, string block)
    {
        return $"while ({condition}) {block}";
    }

    [Production("condition: IDENTIFIER RELATIONAL_OPERATOR IDENTIFIER")]
    public string Condition(Token<Tokens> left, Token<Tokens> op, Token<Tokens> right)
    {
        return $"{left.Value} {op.Value} {right.Value}";
    }

    [Production("command: IDENTIFIER ASSIGNMENT_OPERATOR expression SEMICOLON")]
    public string Command(Token<Tokens> identifier, Token<Tokens> op, string expression, Token<Tokens> end)
    {
        return $"{identifier.Value} {op.Value} {expression};";
    }

    [Production("expression: IDENTIFIER (MATH_OPERATOR IDENTIFIER)*")]
    public string Expression(Token<Tokens> first, List<Group<Tokens, string>> rest)
    {
        var expr = first.Value;

        foreach (var group in rest)
        {
            expr += $" {group.Token(0).Value} {group.Token(1).Value}";
        }

        return expr;
    }

    [Production("block: LBRACE[d] command* RBRACE[d]")]
    public string Block(List<string> commands)
    {
        return $"{{ {string.Join(" ", commands)} }}";
    }
}