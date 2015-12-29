# Textc

**Note: This is a work in progress and is subject to change without notice.**

**Text** is a natural language parser library that allows developers to build text-commands-based applications.


## NuGet

[**Takenet.Textc**](https://www.nuget.org/packages/Takenet.Textc)


## Concepts

* Token - 
* Token template - A definition 
* Syntax
* Text processor
* Expression
* Command processor
* Context


### CSDL

The **Command Syntax Definition Language** is the notation used by the library to allow developers define command syntaxes in a non-programmatic way.

A CSDL declaration is composed by one or more token templates, each one defined in the following way:


```
name:type(initializer)
```

Where:

* **name** - The name of the token to be extracted, which must be unique in each declaration. This value is used to store values in the context or to bind to method parameters. Optional.
* **type** - The type of the token to be extracted. The library define some basic types, like `Word` and `Integer`, but the developer can define its own types. Mandatory.
* **initializer** - The initialization date for the token template, which is used in specific ways accordingly to the type.  For instance, in the `Word` type, it is used to define the set of words that can be parsed by the template. Optional.

For instance, if we have a calculator that must accept commands like `sum 1 and 2` where `1` and `2` are variable values, the CSDL for this syntax is:

```
command:Word(sum) num1:Integer conjunction:Word(and) num2:Integer
```

We can simplify our syntax by omitting the name of tokens that we known that will not be used in the command execution, like some language constructs. 
In this case, the syntax will be:

```
:Word(sum) num1:Integer :Word(and) num2:Integer
```

We can define optional tokens in the syntax definition, meaning that they can be present or not in the user input, without changing the semantics of the text. 
To notate that, we must put a question mark (`?`) character after the type definition:

```
:Word(sum) num1:Integer :Word?(and) num2:Integer
```

Now, our syntax allow us to parse inputs like `sum 1 2`.


### Basic token types


| Name     | Description                         | Sample values       |
| ---------|-------------------------------------|---------------------|
| Decimal  | A culture-invariant decimal number. |  `-1.123`,`0.1231`,`1.1`, `131.2` |
| Integer  | A 32 bit number.                    |  `-1`,`0`,`1`, `300` |
| Long     | A 64 bit number.                    |  `-1`,`0`,`1`, `292908889192986509` |
| Word     | A text word. It consumes all the input until the next blank space character. You can limit the expected words in the token template initialization. | `Dog`, `cat`, `Banana`    |
| Text     | A text with multiple words. Note that this token type will consume all remaining input, so it must be the last to be parsed in a syntax.              | `This is a sentence`      |


### Special token types


| Name      | Description                         | Sample values       |
| ----------|-------------------------------------|---------------------|
| LDWord    | A levenshtein distance text word. It parses any word with a default distance of 2 characters from the initialization values. | `Mispelled` |
| Regex     | A text with an regex initializer. Note that this token type will consume all remaining input that matches the specified regex. | `Mycustom text`    |
| RegexWord | A text word with an regex initializer. It allows more flexible parsing rules. | `custom-word-pattern`    |



## Samples

### Calculator


Creating the text processor:

```csharp
// Fist, define the calculator methods
Func<int, int, int> sumFunc = (a, b) => a + b;
Func<int, int, int> subtractFunc = (a, b) => a - b;
Func<int, int, int> multiplyFunc = (a, b) => a * b;
Func<int, int, int> divideFunc = (a, b) => a / b;

// After that, the syntaxes for all operations, using the CSDL parser:
                        
// 1. Sum:            
// a) The default syntax, for inputs like 'sum 1 and 2' or 'sum 3 4'
var sumSyntax = CsdlParser.Parse("operation+:Word(sum) a:Integer :Word?(and) b:Integer");
// b) The alternative syntax, for inputs like '3 plus 4'
var alternativeSumSyntax = CsdlParser.Parse("a:Integer :Word(plus,more) b:Integer");

// 2. Subtract:            
// a) The default syntax, for inputs like 'subtract 2 from 3'
var subtractSyntax = CsdlParser.Parse("operation+:Word(subtract,sub) b:Integer :Word(from) a:Integer");
// b) The alternative syntax, for inputs like '5 minus 3'
var alternativeSubtractSyntax = CsdlParser.Parse("a:Integer :Word(minus) b:Integer");

// 3. Multiply:            
// a) The default syntax, for inputs like 'multiply 3 and 3' or 'multiply 5 2'
var multiplySyntax = CsdlParser.Parse("operation+:Word(multiply,mul) a:Integer :Word?(and,by) b:Integer");
// b) The alternative syntax, for inputs like '6 times 2'
var alternativeMultiplySyntax = CsdlParser.Parse("a:Integer :Word(times) b:Integer");

// 4. Divide:            
// a) The default syntax, for inputs like 'divide 3 by 3' or 'divide 10 2'
var divideSyntax = CsdlParser.Parse("operation+:Word(divide,div) a:Integer :Word?(by) b:Integer");
// b) The alternative syntax, for inputs like '6 by 2'
var alternativeDivideSyntax = CsdlParser.Parse("a:Integer :Word(by) b:Integer");

// Define a output processor that prints the command results to the console
var outputProcessor = new DelegateOutputProcessor<int>((o, context) => Console.WriteLine($"Result: {o}"));
            
// Now create the command processors, to bind the methods to the syntaxes
var sumCommandProcessor = DelegateCommandProcessor.Create(
    sumFunc,
    outputProcessor,
    sumSyntax,
    alternativeSumSyntax
    );
var subtractCommandProcessor = DelegateCommandProcessor.Create(
    subtractFunc,
    outputProcessor,
    subtractSyntax,
    alternativeSubtractSyntax
    );
var multiplyCommandProcessor = DelegateCommandProcessor.Create(
    multiplyFunc,
    outputProcessor,
    multiplySyntax,
    alternativeMultiplySyntax
    );
var divideCommandProcessor = DelegateCommandProcessor.Create(
    divideFunc,
    outputProcessor,
    divideSyntax,
    alternativeDivideSyntax
    );

// Finally, create the text processor and register all command processors
var textProcessor = new TextProcessor();
textProcessor.AddCommandProcessor(sumCommandProcessor);
textProcessor.AddCommandProcessor(subtractCommandProcessor);
textProcessor.AddCommandProcessor(multiplyCommandProcessor);
textProcessor.AddCommandProcessor(divideCommandProcessor);

```

Submitting the input:

```csharp
Console.Write("> ");

try
{
    await textProcessor.ProcessAsync(Console.ReadLine(), context);
}
catch (MatchNotFoundException)
{
    Console.WriteLine("There's no match for the specified input");
}
```

Results:

```
> sum 2 and 3
Result: 5

> 12 plus 3
Result: 15

> subtract 30 from 90
Result: 60

> 77 minus 27
Result: 50

> multiply 2 and 3
Result: 6

> 6 times 6
Result: 36

> divide 30 by 5
Result: 6

> 100 by 10
Result: 10

> sum two and three
There's no match for the specified input

```