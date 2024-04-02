using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class Token
    {
        public enum TokenType
        {
            //Single-character tokens
            LEFT_PAREN,     // (
            RIGHT_PAREN,    // )
            LEFT_SQUARE,    // [
            RIGHT_SQUARE,   // ]
            MINUS,          // -
            PLUS,           // +
            SLASH,          // /
            STAR,           // *
            MODULO,         // %
            COMMENT,        // #
            CONCATENATE,    // &
            EQUAL,          // =
            //One or two character tokens
            NOT_EQUAL,      // <>
            EQUAL_EQUAL,    // ==
            GREATER,        // >
            GREATER_EQUAL,  // >=
            LESS,           // <
            LESS_EQUAL,     // <=
            //Literals
            IDENTIFIER,     // num1, varName, x, y
            LETTER,         // 'a', 'b', 'c'
            NUMBER,         // 22, 345.3
            STRING,         // 
            //Keywords
            INT,            // "INT"
            CHAR,           // "CHAR"
            FLOAT,          // "FLOAT"
            BOOL,           // "BOOL"
            TRUE,           // "TRUE"
            FALSE,          // "FALSE"
            IF,             // "IF"
            NOT,            // "NOT"
            AND,            // "AND"
            OR,             // "OR"
            DISPLAY,        // "DISPLAY"
            BEGIN,          // "BEGIN"
            END,            // "END"
            CODE,           // "CODE"

            EOF,
        }

        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"Type: {Type}, Value: {Value}";
        }
    }
}
