# Interpreter Tings
This interpreter project is written using [C#](https://www.w3schools.com/cs/index.php) programming language. <br />
[Book about crafting interpreters](https://craftinginterpreters.com/introduction.html).

## Main Component
(add description for documentation purposes)

## Lexer Component
(add description for documentation purposes)

## Parser Component
### Parser Class
- The Parser class parses a list of tokens into an [abstract syntax tree (AST)](https://en.wikipedia.org/wiki/Abstract_syntax_tree). It has a constructor that initializes the list of tokens and the current position.
- The `Parse` method initiates the parsing process by calling the `Expression` method.
- The `Expression`, `Term`, and `Factor` methods implement the [recursive descent parsing algorithm](https://en.wikipedia.org/wiki/Recursive_descent_parser) to handle expressions, terms, and factors, respectively.

### Grammar Rules ([Extended Backus-Naur Form](https://en.wikipedia.org/wiki/Extended_Backus–Naur_form))
- Expression&nbsp;&nbsp;-> Term { ('+' | '-') Term }
- Term&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-> Factor { ('*' | '/') Factor }
- Factor&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-> Literal | Identifier | '(' Expression ')'

### ASTNode Heirarchy
- The ASTNode class is an abstract base class for all nodes in the abstract syntax tree.
- Subclasses like BinaryExpressionNode, LiteralNode, and IdentifierNode represent specific types of nodes in the AST.
- Each node class has properties to store relevant information, such as the operator, operands, literals, or identifiers.

### Utility Methods
- The `Match` method checks if the current token matches a specified type and value.
- The `Consume` method retrieves the current token and advances the position to the next token.
