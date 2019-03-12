using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Transformation.Text;
using Microsoft.ProgramSynthesis.Wrangling.Constraints;

using CommandLine;
using CommandLine.Text;
using Microsoft.ProgramSynthesis.AST;
using System.Linq;

namespace CommandLine.Text
{
    class Options
    {
        [Option('b', "before", SetName = "SingleExample", Required = true, HelpText = "An example of an input line before transformation")]
        public String before { get; set; }
        [Option('a', "after", SetName = "SingleExample", Required = true, HelpText = "An example of an output line after transformation")]
        public String after { get; set; }

        [Option('f', "example-file", SetName = "MultiExample", Required = true, HelpText = "A file containing one or multiple transformation examples. The before and after transfer string are separated by => on the same line. One line per example.")]
        public String exampleFile { get; set; }


        [Option("save", Required = false, HelpText = "Save the inferred program in a file, based on the examples. Does not execute the actual program.")]
        public String save { get; set; }

        [Option("load", Required = true, SetName = "Serialization", HelpText = "Load a previously inferred program from a file.")]
        public String load { get; set; }

        [Option("describe", Required = false, HelpText = "Print-out a human-readable description of the inferred program, based on the examples. Do not perform any actual string transformation.")]
        public bool describe { get; set; }


        [Usage(ApplicationAlias = "strans")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Standard usage where strans infers a string transformation rule from one example, extracting the file extension from the input on STDIN (for example, from ls)", new Options { before = "file.bin", after = "bin" });
                yield return new Example("Usage with multiple examples to infer string transformation rules stored in file examples. Syntax in example is one transformation example per line as before => after", new Options { exampleFile = "examples" });
            }
        }
    }

}

namespace Microsoft.ProgramSynthesis.Transformation.Text
{

    class MyProgram
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => run(options),
                    errors => handleErrors(errors));
        }

        static int handleErrors(IEnumerable<Error> errors)
        {
            if (errors.Any(l => l.GetType() == typeof(VersionRequestedError)))
            {
                Console.Out.WriteLine();
            }

            return 0;
        }

        static Program buildProgram(Options options)
        {
            Session session = new Session();

            if (options.load != null)
            {
                string program = System.IO.File.ReadAllText(options.load);
                return session.Load(program, ASTSerializationFormat.XML);
            }

            HashSet<Example> examples;
            if (options.exampleFile != null)
            {
                examples = buildExamples(options.exampleFile);
            }
            else
            {
                examples = buildExamples(options);
            }

            if (examples.Count < 1)
            {
                Console.Error.WriteLine(@"Error: You need to provide at least one valid transformation example of form 'before => after'.");
                return null;
            }

            session.Constraints.Add(examples);
            return session.Learn();
        }

        static int run(Options options)
        {
            Program program = buildProgram(options);

            if (program == null)
            {
                Console.Error.WriteLine(@"Error: Given your transformation examples, no program could be learned.");
                return -1;
            }

            if (options.describe)
            {
                Console.Out.WriteLine(program.Serialize(ASTSerializationFormat.HumanReadable));
                return 0;
            }

            if (options.save != null)
            {
                System.IO.File.WriteAllText(options.save, program.Serialize(ASTSerializationFormat.XML));
                return 0;
            }

            processInputPipe(program);

            return 0;
        }

        static HashSet<Example> buildExamples(Options options)
        {
            HashSet<Example> examples = new HashSet<Example>();
            var before = options.before.Trim();
            var after = options.after.Trim();

            examples.Add(new Example(new InputRow(before), after));

            return examples;
        }

        static HashSet<Example> buildExamples(string file)
        {
            HashSet<Example> examples = new HashSet<Example>();
            string[] lines = System.IO.File.ReadAllLines(file);

            foreach (string line in lines)
            {
                if (line.Trim().Equals(System.String.Empty))
                {
                    continue;
                }

                var example = line.Split(@"=>");
                if (example.Length != 2)
                {
                    Console.Error.WriteLine(@"Wrongly formatted line in examples: " + line);
                }
                else
                {
                    examples.Add(new Example(new InputRow(example[0].Trim()), example[1].Trim()));
                }
            }
            return examples;
        }


        static void processInputPipe(Program program)
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                Console.WriteLine(program.Run(new InputRow(line)));
            }
        }
    }
}