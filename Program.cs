using sly.parser;
using sly.parser.generator;

namespace Tradutores_Vivencial1;

static class Program
{
    static async Task Main(string[] args)
    {
        var files = Directory.GetFiles(Directory.GetCurrentDirectory());

        files = files.Where(f => Path.GetFileName(f).StartsWith("teste") && Path.GetExtension(f) == ".txt").OrderBy(n => n).ToArray();

        foreach (var file in files)
        {
            string content = null;

            if (File.Exists(file))
            {
                content = await File.ReadAllTextAsync(file);

                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"Arquivo: {Path.GetFileName(file)}\nConteúdo do arquivo:\n");
                Console.WriteLine(content);
                Console.WriteLine();

                var parser = new Parser();
                var parserInstance = new ParserBuilder<Tokens, object>()
                    .BuildParser(parser, ParserType.EBNF_LL_RECURSIVE_DESCENT, "instruction");

                if (parserInstance.IsOk)
                {
                    var resultParser = new ParseResult<Tokens, object>() { IsError = true };
                    var awaiter = Task.Run(() => resultParser = parserInstance.Result.Parse(content));

                    await Task.WhenAny(awaiter, Task.Delay(TimeSpan.FromSeconds(2)));

                    if (resultParser.IsOk)
                    {
                        Console.WriteLine($"Resultado da analise sintática: Sucesso!");
                        Console.WriteLine($"Resultado escrito:\n{resultParser.Result}\n");
                        Console.WriteLine($"Valor final variáveis:");

                        foreach (var item in parser.Variaveis)
                        {
                            Console.WriteLine($"{item.Token.Value}: {item.Valor}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Resultado da analise sintática: Falha!");

                        if (resultParser.Errors is not null)
                        {
                            foreach (var error in resultParser.Errors)
                            {
                                Console.WriteLine(error);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Erro ao converter o arquivo.");
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