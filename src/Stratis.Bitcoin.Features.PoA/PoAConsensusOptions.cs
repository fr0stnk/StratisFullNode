﻿using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.Protocol;

namespace Stratis.Bitcoin.Features.PoA
{
    public class PoAConsensusOptions : ConsensusOptions
    {
        /// <summary>Public keys and other federation members related information at the start of the chain.</summary>
        /// <remarks>
        /// Do not use this list anywhere except for at the initialization of the chain.
        /// Actual collection of the federation members can be changed with time.
        /// Use <see cref="IFederationManager.GetFederationMembers"/> as a source of
        /// up to date federation keys.
        /// </remarks>
        public List<IFederationMember> GenesisFederationMembers { get; protected set; }

        /// <summary>
        /// The number of elapsed seconds required between mined block.
        /// </summary>
        public uint TargetSpacingSeconds { get; protected set; }

        /// <summary>Adds capability of voting for adding\kicking federation members and other things.</summary>
        public bool VotingEnabled { get; protected set; }

        /// <summary>Makes federation members kick idle members.</summary>
        /// <remarks>Requires voting to be enabled to be set <c>true</c>.</remarks>
        public bool AutoKickIdleMembers { get; protected set; }

        /// <summary>Time that federation member has to be idle to be kicked by others in case <see cref="AutoKickIdleMembers"/> is enabled.</summary>
        public uint FederationMemberMaxIdleTimeSeconds { get; protected set; }

        /// <summary>
        /// This currently only applies to  Cirrus Main Net.
        /// </summary>
        public uint? FederationMemberActivationTime { get; protected set; }

        /// <summary>
        /// The height at which a federation members will be resolved via the <see cref="FederationHistory"/> class.
        /// <para>
        /// A poll was incorrectly executed at block 1476880 because the legacy GetFederationMemberForTimestamp incorrectly
        /// derived a federation member for a mined block.
        /// </para>
        /// <para>
        /// After this block height, federation member votes will derived using the <see cref="FederationHistory.GetFederationMemberForBlock(ChainedHeader)"/>
        /// method which resolves the pubkey from the signature directly.
        /// </para>
        /// </summary>
        public int VotingManagerV2ActivationHeight { get; protected set; }

        /// <summary>
        /// This is the height on the main chain at which the dynamic fees paid to the multsig for interop conversion requests will activate.
        /// </summary>
        public int InterFluxV2MainChainActivationHeight { get; protected set; }

        /// <summary>
        /// The height at which inituitive mining slots become active.
        /// Legacy mining slots are determined by mining_slot = block_height % number_of_federation_members.
        /// Once the specified height is reached there should no longer be a shift in mining slots when new federation members are added/removed.
        /// </summary>
        public int GetMiningTimestampV2ActivationHeight { get; protected set; }

        /// <summary>
        /// The height at which inituitive mining slots are enfored without any lenience.
        /// Currently errors are sometimes suppressed if a federation change occurred.
        /// </summary>
        public int GetMiningTimestampV2ActivationStrictHeight { get; protected set; }

        /// <summary>
        /// Logic related to release 1.1.0.0 will activate at this height, this includes Poll Expiry and the Join Federation Voting Request consensus rule.
        /// </summary>
        public int Release1100ActivationHeight { get; protected set; }

        /// <summary>
        /// Polls are expired once the tip reaches a block this far beyond the poll start block.
        /// I.e. if (Math.Max(startblock + PollExpiryBlocks, PollExpiryActivationHeight) <= tip) (See IsPollExpiredAt)
        /// </summary>
        public int PollExpiryBlocks { get; protected set; }

        /// <summary>
        /// Defines when V2 of the contract serializer will be used.
        /// I.e if tip <= ContractSerializerV2ActivationHeight, V1 will be used.
        /// </summary>
        public int ContractSerializerV2ActivationHeight { get; protected set; }

        /// <summary>Initializes values for networks that use block size rules.</summary>
        /// <param name="maxBlockBaseSize">See <see cref="ConsensusOptions.MaxBlockBaseSize"/>.</param>
        /// <param name="maxStandardVersion">See <see cref="ConsensusOptions.MaxStandardVersion"/>.</param>
        /// <param name="maxStandardTxWeight">See <see cref="ConsensusOptions.MaxStandardTxWeight"/>.</param>
        /// <param name="maxBlockSigopsCost">See <see cref="ConsensusOptions.MaxBlockSigopsCost"/>.</param>
        /// <param name="maxStandardTxSigopsCost">See <see cref="ConsensusOptions.MaxStandardTxSigopsCost"/>.</param>
        /// <param name="genesisFederationMembers">See <see cref="GenesisFederationMembers"/>.</param>
        /// <param name="targetSpacingSeconds">See <see cref="TargetSpacingSeconds"/>.</param>
        /// <param name="votingEnabled">See <see cref="VotingEnabled"/>.</param>
        /// <param name="autoKickIdleMembers">See <see cref="AutoKickIdleMembers"/>.</param>
        /// <param name="federationMemberMaxIdleTimeSeconds">See <see cref="FederationMemberMaxIdleTimeSeconds"/>.</param>
        /// <param name="enforceMinProtocolVersionAtBlockHeight">See <see cref="ConsensusOptions.EnforceMinProtocolVersionAtBlockHeight"/>.</param>
        /// <param name="enforcedMinProtocolVersion"><see cref="ConsensusOptions.EnforcedMinProtocolVersion"/>.</param>
        /// <param name="federationMemberActivationTime"><see cref="FederationMemberActivationTime"/>.</param>
        /// <param name="votingManagerV2ActivationHeight"><see cref="VotingManagerV2ActivationHeight"/>.</param>
        /// <param name="interFluxV2MainChainActivationHeight"><see cref="InterFluxV2MainChainActivationHeight"/>.</param>
        /// <param name="getMiningTimestampV2ActivationHeight"><see cref="GetMiningTimestampV2ActivationHeight"/>.</param>
        /// <param name="getMiningTimestampV2ActivationStrictHeight"><see cref="GetMiningTimestampV2ActivationStrictHeight"/>.</param>
        /// <param name="release1100ActivationHeight"><see cref="Release1100ActivationHeight"/>.</param>
        /// <param name="pollExpiryBlocks"><see cref="PollExpiryBlocks"/>.</param>
        /// <param name="contractSerializerV2ActivationHeight"><see cref="ContractSerializerV2ActivationHeight"/>.</param>
        public PoAConsensusOptions(
            uint maxBlockBaseSize,
            int maxStandardVersion,
            int maxStandardTxWeight,
            int maxBlockSigopsCost,
            int maxStandardTxSigopsCost,
            List<IFederationMember> genesisFederationMembers,
            uint targetSpacingSeconds,
            bool votingEnabled,
            bool autoKickIdleMembers,
            uint federationMemberMaxIdleTimeSeconds = 60 * 60 * 24 * 7,
            int? enforceMinProtocolVersionAtBlockHeight = null,
            ProtocolVersion? enforcedMinProtocolVersion = null,
            uint? federationMemberActivationTime = null,
            int? votingManagerV2ActivationHeight = null,
            int? interFluxV2MainChainActivationHeight = null,
            int? getMiningTimestampV2ActivationHeight = null,
            int? getMiningTimestampV2ActivationStrictHeight = null,
            int? release1100ActivationHeight = null,
            int? pollExpiryBlocks = null,
            int? contractSerializerV2ActivationHeight = null)
                : base(maxBlockBaseSize, maxStandardVersion, maxStandardTxWeight, maxBlockSigopsCost, maxStandardTxSigopsCost, witnessScaleFactor: 1)
        {
            this.GenesisFederationMembers = genesisFederationMembers;
            this.TargetSpacingSeconds = targetSpacingSeconds;
            this.VotingEnabled = votingEnabled;
            this.AutoKickIdleMembers = autoKickIdleMembers;
            this.FederationMemberMaxIdleTimeSeconds = federationMemberMaxIdleTimeSeconds;
            this.InterFluxV2MainChainActivationHeight = 0;
            if (enforceMinProtocolVersionAtBlockHeight.HasValue)
                this.EnforceMinProtocolVersionAtBlockHeight = enforceMinProtocolVersionAtBlockHeight.Value;
            if (enforcedMinProtocolVersion.HasValue)
                this.EnforcedMinProtocolVersion = enforcedMinProtocolVersion.Value;
            this.FederationMemberActivationTime = federationMemberActivationTime;
            if (pollExpiryBlocks.HasValue)
                this.PollExpiryBlocks = pollExpiryBlocks.Value;
            if (interFluxV2MainChainActivationHeight.HasValue)
                this.InterFluxV2MainChainActivationHeight = interFluxV2MainChainActivationHeight.Value;
            if (getMiningTimestampV2ActivationHeight.HasValue)
                this.GetMiningTimestampV2ActivationHeight = getMiningTimestampV2ActivationHeight.Value;
            if (getMiningTimestampV2ActivationStrictHeight.HasValue)
                this.GetMiningTimestampV2ActivationStrictHeight = getMiningTimestampV2ActivationStrictHeight.Value;
            if (votingManagerV2ActivationHeight.HasValue)
                this.VotingManagerV2ActivationHeight = votingManagerV2ActivationHeight.Value;
            if (release1100ActivationHeight.HasValue)
                this.Release1100ActivationHeight = release1100ActivationHeight.Value;
            if (contractSerializerV2ActivationHeight.HasValue)
                this.ContractSerializerV2ActivationHeight = contractSerializerV2ActivationHeight.Value;

            if (this.AutoKickIdleMembers && !this.VotingEnabled)
                throw new ArgumentException("Voting should be enabled for automatic kicking to work.");
        }
    }
}
