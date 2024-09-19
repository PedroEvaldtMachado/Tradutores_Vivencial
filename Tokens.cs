using sly.lexer;

namespace Tradutores_Vivencial1;

public enum Tokens
{
    [Lexeme("(if)")]
    IF,

    [Lexeme("(else)")]
    ELSE,

    [Lexeme("(while)")]
    WHILE,

    [Lexeme("(char)")]
    CHAR,

    [Lexeme("\\[\\]")]
    ARRAY,

    [Lexeme("(int)")]
    INT,

    [Lexeme("(float)")]
    FLOAT,

    [Lexeme(";")]
    SEMICOLON,

    [Lexeme(",")]
    COMMA,

    [Lexeme("(==|!=|<=|>=|<|>)")]
    RELATIONAL_OPERATOR,

    [Lexeme("[+\\-*/]")]
    MATH_OPERATOR,

    [Lexeme("(=|\\+=|-=|\\*=|/=|\\+\\+|--)")]
    ASSIGNMENT_OPERATOR,

    [Lexeme("[(]")]
    LBRACKET,

    [Lexeme("[)]")]
    RBRACKET,

    [Lexeme("[{]")]
    LBRACE,

    [Lexeme("[}]")]
    RBRACE,

    [Lexeme("[A-z_]{1}[0-9A-z_]*")]
    IDENTIFIER,

    [Lexeme("[ \\r\\t]+", isSkippable: true)]
    WHITESPACE,

    [Lexeme("[\\n]", isSkippable: true, isLineEnding: true)]
    LINEENDING
}