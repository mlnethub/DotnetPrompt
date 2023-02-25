﻿using DotnetPrompt.Abstractions.Prompts;
using DotnetPrompt.LLM.OpenAI;
using DotnetPrompt.Prompts;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace DotnetPrompt.Tests.Examples.Prompts;

public class PromptTemplateExamples
{
    [Test]
    public void Example_CreatingPromptTemplate()
    {
        #region Example_CreatingPromptTemplate
        var template = "I want you to act as a naming consultant for new companies.\n\n" +
                       "Here are some examples of good company names:\n\n" +
                       "- search engine, Google\n" +
                       "- social media, Facebook\n" +
                       "- video sharing, YouTube\n\n" +
                       "The name should be short, catchy and easy to remember.\n\n" +
                       "What is a good name for a company that makes {product}?\n";

        var prompt = new PromptTemplate(
            template: template,
            inputVariables: new[] { "product" });
        #endregion

        #region Example_CreatingSeveralPromptTemplate
        // An example prompt with no input variables
        var noInputPrompt = new PromptTemplate("Tell me a joke.");
        Console.WriteLine(noInputPrompt.Format(new Dictionary<string, string>()));
        //> "Tell me a joke."

        //An example prompt with one input variable
        var oneInputPrompt =
            new PromptTemplate(template: "Tell me a {adjective} joke.", inputVariables: new[] { "adjective" });

        var valuesOneInput = new Dictionary<string, string>
        {
            { "adjective", "funny" }
        };
        Console.WriteLine(oneInputPrompt.Format(valuesOneInput));
        //> "Tell me a funny joke."

        //An example prompt with multiple input variables
        var multipleInputPrompt = new PromptTemplate("Tell me a {adjective} joke about {content}.",
            new[] { "adjective", "content" });

        var valuesMultipleInput = new Dictionary<string, string>
        {
            { "adjective", "funny" },
            { "content", "chickens" }
        };
        Console.WriteLine(multipleInputPrompt.Format(valuesMultipleInput));
        //> "Tell me a funny joke about chickens."
        #endregion
    }

    [Test]
    public async Task Example_CustomPromptTemplate()
    {
        #region Example_CustomPromptTemplate
        var funcExplainer = new FunctionExplainerPromptTemplate();

        var methods = PythonHelpers.GetPythonMethods("data/test_base.py").ToList();

        foreach (var (name, def, body) in methods)
        {
            // Generate a prompt for the function "format" if python file "data/few_shot.py"
            var values = new Dictionary<string, string>()
            {
                {"function_file","data/test_base.py"},
                {"function_name",name},
            };

            var prompt = funcExplainer.Format(values);
            //Console.WriteLine(prompt);
            // Arrange
            var llm = new OpenAIModel("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",
                OpenAIModelConfiguration.Default with { MaxTokens = -1 },
                NullLogger.Instance);

            // Act
            var output = await llm.PromptAsync(prompt);
            Console.WriteLine(output);
        }

        #endregion
    }

    #region Example_CustomPromptTemplate_FunctionExplainerPromptTemplate
    internal class FunctionExplainerPromptTemplate : IPromptTemplate
    {
        public FunctionExplainerPromptTemplate()
        {
            InputVariables = new List<string>() { "function_file", "function_name" };
        }

        public IList<string> InputVariables { get; set; }

        public string Format(IDictionary<string, string> values)
        {
            // Get the source code of the function
            var methods = PythonHelpers.GetPythonMethods(values["function_file"]);
            var method = methods.First(m => m.Name == values["function_name"]);

            // Generate the prompt to be sent to the language model
            var prompt = "Given the function python source code, generate an C# version of the function.\n" +
                         "Source Code:\n" +
                         method.Def +
                         method.Body +
                         "C# version of the function:\n";
            return prompt;
        }
    }
    #endregion
}