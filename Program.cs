using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Transformation.Text;
using Microsoft.ProgramSynthesis.Wrangling.Constraints;
using CommandLine;

class Options
{
    [Option('b', "before", Required = true, HelpText = "An example of an input line before transformation")]
    public String before { get; set; }

    [Option('a', "after", Required = true, HelpText = "An example of an output line after transformation")]
    public String after { get; set; }
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
        var before = options.before.Trim();
        var after = options.after.Trim();

        IEnumerable<Example> examples = new[]
        {
                new Example(new InputRow(before), after)
            };
        session.Constraints.Add(examples);
        Program program = session.Learn();

        processInputPipe(program);

        return 0;
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
