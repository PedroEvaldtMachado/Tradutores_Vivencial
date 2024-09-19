using sly.parser;
using sly.parser.generator;

namespace Tradutores_Vivencial1;

static class Program
{
    static void Main(string[] args)
    {
        var files = Directory.GetFiles(Directory.GetCurrentDirectory());

        files = files.Where(f => Path.GetFileName(f).StartsWith("teste") && Path.GetExtension(f) == ".txt").ToArray();

        foreach (var file in files)
        {
            string content = null;

            if (File.Exists(file))
            {
                content = File.ReadAllText(file);

                Console.WriteLine($"Arquivo: {Path.GetFileName(file)} | Conteúdo do arquivo:");
                Console.WriteLine(content);
                Console.WriteLine();

                var parserInstance = new ParserBuilder<Tokens, string>()
                    .BuildParser(new Parser(), ParserType.EBNF_LL_RECURSIVE_DESCENT, "instruction");

                if (parserInstance.IsOk)
                {
                    var lexerResult = parserInstance.Result.Lexer.Tokenize(content);

                    if (lexerResult.IsOk)
                    {
                        Console.WriteLine("Resultado da analise dos tokens: Sucesso!");
                        var tokens = lexerResult.Tokens;

                        foreach (var token in tokens)
                        {
                            Console.WriteLine($"{token.TokenID}: {token.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Resultado da analise dos tokens: Falha!");
                        Console.WriteLine($"{lexerResult.Error.ErrorMessage}");
                    }

                    var resultParser = parserInstance.Result.Parse(content);

                    if (resultParser.IsOk)
                    {
                        Console.WriteLine($"Resultado da analise sintática: Sucesso!");
                        Console.WriteLine($"Resultado: {resultParser.Result}");
                    }
                    else
                    {
                        Console.WriteLine($"Resultado da analise sintática: Falha!");
                        foreach (var error in resultParser.Errors)
                        {
                            Console.WriteLine(error);
                        }
                    }

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Falha ao gerar o parser.");
                    foreach (var error in parserInstance.Errors)
                    {
                        Console.WriteLine(error.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Arquivo não encontrado.");
            }

            Console.WriteLine();
        }

        if (!files.Any())
        {
            Console.WriteLine("Não encontrou arquivos para teste.");
        }

        Console.ReadLine();
    }

    private static void TestParse(Parser<Tokens, string> parser, string stringTest)
    {

    }
}