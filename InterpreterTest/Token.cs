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

            //Expression Seperators
            LEFT_PAREN,     // (
            RIGHT_PAREN,    // )

            //BinaryOperators
            MINUS,          // -
            PLUS,           // +
            SLASH,          // /
            STAR,           // *
            MODULO,         // %
            EXPONENT,       // ^

            //Commenting
            COMMENT,        // #

            //BinaryStringOperator
            CONCATENATE,    // &

            //AssignmentOperator
            EQUAL,          // =

            //AssignmentSeparator
            COMMA,          // ,

            COLON,          // :
            NEXT_LINE,      // $
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
            NUMBER,         // 22
            DECIMAL_NUMBER, // 345.3
            STRING,         // "Ola"

            //Keywords

            //DataTypes
            INT,            // "INT"
            CHAR,           // "CHAR"
            FLOAT,          // "FLOAT"
            BOOL,           // "BOOL"

            //Booleans
            TRUE,           // "TRUE"
            FALSE,          // "FALSE"

            //Control flow
            IF,             // "IF"
            ELSE,           // "ELSE"

            //Looping tings
            WHILE,          // "WHILE"

            //Logic Operators
            NOT,            // "NOT"
            AND,            // "AND"
            OR,             // "OR"
            XOR,            // "XOR"

            //Print
            DISPLAY,        // "DISPLAY"
            //Scan
            SCAN,           // "SCAN"

            //Blocking
            BEGIN,          // "BEGIN"
            END,            // "END"
            CODE,           // "CODE"
            
            LINE_SEPARATOR, // "\n"

            EOF,            // idk what this is supposed to do
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
