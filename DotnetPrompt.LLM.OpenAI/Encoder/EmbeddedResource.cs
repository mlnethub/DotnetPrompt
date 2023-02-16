﻿// @author: Devis Lucato. @license: CC0.

using System;
using System.IO;
using System.Reflection;

namespace DotnetPrompt.LLM.OpenAI.Encoder;

internal static class EmbeddedResource
{
    private static readonly string? NAMESPACE = typeof(EmbeddedResource).Namespace;

    internal static string Read(string name)
    {
        var assembly = typeof(EmbeddedResource).GetTypeInfo().Assembly;
        if (assembly == null) throw new NullReferenceException($"[{NAMESPACE}] {name} assembly not found");

        using Stream? resource = assembly.GetManifestResourceStream($"{NAMESPACE}." + name);
        if (resource == null) throw new NullReferenceException($"[{NAMESPACE}] {name} resource not found");

        using var reader = new StreamReader(resource);
        return reader.ReadToEnd();
    }
}