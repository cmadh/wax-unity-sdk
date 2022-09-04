using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Api;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// The EOSIO Client wrapper to interact with the EOSIO chains.
    /// </summary>
    public class EosioClient
    {

        #region Events

        /// <summary>
        /// Occurs when creating a new transaction.
        /// </summary>
        public event EventHandler<TransactionEventArgs> CreatingTransaction;

        #endregion

        #region Fields

        protected EosioClientConfig config;

        #endregion

        #region Properties

        public EosioChainHttpApi ChainApi { get; private set; }

        public EosioHistoryHttpApi HistoryApi { get; private set; }

        public AbiTypeWriter AbiWriter { get; private set; }

        public AbiTypeReader AbiReader { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="EosioClient"/>.
        /// </summary>
        /// <param name="config">The configuration</param>
        public EosioClient(EosioClientConfig config)
        {
            this.config = config;
            if (this.config == null)
            {
                throw new ArgumentNullException("config");
            }

            ChainApi = new EosioChainHttpApi(this.config.HttpEndpoint);
            HistoryApi = new EosioHistoryHttpApi(this.config.HttpEndpoint);
            AbiWriter = new AbiTypeWriter(this);
            AbiReader = new AbiTypeReader(this);
        }

        #region Public Methods

        /// <summary>
        /// Gets the blockchain information
        /// </summary>
        /// <returns>Returns the blockchain information</returns>
        public UniTask<GetInfoResponse> GetInfo()
        {
            return ChainApi.GetInfo();
        }

        /// <summary>
        /// Gets the account information for the specified <paramref name="accountName"/>.
        /// </summary>
        /// <param name="accountName">The account name to get the information for</param>
        /// <returns>Returns the account information</returns>
        public UniTask<GetAccountResponse> GetAccount(string accountName)
        {
            return ChainApi.GetAccount(new GetAccountRequest()
            {
                account_name = accountName
            });
        }

        /// <summary>
        /// Gets the smart contract information
        /// </summary>
        /// <param name="accountName">The smart contract's account name</param>
        /// <param name="codeAsWasm">Whether to query the code as WASM or not</param>
        /// <returns>Returns the smart contract information</returns>
        public UniTask<GetCodeResponse> GetCode(string accountName, bool codeAsWasm)
        {
            return ChainApi.GetCode(new GetCodeRequest()
            {
                account_name = accountName,
                code_as_wasm = codeAsWasm
            });
        }

        /// <summary>
        /// Gets the smart contract's ABI information.
        /// </summary>
        /// <param name="accountName">The smart contract's account name</param>
        /// <returns>Returns the smart contract's ABI information</returns>
        public async UniTask<Abi> GetAbi(string accountName)
        {
            return (await ChainApi.GetAbi(new GetAbiRequest()
            {
                account_name = accountName
            })).abi;
        }

        /// <summary>
        /// Gets the smart contract's ABI information in raw form.
        /// </summary>
        /// <param name="accountName">The smart contract's account name</param>
        /// <returns>Returns the smart contract's ABI information as Base64FcString</returns>
        public UniTask<GetRawAbiResponse> GetRawAbi(string accountName)
        {
            return ChainApi.GetRawAbi(new GetRawAbiRequest()
            {
                account_name = accountName,
            });
        }

        /// <summary>
        /// Gets the smart contract's raw WASM and ABI information
        /// </summary>
        /// <param name="accountName">The smart contract's account name</param>
        /// <returns>Returns the smart contract's WASM and ABI information</returns>
        public UniTask<GetRawCodeAndAbiResponse> GetRawCodeAndAbi(string accountName)
        {
            return ChainApi.GetRawCodeAndAbi(new GetRawCodeAndAbiRequest()
            {
                account_name = accountName
            });
        }

        /// <summary>
        /// Transforms the action data to packed binary format.
        /// </summary>
        /// <param name="code">The smart contract's account name</param>
        /// <param name="action">The action name</param>
        /// <param name="data">The action data object </param>
        /// <returns>Returns the action data in binary format</returns>
        public async UniTask<string> AbiJsonToBin(string code, string action, object data)
        {
            return (await ChainApi.AbiJsonToBin(new AbiJsonToBinRequest()
            {
                code = code,
                action = action,
                args = data
            })).binargs;
        }

        /// <summary>
        /// Transforms the action data from packed binary format to object.
        /// </summary>
        /// <param name="code">The smart contract's account name</param>
        /// <param name="action">The action name</param>
        /// <param name="data">The action data in packed binary format </param>
        /// <returns></returns>
        public async UniTask<object> AbiBinToJson(string code, string action, string data)
        {
            return (await ChainApi.AbiBinToJson(new AbiBinToJsonRequest()
            {
                code = code,
                action = action,
                binargs = data
            })).args;
        }

        /// <summary>
        /// Gets the required keys to sign the given transaction.
        /// </summary>
        /// <param name="availableKeys">The available public keys list</param>
        /// <param name="trx">The transaction requiring signatures</param>
        /// <returns>Returns the required public keys</returns>
        public async UniTask<List<string>> GetRequiredKeys(List<string> availableKeys, Transaction trx)
        {
            int actionIndex = 0;
            var abiResponses = await TransactionUtility.GetTransactionAbis(this, trx);

            foreach (var action in trx.ContextFreeActions)
            {
                action.Data.SetData(HexUtility.ToHexString(AbiWriter.PackActionData(action, abiResponses[actionIndex++])));
            }

            foreach (var action in trx.Actions)
            {
                action.Data.SetData(HexUtility.ToHexString(AbiWriter.PackActionData(action, abiResponses[actionIndex++])));
            }

            return (await ChainApi.GetRequiredKeys(new GetRequiredKeysRequest()
            {
                available_keys = availableKeys,
                transaction = trx
            })).required_keys;
        }

        /// <summary>
        /// Get the blockchain block information.
        /// </summary>
        /// <param name="blockNumOrId">The block number or id to get the information for</param>
        /// <returns>Returns the block information</returns>
        public UniTask<GetBlockResponse> GetBlock(string blockNumOrId)
        {
            return ChainApi.GetBlock(new GetBlockRequest()
            {
                block_num_or_id = blockNumOrId
            });
        }

        /// <summary>
        /// Gets the block header state information.
        /// </summary>
        /// <param name="blockNumOrId">The block number or id</param>
        /// <returns>Returns the block's header state information</returns>
        public UniTask<GetBlockHeaderStateResponse> GetBlockHeaderState(string blockNumOrId)
        {
            return ChainApi.GetBlockHeaderState(new GetBlockHeaderStateRequest()
            {
                block_num_or_id = blockNumOrId
            });
        }

        /// <summary>
        /// Retrieves the contents of a database table.
        /// <br/>
        /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_table_rows"/>
        /// </summary>
        /// <typeparam name="TRowType">The type for the table rows</typeparam>
        /// <param name="request">The request</param>
        /// <returns>Returns all the rows that have been retrieved and provides whether there are more rows available to be retrieved</returns>
        public async UniTask<GetTableRowsResponse<TRowType>> GetTableRows<TRowType>(GetTableRowsRequest request)
        {
            if (request.json)
            {
                return await ChainApi.GetTableRows<TRowType>(request);
            }
            else
            {
                var apiResult = await ChainApi.GetTableRows(request);
                var result = new GetTableRowsResponse<TRowType>()
                {
                    more = apiResult.more
                };

                var unpackedRows = new List<TRowType>();

                var abi = await AbiUtility.GetAbi(this, request.code);
                var table = abi.Tables.First(t => t.name == request.table);

                foreach (var rowData in apiResult.rows)
                {
                    unpackedRows.Add(AbiReader.DeserializeStructData<TRowType>(table.type, (string)rowData, abi));
                }

                result.rows = unpackedRows;
                return result;
            }
        }

        /// <summary>
        /// Retrieves the contents of a database table.
        /// <br/>
        /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_table_rows"/>
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Returns all the rows that have been retrieved and provides whether there are more rows available to be retrieved</returns>
        public async UniTask<GetTableRowsResponse> GetTableRows(GetTableRowsRequest request)
        {
            var result = await ChainApi.GetTableRows(request);

            if (!request.json)
            {
                var unpackedRows = new List<object>();

                var abi = await AbiUtility.GetAbi(this, request.code);
                var table = abi.Tables.First(t => t.name == request.table);

                foreach (var rowData in result.rows)
                {
                    unpackedRows.Add(AbiReader.DeserializeStructData(table.type, (string)rowData, abi));
                }

                result.rows = unpackedRows;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the balance of an account for a given currency.
        /// <br/>
        /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_currency_balance"/>
        /// </summary>
        /// <param name="code">The token's smart contract account</param>
        /// <param name="account">The account name to retrieve the balance</param>
        /// <param name="symbol">A string representation of an EOSIO symbol, composed of a float with a precision of 4, and a symbol composed of capital letters between 1-7 letters separated by a space, example 1.0000 ABC</param>
        /// <returns>Returns the token balances</returns>
        public async UniTask<List<string>> GetCurrencyBalance(string code, string account, string symbol)
        {
            return (await ChainApi.GetCurrencyBalance(new GetCurrencyBalanceRequest()
            {
                code = code,
                account = account,
                symbol = symbol
            })).assets;
        }

        /// <summary>
        /// Retrieves currency stats.
        /// <br/>
        /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_currency_stats"/>
        /// </summary>
        /// <param name="code">The token's smart contract account</param>
        /// <param name="symbol">A string representation of an EOSIO symbol, composed of a float with a precision of 4, and a symbol composed of capital letters between 1-7 letters separated by a space, example 1.0000 ABC.</param>
        /// <returns>Returns the currencies statistics</returns>
        public async UniTask<Dictionary<string, CurrencyStat>> GetCurrencyStats(string code, string symbol)
        {
            return (await ChainApi.GetCurrencyStats(new GetCurrencyStatsRequest()
            {
                code = code,
                symbol = symbol
            })).stats;
        }

        /// <summary>
        /// Retrieves producers list.
        /// <br/>
        /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_producers"/>
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Returns the producers information</returns>
        public async UniTask<GetProducersResponse> GetProducers(GetProducersRequest request)
        {
            var result = await ChainApi.GetProducers(request);

            if (!request.json)
            {
                var unpackedRows = new List<object>();

                foreach (var rowData in result.rows)
                {
                    unpackedRows.Add(AbiReader.DeserializeType<Producer>((string)rowData));
                }

                result.rows = unpackedRows;
            }

            return result;
        }

        /// <summary>
        ///  Retrieve the producer schedule.
        /// </summary>
        /// <returns>Returns the producers schedule such as active, pending and proposed schedule</returns>
        public UniTask<GetProducerScheduleResponse> GetProducerSchedule()
        {
            return ChainApi.GetProducerSchedule();
        }

        /// <summary>
        /// Retrieves the scheduled transaction.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Returns the scheduled transactions and whether there are more available to be fetched</returns>
        public async UniTask<GetScheduledTransactionsResponse> GetScheduledTransactions(GetScheduledTransactionsRequest request)
        {
            var result = await ChainApi.GetScheduledTransactions(request);

            if (!request.json)
            {
                foreach (var trx in result.transactions)
                {
                    try
                    {
                        trx.transaction = await AbiReader.DeserializePackedTransaction((string)trx.transaction);
                    }
                    catch (Exception)
                    {
                        // Transactions with invalid ABI are ignored
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a signed transaction using the signature provider and broadcasts it to the network
        /// </summary>
        /// <param name="trx">The transaction to send</param>
        /// <param name="requiredKeys">The custom required keys to sign transaction (overrides the existing ones)</param>
        /// <returns>Returns the transaction ID</returns>
        public async UniTask<string> CreateTransaction(Transaction trx, List<string> requiredKeys = null)
        {
            CreatingTransaction?.Invoke(this, new TransactionEventArgs(trx));
            var signedTrx = await SignTransaction(trx, requiredKeys);
            return await BroadcastTransaction(signedTrx);
        }

        /// <summary>
        /// Creates a signed transaction using the signature provider.
        /// </summary>
        /// <param name="trx">The transaction to sign</param>
        /// <param name="requiredKeys">The custom required keys to sign transaction (overrides the existing ones)</param>
        /// <returns>Returns the transaction ID</returns>
        public async UniTask<SignedTransaction> SignTransaction(Transaction trx, List<string> requiredKeys = null)
        {
            if (trx == null)
                throw new ArgumentNullException("Transaction");

            if (this.config.SignProvider == null)
                throw new ArgumentNullException("SignProvider");

            GetInfoResponse getInfoResult = null;
            string chainId = this.config.ChainId;

            if (string.IsNullOrWhiteSpace(chainId))
            {
                getInfoResult = await ChainApi.GetInfo();
                chainId = getInfoResult.chain_id;
            }

            var expirationDate = DateTime.Parse(trx.Expiration, System.Globalization.CultureInfo.InvariantCulture);
            if (expirationDate == DateTime.MinValue ||
                trx.RefBlockNum == 0 ||
                trx.RefBlockPrefix == 0)
            {
                if (getInfoResult == null)
                    getInfoResult = await ChainApi.GetInfo();

                var taposBlockNum = getInfoResult.head_block_num - (int)this.config.BlocksBehind;

                if ((taposBlockNum - getInfoResult.last_irreversible_block_num) < 2)
                {
                    var getBlockResult = await ChainApi.GetBlock(new GetBlockRequest()
                    {
                        block_num_or_id = taposBlockNum.ToString()
                    });
                    trx.Expiration = getBlockResult.timestamp.AddSeconds(this.config.ExpireSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    trx.RefBlockNum = (ushort)(getBlockResult.block_num & 0xFFFF);
                    trx.RefBlockPrefix = getBlockResult.ref_block_prefix;
                }
                else
                {
                    var getBlockHeaderState = await ChainApi.GetBlockHeaderState(new GetBlockHeaderStateRequest()
                    {
                        block_num_or_id = taposBlockNum.ToString()
                    });
                    trx.Expiration = getBlockHeaderState.header.timestamp.AddSeconds(this.config.ExpireSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    trx.RefBlockNum = (ushort)(getBlockHeaderState.block_num & 0xFFFF);
                    trx.RefBlockPrefix = Convert.ToUInt32(HexUtility.ReverseHex(getBlockHeaderState.id.Substring(16, 8)), 16);
                }
            }

            var packedTrx = await AbiWriter.SerializeTransaction(trx);

            if (requiredKeys == null || requiredKeys.Count == 0)
            {
                var availableKeys = await this.config.SignProvider.GetAvailableKeys();
                requiredKeys = await GetRequiredKeys(availableKeys.ToList(), trx);
            }

            IEnumerable<string> abis = null;

            if (trx.Actions != null)
                abis = trx.Actions.Select(a => a.Account.Value);

            var signatures = await this.config.SignProvider.Sign(chainId, requiredKeys, packedTrx, abis);

            return new SignedTransaction()
            {
                Signatures = signatures,
                PackedTransaction = packedTrx
            };
        }

        /// <summary>
        /// Broadcasts the signed transaction to the network.
        /// </summary>
        /// <param name="strx">The signed transaction to send</param>
        /// <returns>Returns the transaction ID</returns>
        public async UniTask<string> BroadcastTransaction(SignedTransaction strx)
        {
            if (strx == null)
                throw new ArgumentNullException("SignedTransaction");

            var result = await ChainApi.PushTransaction(new PushTransactionRequest()
            {
                signatures = strx.Signatures.ToArray(),
                compression = 0,
                packed_context_free_data = "",
                packed_trx = HexUtility.ToHexString(strx.PackedTransaction)
            });
            return result.transaction_id;
        }

        /// <summary>
        /// Retrieve all actions with specific account name referenced in authorization or receiver
        /// </summary>
        /// <param name="accountName">The name of account to query on (required)</param>
        /// <param name="pos">The sequence number of action for this account, -1 for last (optional)</param>
        /// <param name="offset">[pos, pos + offset] for positive offset or [pos - offset, pos] for negative offset</param>
        /// <returns>Returns all the actions with the specified account name</returns>
        [Obsolete("https://developers.eos.io/manuals/eos/v2.0/nodeos/plugins/history_api_plugin/index")]
        public UniTask<GetActionsResponse> GetActions(string accountName, int pos, int offset)
        {
            return HistoryApi.GetActions(new GetActionsRequest()
            {
                account_name = accountName,
                pos = pos,
                offset = offset
            });
        }

        /// <summary>
        /// Retrieves the transaction information from the blockchain.
        /// </summary>
        /// <param name="transactionId">ID of the transaction to retrieve (required)</param>
        /// <param name="blockNumberHint">The block number this transaction may be in</param>
        /// <returns>Returns the transaction information</returns>
        [Obsolete("https://developers.eos.io/manuals/eos/v2.0/nodeos/plugins/history_api_plugin/index")]
        public UniTask<GetTransactionResponse> GetTransaction(string transactionId, uint? blockNumberHint = null)
        {
            return HistoryApi.GetTransaction(new GetTransactionRequest()
            {
                id = transactionId,
                block_num_hint = blockNumberHint
            });
        }

        /// <summary>
        /// Retrieves all accounts associated with a defined public key
        /// </summary>
        /// <remarks>
        /// This command will not return privileged accounts.
        /// </remarks>
        /// <param name="publicKey">The public key to retrieve accounts for</param>
        /// <returns>account names</returns>
        [Obsolete("https://developers.eos.io/manuals/eos/v2.0/nodeos/plugins/history_api_plugin/index")]
        public async UniTask<List<string>> GetKeyAccounts(string publicKey)
        {
            return (await HistoryApi.GetKeyAccounts(new GetKeyAccountsRequest()
            {
                public_key = publicKey
            })).account_names;
        }

        /// <summary>
        /// Retrieves all the controlled accounts by a given account name.
        /// </summary>
        /// <param name="accountName">The account name to search</param>
        /// <returns>Returns the controlled account names</returns>
        [Obsolete("https://developers.eos.io/manuals/eos/v2.0/nodeos/plugins/history_api_plugin/index")]
        public async UniTask<List<string>> GetControlledAccounts(string accountName)
        {
            return (await HistoryApi.GetControlledAccounts(new GetControlledAccountsRequest()
            {
                controlling_account = accountName
            })).controlled_accounts;
        }

        #endregion

    }

    public class TransactionEventArgs : EventArgs
    {

        public readonly Transaction Transaction;

        public TransactionEventArgs(Transaction transaction)
        {
            this.Transaction = transaction;
        }

    }

}