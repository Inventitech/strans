using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Transformation.Text;
using Microsoft.ProgramSynthesis.Wrangling.Constraints;
using CommandLine;

class Options
{
    [Option('b', "before", SetName = "SingleExample", Required = true, HelpText = "An example of an input line before transformation")]
    public String before { get; set; }
    [Option('a', "after", SetName = "SingleExample", Required = true, HelpText = "An example of an output line after transformation")]
    public String after { get; set; }

    [Option('f', "example-file", SetName = "MultiExample", Required = true, HelpText = "A file containing one or multiple transformation examples. The before and after transfer string are separated by => on the same line. One line per example.")]
    public String multiFile { get; set; }
}


class MyProgram
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<Options>(args)
            .MapResult(
                options => run(options),
                _ => 1);
    }

    static int run(Options options)
    {
        Session session = new Session();

        List<Example> examples = buildExamples(options);
        session.Constraints.Add(examples);
        Program program = session.Learn();

        processInputPipe(program);

        return 0;
    }

    static List<Example> buildExamples(Options options)
    {
        List<Example> examples = new List<Example>();
        var before = options.before.Trim();
        var after = options.after.Trim();

        examples.Add(new Example(new InputRow(before), after));

        return examples;
    }

    static List<Example> buildExamples(string file)
    {
        List<Example> examples = new List<Example>();
        string[] lines = System.IO.File.ReadAllLines(file);

        foreach (string line in lines)
        {
            var example = line.Split("=>");
            if (example.Length != 2)
            {
                Console.Error.WriteLine("Wrongly formatted line in examples: " + line);
            }
            else
            {
                examples.Add(new Example(new InputRow(example[0]), example[1]));
            }
        }
        return examples;
    }


    static void processInputPipe(Program program)
    {
        string s;
        while ((s = Console.ReadLine()) != null)
        {
            Console.WriteLine(program.Run(new InputRow(s)));
        }
    }
}
