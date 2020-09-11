using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DataContract.V1.Request;
using DataContract.V1.Response;
using Domain;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using WalletServiceApi.Utilities;

namespace WalletServiceApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly ITransactionExecutor _transactionExecutor;
        private readonly ITransactionTypeConverter _transactionTypeConverter;

        public WalletController(IRepository<Player> playerRepository,
            ITransactionExecutor transactionExecutor,
            ITransactionTypeConverter transactionTypeConverter)
        {
            _playerRepository = playerRepository;
            _transactionExecutor = transactionExecutor;
            _transactionTypeConverter = transactionTypeConverter;
        }

        [HttpGet(Constants.PlayerIdentifierArgumentRouteName)]
        public IActionResult GetBalance(Guid playerIdentifier)
        {
            Maybe<Player> player = _playerRepository.GetByIdentifier(playerIdentifier);

            if (player.HasNoValue)
            {
                return BadRequest("Player does not exist.");
            }

            PlayerBalanceDto playerBalanceDto = new PlayerBalanceDto()
            {
                PlayerIdentifier = playerIdentifier,
                Balance = player.Value.Wallet.GetBalance().Amount
            };

            return Ok(playerBalanceDto);

        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] RegisterWalletForPlayerDto registerWalletForPlayerDto)
        {
            Maybe<Player> playerFromRepository = _playerRepository.GetByIdentifier(registerWalletForPlayerDto.PlayerIdentifier);

            if (playerFromRepository.HasValue)
            {
                return BadRequest("Player's wallet already exists.");
            }
            
            Player player = new Player(registerWalletForPlayerDto.PlayerIdentifier);
            _playerRepository.Save(player);

            return Ok();
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> Transaction([FromBody] CreditTransactionDto creditTransactionDto)
        {
            Maybe<Player> playerFromRepository = _playerRepository.GetByIdentifier(creditTransactionDto.PlayerIdentifier);

            if (playerFromRepository.HasNoValue)
            {
                return BadRequest("Player does not exists");
            }

            Result<Money> money = Money.Create(creditTransactionDto.Amount);

            if (money.IsFailure)
            {
                return BadRequest(money.Error);
            }

            Result<TransactionAttempt> transaction = await _transactionExecutor.ExecuteTransaction(creditTransactionDto.TransactionIdentifier,
                playerFromRepository.Value, 
                money.Value,
                _transactionTypeConverter.Convert(creditTransactionDto.Type));

            if (transaction.IsFailure)
            {
                return BadRequest(transaction.Error);
            }

            CreditTransactionResult transactionResult = _transactionTypeConverter.Convert(transaction.Value.TransactionState);

            TransactionResultDto result = new TransactionResultDto
            {
                Identifier = transaction.Value.Identifier,
                Result = transactionResult
            };

            return Ok(result);
        }
    }
}
