﻿using System;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Utilities;

namespace Stratis.Bitcoin.Consensus
{
    /// <summary>
    /// An engine that enforce the execution and validation of consensus rule.
    /// </summary>
    /// <remarks>
    /// In order for a block to be valid it has to successfully pass the rules checks.
    /// A block  that is not valid will result in the <see cref="ValidationContext.Error"/> as not <c>null</c>.
    /// </remarks>
    public interface IConsensusRuleEngine : IDisposable
    {
        /// <summary>
        /// Initialize the rules engine.
        /// </summary>
        /// <param name="chainTip">Last common header between chain repository and block store if it's available.
        /// <param name="consensusManager">The consensus manager.</param>
        void Initialize(ChainedHeader chainTip, IConsensusManager consensusManager);

        /// <summary>
        /// Register a new rule to the engine
        /// </summary>
        ConsensusRuleEngine SetupRulesEngineParent();

        /// <summary>
        /// Gets the consensus rule that is assignable to the supplied generic type.
        /// </summary>
        T GetRule<T>() where T : ConsensusRuleBase;

        /// <summary>
        /// Create an instance of the <see cref="RuleContext"/> to be used by consensus validation.
        /// </summary>
        /// <remarks>
        /// Each network type can specify it's own <see cref="RuleContext"/>.
        /// </remarks>
        RuleContext CreateRuleContext(ValidationContext validationContext);

        /// <summary>
        /// Retrieves the block hash and height of the current tip of the coinview (coin database).
        /// </summary>
        /// <returns>Block hash and height of the current tip of the coinview (coin database).</returns>
        HashHeightPair GetBlockHash();

        /// <summary>
        /// Rewinds the chain to the last saved state.
        /// <para>
        /// This operation includes removing the recent transactions
        /// and restoring the chain to an earlier state.
        /// </para>
        /// </summary>
        /// <param name="target">The final rewind target or <c>null</c> if a single block should be rewound. See remarks.</param>
        /// <returns>Hash of the block header which is now the tip of the chain.</returns>
        /// <remarks>This method can be implemented to rewind one or more blocks. Implementations
        /// that rewind only one block can ignore the target, while more advanced implementations
        /// can rewind a batch of multiple blocks but not overshooting the <paramref name="target"/>.</remarks>
        Task<RewindState> RewindAsync(HashHeightPair target);

        /// <summary>Execute header validation rules.</summary>
        /// <param name="header">The chained header that is going to be validated.</param>
        /// <returns>Context that contains validation result related information.</returns>
        ValidationContext HeaderValidation(ChainedHeader header);

        /// <summary>Execute integrity validation rules.</summary>
        /// <param name="header">The chained header that is going to be validated.</param>
        /// <param name="block">The block that is going to be validated.</param>
        /// <returns>Context that contains validation result related information.</returns>
        ValidationContext IntegrityValidation(ChainedHeader header, Block block);

        /// <summary>Execute partial validation rules.</summary>
        /// <param name="header">The chained header that is going to be validated.</param>
        /// <param name="block">The block that is going to be validated.</param>
        /// <returns>Context that contains validation result related information.</returns>
        Task<ValidationContext> PartialValidationAsync(ChainedHeader header, Block block);

        /// <summary>Execute full validation rules.</summary>
        /// <param name="header">The chained header that is going to be validated.</param>
        /// <param name="block">The block that is going to be validated.</param>
        /// <returns>Context that contains validation result related information.</returns>
        Task<ValidationContext> FullValidationAsync(ChainedHeader header, Block block);
    }
}