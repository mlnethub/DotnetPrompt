﻿using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using DotnetPrompt.Abstractions.Chains;

namespace DotnetPrompt.Chains.Specialized;

/// <summary>
/// Chain that represent sequential list of chains
/// </summary>
public class SequentialChain : IChain
{
    /// <summary>
    /// List of inner chains of SequentialChain
    /// </summary>
    public IReadOnlyList<IChain> Chains { get; }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="chains">List of chain that would be linked together where input should match output</param>
    public SequentialChain(IReadOnlyList<IChain> chains)
    {
        Chains = chains;

        for (var i = 0; i < chains.Count - 1; i++)
        {
            chains[i].LinkTo(chains[i + 1]);
        }
    }

    /// <inheritdoc />
    public ITargetBlock<ChainMessage> InputBlock => Chains[0].InputBlock;

    /// <inheritdoc />
    public ISourceBlock<ChainMessage> OutputBlock => Chains[^1].OutputBlock;

    /// <inheritdoc />
    public IList<string> InputVariables => Chains[0].InputVariables;

    /// <inheritdoc />
    public string DefaultOutputKey { get; set; } = "text";
}