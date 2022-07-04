﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.AsyncWork;
using Stratis.Bitcoin.Base;
using Stratis.Bitcoin.Base.Deployments;
using Stratis.Bitcoin.Configuration.Settings;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Features.Consensus.CoinViews;
using Stratis.Bitcoin.Utilities;
using TracerAttributes;

namespace Stratis.Bitcoin.Features.Consensus.Rules
{
    /// <summary>
    /// Extension of consensus rules that provide access to a store based on UTXO (Unspent transaction outputs).
    /// </summary>
    public class PowConsensusRuleEngine : ConsensusRuleEngine
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>The consensus db, containing all unspent UTXO in the chain.</summary>
        public ICoinView UtxoSet { get; }

        private readonly CoinviewPrefetcher prefetcher;

        public PowConsensusRuleEngine(Network network, ILoggerFactory loggerFactory, IDateTimeProvider dateTimeProvider, ChainIndexer chainIndexer,
            NodeDeployments nodeDeployments, ConsensusSettings consensusSettings, ICheckpoints checkpoints, ICoinView utxoSet, IChainState chainState,
            IInvalidBlockHashStore invalidBlockHashStore, INodeStats nodeStats, IAsyncProvider asyncProvider, ConsensusRulesContainer consensusRulesContainer)
            : base(network, loggerFactory, dateTimeProvider, chainIndexer, nodeDeployments, consensusSettings, checkpoints, chainState, invalidBlockHashStore, nodeStats, consensusRulesContainer)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);

            this.UtxoSet = utxoSet;
            this.prefetcher = new CoinviewPrefetcher(this.UtxoSet, chainIndexer, loggerFactory, asyncProvider, checkpoints);
        }

        /// <inheritdoc />
        [NoTrace]
        public override RuleContext CreateRuleContext(ValidationContext validationContext)
        {
            return new PowRuleContext(validationContext, this.DateTimeProvider.GetTimeOffset());
        }

        /// <inheritdoc />
        public override HashHeightPair GetBlockHash()
        {
            return this.UtxoSet.GetTipHash();
        }

        /// <inheritdoc />
        public override Task<RewindState> RewindAsync(HashHeightPair target)
        {
            var state = new RewindState()
            {
                BlockHash = this.UtxoSet.Rewind(target)
            };

            return Task.FromResult(state);
        }

        /// <inheritdoc />
        public override void Initialize(ChainedHeader chainTip)
        {
            base.Initialize(chainTip);

            this.UtxoSet.Initialize(chainTip, this.ChainIndexer);
        }

        public override async Task<ValidationContext> FullValidationAsync(ChainedHeader header, Block block)
        {
            ValidationContext result = await base.FullValidationAsync(header, block).ConfigureAwait(false);

            if ((result != null) && (result.Error == null))
            {
                // Notify prefetch manager about block that was validated so prefetch manager
                // can decide what coins we will most likely need for full validation in the near future.
                this.prefetcher.Prefetch(header);
            }

            return result;
        }

        public override void Dispose()
        {
            this.prefetcher.Dispose();

            var cache = this.UtxoSet as CachedCoinView;
            if (cache != null)
            {
                this.logger.LogInformation("Flushing Cache CoinView.");
                cache.Flush();
            }

            ((IDisposable)((CachedCoinView)this.UtxoSet).ICoindb).Dispose();
        }
    }
}
