using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal enum TokenType
    {
        BEGIN,
        END,
        INT,
        CHAR,
        BOOL,
        FLOAT,
        IDENTIFIER,
        COMMENT,
        OPERATOR,
        SEMICOLON,
        COMMA,
        STRING,
        ERROR
    }
}
