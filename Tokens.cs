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


    [Lexeme("[A-z_]{1}[0-9A-z_]*")]
    IDENTIFIER,


    [Lexeme("(==|!=|<=|>=|<|>)")]
    RELATIONAL_OPERATOR,

    [Lexeme("(=|\\+=|-=|\\*=|/=)")]
    ASSIGNMENT_OPERATOR,

    [Lexeme("[+\\-\\*/]")]
    MATH_OPERATOR,


    [Lexeme("[(]")]
    LBRACKET,

    [Lexeme("[)]")]
    RBRACKET,

    [Lexeme("[{]")]
    LBRACE,

    [Lexeme("[}]")]
    RBRACE,


    [Lexeme(";", isLineEnding: true)]
    SEMICOLON,

    [Lexeme(",")]
    COMMA,


    [Lexeme("[ \\t]+", isSkippable: true)]
    WHITESPACE,

    [Lexeme("(\\s\\n)", isSkippable: true, isLineEnding: true)]
    LINEENDING,

    [Lexeme("(\\n)", isLineEnding: true)]
    NEWLINE,


    [Lexeme("\"[^\"\\\\]*(\\\\.[^\"\\\\])*\"")]
    CHARVALUES,

    [Lexeme("[0-9]{1,}[.][0-9]*")]
    NUMBERVALUES,

    [Lexeme("[0-9]*")]
    INTVALUES
}